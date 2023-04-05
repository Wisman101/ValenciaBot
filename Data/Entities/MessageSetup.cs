using ValenciaBot.Data.Abstract;
using ValenciaBot.Data.Enum;

namespace ValenciaBot.Data.Entities;

public class MessageSetup : BaseEntity
{
    public string Input { get; set; }
    public string Response { get; set; }
    public bool isDynamic { get; set; }
    public Key key { get; set; }
    public MessageSetup? Parent { get; set; }
}