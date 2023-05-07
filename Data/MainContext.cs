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
            modelBuilder.ApplyConfiguration(new MessagesMapping());
            modelBuilder.ApplyConfiguration(new ClinicOperatingHoursMapping());
            modelBuilder.ApplyConfiguration(new ServiceOperatingHoursMapping());
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Conversation> conversations { get; set; }
        public DbSet<MessageSetup> MessageSetups { get; set; }
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<ClinicService> ClinicServices { get; set; }
        public DbSet<ClinicOperatingHour> ClinicOperatingHours { get; set; }
        public DbSet<ServiceOperatingHour> ServiceOperatingHours { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
    }
}