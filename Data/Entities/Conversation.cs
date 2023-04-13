using Newtonsoft.Json.Linq;
using ValenciaBot.Data.Abstract;
using ValenciaBot.Data.Enum;

namespace ValenciaBot.Data.Entities;

public class Conversation : BaseEntity
{
    public Client client { get; set; }
    public ServiceCategory category {get; set;}
    public JToken? TransitData { get; set; }
    public Status status { get; set; }
    public MessageSetup MessageSetup { get; set; }
    public List<Messages> Messages { get; set; } = new();
}