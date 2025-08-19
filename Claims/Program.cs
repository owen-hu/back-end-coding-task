using Claims.DataLayer.Auditing;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using Claims.Core;
using Claims.DataLayer.Claims;
using Testcontainers.MongoDb;
using Testcontainers.MsSql;

var builder = WebApplication.CreateBuilder(args);

// Start Testcontainers for SQL Server and MongoDB
var sqlContainer = (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
        ? new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        : new()

    ).Build();

var mongoContainer = new MongoDbBuilder()
    .WithImage("mongo:latest")
    .Build();

await sqlContainer.StartAsync();
await mongoContainer.StartAsync();

// Add services to the container.
builder.Services
    .AddControllers()
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

var databaseName = "claims";
var connectionString = sqlContainer.GetConnectionString().Replace("Database=master", $"Database={databaseName}");
var mongoConnectionString = mongoContainer.GetConnectionString();

builder.Services.AddDbContext<AuditContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<ClaimsContext>(options =>
{
    var client = new MongoClient(mongoConnectionString);
    var database = client.GetDatabase(builder.Configuration["MongoDb:DatabaseName"]); // Use a default/test database name
    options.UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName);
});

//Add necessary Scoped Objects
builder.Services.AddScoped<IRepository<Claim>, ClaimsRepository>();
builder.Services.AddScoped<IRepository<Cover>, CoversRepository>();
builder.Services.AddScoped<IPremiumCalculator, BadPremiumCalculator>();
builder.Services.AddScoped<IAuditor, Auditor>();
builder.Services.AddScoped<ClaimsService, ClaimsService>();
builder.Services.AddScoped<CoversService, CoversService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AuditContext>();
    context.Database.Migrate();
}
Console.WriteLine($"SQL Connection String: {connectionString}");
Console.WriteLine($"MongoDB Connection String: {mongoConnectionString}");

app.Run();

public partial class Program { }
