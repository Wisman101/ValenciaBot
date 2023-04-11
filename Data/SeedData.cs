using Newtonsoft.Json;
using ValenciaBot.Data.Entities;

namespace ValenciaBot.Data.SeedData;

public static class SeedData
{
    public static async Task Execute(MainContext context, IHostEnvironment env)
    {
        await SeedMessageSetup(context, env);
        await SeedClinics(context, env);
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

    #region Seed Clinics
    private static async Task SeedClinics(MainContext context, IHostEnvironment env)
    {
        string filePath = $"{env.ContentRootPath}/Data/SeedData/Clinics.json";
        var clinics = JsonConvert.DeserializeObject<List<Clinic>>(await File.ReadAllTextAsync(filePath));
        foreach(var clinic in clinics)
        {
            var exist = context.Clinics.Any(c => c.Code == clinic.Code);
            if(exist)
            {
                context.Clinics.Update(clinic);
            }
            else
            {
                await context.Clinics.AddAsync(clinic);
            }
        }
        
        await context.SaveChangesAsync();
    }
    #endregion

}