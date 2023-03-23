using Microsoft.EntityFrameworkCore;
using ValenciaBot.Data.Entities;
using ValenciaBot.Data.Mappings;

namespace ValenciaBot.Data
{
    public class MainContext : DbContext
    {
        public MainContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ConversationMapping());
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Conversation> conversations { get; set; }
        public DbSet<MessageSetup> MessageSetups { get; set; }

    }
}