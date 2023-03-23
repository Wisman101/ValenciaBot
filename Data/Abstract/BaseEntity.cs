using System.ComponentModel.DataAnnotations;

namespace ValenciaBot.Data.Abstract;

public class BaseEntity
{
    public int Id { get; set; }
    public Guid entityGuid { get; set; } = new Guid();
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public bool isDeleted { get; set; }
    public bool isActive { get; set; } = true;
    public DateTime LastModified { get; set; } = DateTime.UtcNow;
    public string? LastModifiedBy { get; set; }
}