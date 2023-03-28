using Newtonsoft.Json.Linq;
using ValenciaBot.Data.Abstract;

namespace ValenciaBot.Data.Entities;

public class ServiceOperatingHour : BaseEntity
{
    public Service Service { get; set; }
    public JToken Days { get; set; }
    public TimeOnly Start { get; set; }
    public TimeOnly End { get; set; }
}