namespace ValenciaBot.Data.Dto;

public class ClinicDto
{
    public int Id { get; set; }
    public Guid entityGuid { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public string Code { get; set; }
    public string County { get; set; }
    public string SubCounty { get; set; }
    public string Ward { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string LocationDescription { get; set; }
    public string Tel { get; set; }
    public string WhatsappNumber { get; set; }
    public string Email { get; set; }
    public bool IsActive { get; set; }
    public List<OperatingHourDto> OperatingHour { get; set; }
}