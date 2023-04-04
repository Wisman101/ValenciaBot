using ValenciaBot.Data.Abstract;

namespace ValenciaBot.Data.Dto;

public class ClinicServiceDto
{
    public int Id { get; set; }
    public Guid entityGuid { get; set; }
    public ClinicDto Clinic { get; set; }
    public ServiceDto Service { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsSpecial { get; set; }
    public List<OperatingHourDto> DaysOffered { get; set; }
}

public class ClinicServiceDtoPost
{
    public int Id { get; set; }
    public Guid entityGuid { get; set; } = Guid.NewGuid();
    public int ClinicId { get; set; }
    public int ServiceId { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsSpecial { get; set; }
    public List<OperatingHourDto> DaysOffered { get; set; }
}