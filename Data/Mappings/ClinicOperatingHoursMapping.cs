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
            builder.Property(e => e.Days).HasConversion(new JTokenValueConverter());
        }
    }

    public class JTokenValueConverter : ValueConverter<JArray, string>
    {
        public JTokenValueConverter() : base(
            jToken => jToken.ToString(),
            json => JArray.Parse(json)
        ) { }
    }
}