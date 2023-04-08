using Newtonsoft.Json.Linq;
using ValenciaBot.Data.Abstract;

namespace ValenciaBot.Data.Entities;

public class Conversation : BaseEntity
{
    public Client client { get; set; }
    public MessageSetup MessageSetup { get; set; }
    public string Input { get; set; }
    public string Response { get; set; }
    public JToken MetaData { get; set; }
    public bool sent { get; set; }
    public JToken? log { get; set; }
}