using ValenciaBot.Data.Enum;

namespace ValenciaBot.Data.Dto;

public class MessageSetupDto
{
    public int Id { get; set; }
    public Guid entityGuid { get; set; } = Guid.NewGuid();
    public bool IsActive { get; set; }
    public string Input { get; set; }
    public string Response { get; set; }
    public bool IsDynamic { get; set; }
    public Key? Key { get; set; }
    public int? ParentId { get; set; }
    public List<MessageSetupDto> Children { get; set; }
}