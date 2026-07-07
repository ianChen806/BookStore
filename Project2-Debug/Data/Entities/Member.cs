namespace Project2_Debug.Data.Entities;

public class Member
{
    public int Id { get; set; }
    public string Account { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int MemberLevelId { get; set; }
    public MemberLevel? MemberLevel { get; set; }
    public DateTime CreatedAt { get; set; }
}
