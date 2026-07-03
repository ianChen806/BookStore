using Microsoft.EntityFrameworkCore;
using Project1_BookStore.Data;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

// 骨架便利:啟動時確保 DB 存在並 seed 兩筆 user
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    if (!db.Users.Any())
    {
        db.Users.AddRange(
            new User { Account = "alice", Name = "Alice" },
            new User { Account = "bob", Name = "Bob" });
        db.SaveChanges();
    }
}

app.MapOpenApi();
app.MapScalarApiReference();
app.MapControllers();

app.Run();
