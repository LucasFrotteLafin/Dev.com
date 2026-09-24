namespace DevCom.Domain.Entities;

public class DevSkill
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public int DevId { get; set; }
    public User Dev { get; set; } = null!;
}
