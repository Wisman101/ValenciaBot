using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ValenciaBot.Data.Entities;

namespace ValenciaBot.Data.Mappings
{
    public class ClinicOperatingHoursMapping : IEntityTypeConfiguration<ClinicOperatingHour>
    {
        public void Configure(EntityTypeBuilder<ClinicOperatingHour> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(e => e.Days).HasConversion(
                v => JsonConvert.SerializeObject(v,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                v => JsonConvert.DeserializeObject<JToken>(v,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }
    }
}