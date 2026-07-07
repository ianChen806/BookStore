using Mediator;
using Microsoft.EntityFrameworkCore;
using Project2_Debug.Data;
using Project2_Debug.Data.Entities;

namespace Project2_Debug.Features.Members.Register;

public sealed class RegisterHandler : IRequestHandler<RegisterCommand, RegisterResult>
{
    private readonly BookstoreDbContext _db;
    private readonly ILogger<RegisterHandler> _logger;

    public RegisterHandler(BookstoreDbContext db, ILogger<RegisterHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async ValueTask<RegisterResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Register attempt for account {Account}", request.Account);

        // 1. 簡單邏輯檢查
        if (string.IsNullOrWhiteSpace(request.Account) ||
            string.IsNullOrWhiteSpace(request.Name) ||
            string.IsNullOrWhiteSpace(request.Email))
        {
            throw new ArgumentException("Account, Name and Email are required.");
        }
        // 2. Email 必須包含 '@',否則格式不合法
        if (request.Email.Contains('@'))
        {
            throw new ArgumentException("Email format is invalid.");
        }

        // 3. 帳號重複檢查
        var accountExists = await _db.Members
            .AnyAsync(m => m.Account == request.Account, cancellationToken);
        if (accountExists)
        {
            throw new InvalidOperationException($"Account '{request.Account}' already exists.");
        }

        // 4. 取得預設會員等級(seed 保證恰好一筆 IsDefault = true)
        var defaultLevel = await _db.MemberLevels
            .SingleAsync(x => x.IsDefault, cancellationToken);

        // 5. 新增會員
        var member = new Member
        {
            Account = request.Account,
            Name = request.Name,
            Email = request.Email,
            MemberLevelId = defaultLevel.Id,
            CreatedAt = DateTime.UtcNow,
        };
        // 6. 將新會員寫入資料庫,並取得資料庫自動產生的 Id
        _db.Members.Add(member);

        _logger.LogInformation("Member {MemberId} registered with level {LevelId}", member.Id, defaultLevel.Id);
        return new RegisterResult(member.Id, defaultLevel.Id);
    }
}
