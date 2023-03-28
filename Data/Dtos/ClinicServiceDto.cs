using ValenciaBot.Data.Abstract;

namespace ValenciaBot.Data.Dto;

public class ClinicServiceDto
{
    public int Id { get; set; }
    public Guid entityGuid { get; set; } = Guid.NewGuid();
    public ClinicDto Clinic { get; set; }
    public ServiceDto Service { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsSpecial { get; set; }
    public List<OperatingHourDto> DaysOffered { get; set; }
}