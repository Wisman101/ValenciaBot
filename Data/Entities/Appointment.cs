using ValenciaBot.Data.Abstract;

namespace ValenciaBot.Data.Entities;

public class Appointment : BaseEntity
{
    public Client client { get; set; }
    public ClinicService service { get; set; }
    public DateTime BookingDate { get; set; }
    public string preferedTime { get; set;}
    public bool confirmed { get; set; }
    public DateTime? ConfirmedDate { get; set; }
    public string? ConfirmedTime { get; set; }
}