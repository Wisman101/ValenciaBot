using ValenciaBot.Data.Abstract;

namespace ValenciaBot.Data.Entities;

public class Appointment : BaseEntity
{
    public Client client { get; set; }
    public ClinicService service { get; set; }
    public DateTime PreferredBookingDate { get; set; }
    public string PreferredTime { get; set;}
    public bool confirmed { get; set; }
    public User? ConfirmedBy { get; set; }
    public DateTime? ConfirmedDate { get; set; }
    public string? ConfirmedTime { get; set; }
    public string Remarks { get; set; }
}