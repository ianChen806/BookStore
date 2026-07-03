using Microsoft.EntityFrameworkCore;
using Project2_Debug.Data.Entities;

namespace Project2_Debug.Data;

public class BookstoreDbContext : DbContext
{
    public BookstoreDbContext(DbContextOptions<BookstoreDbContext> options) : base(options)
    {
    }

    public DbSet<Member> Members => Set<Member>();
    public DbSet<MemberLevel> MemberLevels => Set<MemberLevel>();
}
