using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Web;
using Project2_Debug.Data;
using Project2_Debug.Data.Entities;
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
    builder.Services.AddMediator(options => options.ServiceLifetime = ServiceLifetime.Scoped);

    var app = builder.Build();

    app.UseMiddleware<ExceptionLoggingMiddleware>();

    // 骨架便利:啟動時確保 DB 存在並 seed 會員等級與既有會員
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<BookstoreDbContext>();
        db.Database.EnsureCreated();

        if (!db.MemberLevels.Any())
        {
            db.MemberLevels.AddRange(
                new MemberLevel { Name = "一般會員", IsDefault = true },
                new MemberLevel { Name = "VIP", IsDefault = false },
                new MemberLevel { Name = "SVIP", IsDefault = false });
            db.SaveChanges();
        }

        if (!db.Members.Any())
        {
            var normalId = db.MemberLevels.Single(l => l.IsDefault).Id;
            var vipId = db.MemberLevels.Single(l => l.Name == "VIP").Id;
            var svipId = db.MemberLevels.Single(l => l.Name == "SVIP").Id;
            db.Members.AddRange(
                new Member { Account = "alice", Name = "Alice", Email = "alice@mail.com", MemberLevelId = normalId, CreatedAt = DateTime.UtcNow },
                new Member { Account = "bob", Name = "Bob", Email = "bob@mail.com", MemberLevelId = vipId, CreatedAt = DateTime.UtcNow },
                new Member { Account = "carol", Name = "Carol", Email = "carol@mail.com", MemberLevelId = normalId, CreatedAt = DateTime.UtcNow },
                new Member { Account = "dave", Name = "Dave", Email = "dave@mail.com", MemberLevelId = svipId, CreatedAt = DateTime.UtcNow },
                new Member { Account = "erin", Name = "Erin", Email = "erin@mail.com", MemberLevelId = vipId, CreatedAt = DateTime.UtcNow },
                new Member { Account = "frank", Name = "Frank", Email = "frank@mail.com", MemberLevelId = normalId, CreatedAt = DateTime.UtcNow },
                new Member { Account = "grace", Name = "Grace", Email = "grace@mail.com", MemberLevelId = svipId, CreatedAt = DateTime.UtcNow },
                new Member { Account = "heidi", Name = "Heidi", Email = "heidi@mail.com", MemberLevelId = vipId, CreatedAt = DateTime.UtcNow });
            db.SaveChanges();
        }
    }

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
