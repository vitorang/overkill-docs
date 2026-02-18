using Microsoft.EntityFrameworkCore;
using OverkillDocs.Core.Interfaces;
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

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();
