using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project1_BookStore.Data;

namespace Project1_BookStore.Controllers;

[ApiController]
[Route("ping")]
public class PingController : ControllerBase
{
    private readonly AppDbContext _db;

    public PingController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var userCount = await _db.Users.CountAsync(cancellationToken);
        return Ok(new { message = "pong", userCount });
    }
}
