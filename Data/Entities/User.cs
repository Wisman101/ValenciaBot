using ValenciaBot.Data.Abstract;

namespace ValenciaBot.Data.Entities;

public class User : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PayRollNumber { get; set; }
    public string Tel { get; set; }
    public string Password { get; set; }
    public bool Verified { get; set; }
    public string Role { get; set; }
}