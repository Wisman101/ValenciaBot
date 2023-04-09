using Newtonsoft.Json;
using ValenciaBot.Data.Entities;

namespace ValenciaBot.Data.SeedData;

public static class SeedData
{
    public static async Task Execute(MainContext context, IHostEnvironment env)
    {
        await SeedMessageSetup(context, env);
    }

    #region Seed Message Setup
    private static async Task SeedMessageSetup(MainContext context, IHostEnvironment env)
    {
        string filePath = $"{env.ContentRootPath}/Data/SeedData/MessageSetup.json";
        var messageSetups = JsonConvert.DeserializeObject<List<MessageSetup>>(await File.ReadAllTextAsync(filePath));
        foreach(var messageSetup in messageSetups)
        {
            var setup = context.MessageSetups.Any(setup => setup.Id == messageSetup.Id);
            if(setup)
            {
                context.MessageSetups.Update(messageSetup);
            }
            else
            {
                await context.MessageSetups.AddAsync(messageSetup);
            }
        }
        
        await context.SaveChangesAsync();
    }
    #endregion

}