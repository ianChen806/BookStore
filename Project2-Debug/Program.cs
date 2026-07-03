using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Web;
using Project2_Debug.Data;
using Project2_Debug.Middleware;
using Scalar.AspNetCore;

var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddDbContext<BookstoreDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("Default")));
    // handler 需注入 scoped DbContext,故 Mediator lifetime 設 Scoped(預設 Singleton 會 captive dependency)
    builder.Services.AddMediator(options => options.ServiceLifetime = ServiceLifetime.Scoped);

    var app = builder.Build();

    app.UseMiddleware<ExceptionLoggingMiddleware>();

    app.MapSwagger("/openapi/{documentName}.json");
    app.MapScalarApiReference();
    app.MapControllers();

    app.Run();
}
catch (Exception exception)
{
    logger.Error(exception, "Application stopped because of an exception");
    throw;
}
finally
{
    LogManager.Shutdown();
}
