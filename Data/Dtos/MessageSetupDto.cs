using ValenciaBot.Data.Enum;

namespace ValenciaBot.Data.Dto;

public class MessageSetupDto
{
    public int Id { get; set; }
    public Guid entityGuid { get; set; } = Guid.NewGuid();
    public bool isActive { get; set; }
    public string Input { get; set; }
    public string Response { get; set; }
    public bool isDynamic { get; set; }
    public Key? key { get; set; }
    public int? ParentId { get; set; }
}