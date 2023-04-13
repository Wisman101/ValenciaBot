using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ValenciaBot.Data.Entities;

namespace ValenciaBot.Data.Mappings
{
    public class MessagesMapping : IEntityTypeConfiguration<Messages>
    {
        public void Configure(EntityTypeBuilder<Messages> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(e => e.MetaData).HasConversion(
                v => JsonConvert.SerializeObject(v,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                v => JsonConvert.DeserializeObject<JToken>(v,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            builder.Property(e => e.log).HasConversion(
                v => JsonConvert.SerializeObject(v,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                v => JsonConvert.DeserializeObject<JToken>(v,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }
    }
}