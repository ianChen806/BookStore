using Mediator;

namespace Project2_Debug.Features.Members.Register;

public sealed record RegisterCommand(string Account, string Name, string Email) : IRequest<RegisterResult>;

public sealed record RegisterResult(int MemberId, int MemberLevelId);
