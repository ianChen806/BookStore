using Mediator;
using Microsoft.AspNetCore.Mvc;
using Project2_Debug.Features.Members.List;
using Project2_Debug.Features.Members.Register;

namespace Project2_Debug.Controllers;

[ApiController]
[Route("member")]
public class MemberController : ControllerBase
{
    private readonly IMediator _mediator;

    public MemberController(IMediator mediator) => _mediator = mediator;

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new RegisterCommand(request.Account, request.Name, request.Email),
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("list")]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ListMembersQuery(), cancellationToken);
        return Ok(result);
    }

    public sealed record RegisterRequest(string Account, string Name, string Email);
}
