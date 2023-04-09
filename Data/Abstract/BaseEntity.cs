using System.ComponentModel.DataAnnotations;

namespace ValenciaBot.Data.Abstract;

public class BaseEntity
{
    public int Id { get; set; }
    public Guid EntityGuid { get; set; } = Guid.NewGuid();
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime LastModified { get; set; } = DateTime.UtcNow;
    public string? LastModifiedBy { get; set; }
}