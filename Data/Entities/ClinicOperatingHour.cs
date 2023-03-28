using Newtonsoft.Json.Linq;
using ValenciaBot.Data.Abstract;

namespace ValenciaBot.Data.Entities;

public class ClinicOperatingHour : BaseEntity
{
    public Clinic clinic { get; set; }
    public JArray Days { get; set; }
    public TimeOnly Start { get; set; }
    public TimeOnly End { get; set; }
}