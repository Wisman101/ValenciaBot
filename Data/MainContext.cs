using Microsoft.EntityFrameworkCore;

namespace ValenciaBot.Data
{
    public class MainContext : DbContext
    {
        public MainContext(DbContextOptions options) : base(options)
        {
        }
    }
}