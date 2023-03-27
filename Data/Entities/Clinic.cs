using ValenciaBot.Data.Abstract;

namespace ValenciaBot.Data.Entities;

public class Clinic : BaseEntity
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string County { get; set; }
    public string SubCounty { get; set; }
    public string Ward { get; set; }
    public string Lattitude { get; set; }
    public string Longitude { get; set; }
    public string Tel { get; set; }
    public string Email { get; set; }
    public List<OperatingHour> OperatingHour { get; set; }
}