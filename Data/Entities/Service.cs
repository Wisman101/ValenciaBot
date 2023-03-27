using ValenciaBot.Data.Abstract;

namespace ValenciaBot.Data.Entities;

public class Service : BaseEntity
{
    public string Name { get; set; }
    public string Code { get; set; }
}