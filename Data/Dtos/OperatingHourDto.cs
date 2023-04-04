using Newtonsoft.Json.Linq;

namespace ValenciaBot.Data.Dto;

public class OperatingHourDto
{
    public int Id { get; set; }
    public Guid entityGuid { get; set; } = Guid.NewGuid();
    public JToken Days { get; set; }
    public TimeOnly Start { get; set; }
    public TimeOnly End { get; set; }
}