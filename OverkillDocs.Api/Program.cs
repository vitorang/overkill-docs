using HashidsNet;
using Microsoft.EntityFrameworkCore;
using OverkillDocs.Api.Filters;
using OverkillDocs.Api.Handlers;
using OverkillDocs.Api.Hubs;
using OverkillDocs.Api.Middlewares;
using OverkillDocs.Core.Interfaces;
using OverkillDocs.Core.Security;
using OverkillDocs.Infrastructure.Cache;
using OverkillDocs.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        x => x.MigrationsAssembly("OverkillDocs.Infrastructure")
    ));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<UserContext>();
builder.Services.AddExceptionHandler<ExceptionHandler>();
builder.Services.AddSingleton(typeof(IAppCache<>), typeof(AppCache<>));

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



builder.Services.AddProblemDetails();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<AuthorizationFilter>();
});

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});
builder.Services.AddMemoryCache();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("LocalhostPolicy");

app.UseExceptionHandler();
app.UseMiddleware<SessionMiddleware>();

app.MapHub<MainHub>("/hubs/main");

app.MapControllers();

app.Run();
