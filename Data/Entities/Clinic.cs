using ValenciaBot.Data.Abstract;

namespace ValenciaBot.Data.Entities;

public class Clinic : BaseEntity
{
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
    public List<ClinicOperatingHour> OperatingHour { get; set; }
}