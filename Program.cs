using ClinicManagement;
using ClinicManagement.WebApi.Application;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
try
{
    // Add services to the container.
    builder.AddConfigurations().RegisterSerilog();
    builder.Services.AddControllers();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddApplication();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    //if (app.Environment.IsDevelopment())
    //{
    //    app.UseSwagger();
    //    app.UseSwaggerUI();
    //}

    await app.Services.InitializeDatabasesAsync();

    app.UseInfrastructure(builder.Configuration);
    app.MapEndpoints();
    app.Run();
}
catch (Exception ex) when (!ex.GetType().Name.Equals("HostAbortedException", StringComparison.Ordinal))
{
    StaticLogger.EnsureInitialized();
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    StaticLogger.EnsureInitialized();
    Log.Information("Server Shutting down...");
    Log.CloseAndFlush();
}
