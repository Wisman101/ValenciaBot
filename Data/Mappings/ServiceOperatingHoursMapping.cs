using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ValenciaBot.Data.Entities;

namespace ValenciaBot.Data.Mappings
{
    public class ServiceOperatingHoursMapping : IEntityTypeConfiguration<ServiceOperatingHour>
    {
        public void Configure(EntityTypeBuilder<ServiceOperatingHour> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(e => e.Days).HasConversion(
                v => JsonConvert.SerializeObject(v,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                v => JsonConvert.DeserializeObject<JArray>(v,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }
    }
}