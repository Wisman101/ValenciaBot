using ValenciaBot.Data.Abstract;

namespace ValenciaBot.Data.Entities;

public class Client : BaseEntity
{
    public string PhoneNumber { get; set; }
    public string? Name { get; set; }
    public string? UPI { get; set; } //Unique Patient Identifier
}