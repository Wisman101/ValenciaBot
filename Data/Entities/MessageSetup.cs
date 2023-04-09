using ValenciaBot.Data.Abstract;
using ValenciaBot.Data.Enum;

namespace ValenciaBot.Data.Entities;

public class MessageSetup : BaseEntity
{
    public string Input { get; set; }
    public string Response { get; set; }
    public bool IsDynamic { get; set; }
    public Key? Key { get; set; }
    public MessageSetup? Parent { get; set; }
    public List<MessageSetup> Children { get; set; }
}