using Mediator;

namespace Project2_Debug.Features.Members.List;

public sealed record ListMembersQuery : IRequest<IReadOnlyList<MemberListItem>>;

public sealed record MemberListItem(int Id, string Name, string LevelName);
