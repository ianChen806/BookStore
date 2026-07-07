using Mediator;
using Microsoft.EntityFrameworkCore;
using Project2_Debug.Data;

namespace Project2_Debug.Features.Members.List;

public sealed class ListMembersHandler : IRequestHandler<ListMembersQuery, IReadOnlyList<MemberListItem>>
{
    private readonly BookstoreDbContext _db;
    private readonly ILogger<ListMembersHandler> _logger;

    public ListMembersHandler(BookstoreDbContext db, ILogger<ListMembersHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async ValueTask<IReadOnlyList<MemberListItem>> Handle(ListMembersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Listing all members");

        // 回傳每位會員與其「會員等級名稱」
        var members = await _db.Members.ToListAsync(cancellationToken);

        var result = new List<MemberListItem>();
        foreach (var member in members)
        {
            // 逐一查詢每位會員的等級
            var level = await _db.MemberLevels
                .FirstAsync(l => l.Id == member.MemberLevelId, cancellationToken);
            result.Add(new MemberListItem(member.Id, member.Name, level.Name));
        }

        return result;
    }
}
