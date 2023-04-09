using System.Text.Json.Serialization;

namespace ValenciaBot.Data.Dto;

public class ServiceDto
{
    public int Id { get; set; }
    public Guid entityGuid { get; set; } = Guid.NewGuid();
    public bool IsActive { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
}