namespace ValenciaBot.Data.Dto;

public class ServiceDto
{
    public int Id { get; set; }
    public Guid entityGuid { get; set; }
     public bool isActive { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
}