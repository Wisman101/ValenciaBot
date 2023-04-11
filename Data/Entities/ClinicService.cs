using ValenciaBot.Data.Abstract;

namespace ValenciaBot.Data.Entities;

public class ClinicService : BaseEntity
{
    public Clinic Clinic { get; set; }
    public Service Service { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsSpecial { get; set; }
    public List<ServiceOperatingHour>? DaysOffered { get; set; }
}