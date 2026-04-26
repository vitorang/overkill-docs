using HashidsNet;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OverkillDocs.Api.Constants;
using OverkillDocs.Api.Filters;
using OverkillDocs.Api.Handlers;
using OverkillDocs.Api.Hubs;
using OverkillDocs.Api.Middlewares;
using OverkillDocs.Core.Interfaces;
using OverkillDocs.Core.Security;
using OverkillDocs.Infrastructure.Cache.Memory;
using OverkillDocs.Infrastructure.Cache.Redis;
using OverkillDocs.Infrastructure.Data;
using OverkillDocs.Infrastructure.Interfaces;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


#region Banco de dados
string entityProviderConfig = builder.Configuration.GetValue<string>("FeatureFlags:Database")
    ?? throw new Exception("Banco de dados não definido");

var entityProvider = entityProviderConfig switch
{
    "sqlite" => EntityProvider.Sqlite,
    "sqlserver" => EntityProvider.SqlServer,
    "postgres" => EntityProvider.Postgres,
    _ => EntityProvider.Unknown
};

builder.Services.AddDbContext<AppDbContext>(options =>
{
    _ = entityProvider switch
    {
        EntityProvider.Sqlite => options.UseSqlite(
            builder.Configuration.GetConnectionString("Sqlite"),
            x => x.MigrationsAssembly("OverkillDocs.Migrator.Sqlite")),

        EntityProvider.SqlServer => options.UseSqlServer(
            builder.Configuration.GetConnectionString("SqlServer"),
            x => x.MigrationsAssembly("OverkillDocs.Migrator.SqlServer")),

        EntityProvider.Postgres => options.UseNpgsql(
            builder.Configuration.GetConnectionString("Postgres"),
            x => x.MigrationsAssembly("OverkillDocs.Migrator.Postgres")),

        _ => throw new Exception("Provedor de banco de dados não mapeado")
    };
});
#endregion


#region Injeção de dependências
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<UserContext>();
builder.Services.AddExceptionHandler<ExceptionHandler>();

builder.Services.AddSingleton<IHashids>(_ =>
{
    var salt = builder.Configuration["Hashids:Salt"];
    var minLength = builder.Configuration.GetValue<int>("Hashids:MinHashLength");
    return new Hashids(salt, minLength);
});

builder.Services.Scan(scan => scan
    .FromAssemblies(
        typeof(Program).Assembly,
        typeof(IUnitOfWork).Assembly,
        typeof(UnitOfWork).Assembly
        )

    .AddClasses(classes => classes.Where(
        type => type.Name.EndsWith("Service")
            || type.Name.EndsWith("Repository")))
    .AsImplementedInterfaces()
    .WithScopedLifetime());
#endregion


#region Filtros e middlewares
builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalhostPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddProblemDetails();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<AuthorizationFilter>();
});
#endregion


#region Cache
bool useRedis = builder.Configuration.GetValue<bool>("FeatureFlags:UseRedis");
string redisConnection = builder.Configuration.GetConnectionString("Redis")!;

if (useRedis)
{
    builder.Services.AddSingleton(typeof(IObjectCache<>), typeof(RedisObjectCache<>));
    builder.Services.AddSingleton(typeof(IListCache<>), typeof(RedisListCache<>));

    builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
        ConnectionMultiplexer.Connect(redisConnection));
}
else
{
    builder.Services.AddSingleton(typeof(IObjectCache<>), typeof(MemoryObjectCache<>));
    builder.Services.AddSingleton(typeof(IListCache<>), typeof(MemoryListCache<>));

    builder.Services.AddMemoryCache();
}
#endregion


#region SignalR
var signalRBuilder = builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.AddFilter<HubAuthorizationFilter>();
});

if (useRedis)
    signalRBuilder.AddStackExchangeRedis(redisConnection);
#endregion


var app = builder.Build();


#region Swagger
var swaggerRoute = "api/swagger";
app.UseSwagger(c =>
{
    c.RouteTemplate = $"{swaggerRoute}/{{documentName}}/swagger.json";
});
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint($"/{swaggerRoute}/v1/swagger.json", "OverkillDocs API");
    c.RoutePrefix = swaggerRoute;
});
#endregion


#region SPA e roteamento
app.UseDefaultFiles();
app.UseStaticFiles();

app.Map("/api/{*path}", (string path) => Results.NotFound());
app.MapFallbackToFile("index.html");
#endregion


app.UseCors("LocalhostPolicy");

app.UseExceptionHandler();
app.UseMiddleware<SessionMiddleware>();

app.MapHub<MainHub>(HubRoutes.Main);

app.MapControllers();

app.Run();

enum EntityProvider
{
    Sqlite,
    SqlServer,
    Postgres,
    Unknown
}

public partial class Program { }
