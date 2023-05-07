using ValenciaBot.Data.Dto;

namespace ValenciaBot.Data.Entities;

public class AppointmentDto
{
    public int Id { get; set; }
    public Guid entityGuid { get; set; }
    public Client client { get; set; }
    public ClinicServiceDto service { get; set; }
    public DateTime PreferredBookingDate { get; set; }
    public string PreferredTime { get; set;}
    public bool confirmed { get; set; }
    public User ConfirmedBy { get; set; }
    public DateTime? ConfirmedDate { get; set; }
    public string? ConfirmedTime { get; set; }
    public string Remarks { get; set; }
}

public class AppointmentDtoPost
{
    public int Id { get; set; }
    public Guid entityGuid { get; set; }
    public bool confirmed { get; set; }
    public DateTime? ConfirmedDate { get; set; }
    public string? ConfirmedTime { get; set; }
    public string Remarks { get; set; }
}