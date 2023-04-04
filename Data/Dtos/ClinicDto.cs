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
    public string Lattitude { get; set; }
    public string Longitude { get; set; }
    public string Tel { get; set; }
    public string Email { get; set; }
    public bool isActive { get; set; }
    public List<OperatingHourDto> OperatingHour { get; set; }
}