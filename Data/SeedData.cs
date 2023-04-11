using Microsoft.EntityFrameworkCore;
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
            var existingSetup = await context.MessageSetups.FirstOrDefaultAsync(setup => setup.Id == messageSetup.Id);
            if(existingSetup is not null)
            {
                context.MessageSetups.Update(existingSetup);
            }
            else
            {
                await context.MessageSetups.AddAsync(messageSetup);
            }
            
            await context.SaveChangesAsync();
        }
        
        
    }
    #endregion

    #region Seed Clinics
    private static async Task SeedClinics(MainContext context, IHostEnvironment env)
    {
        string filePath = $"{env.ContentRootPath}/Data/SeedData/Clinics.json";
        var clinics = JsonConvert.DeserializeObject<List<Clinic>>(await File.ReadAllTextAsync(filePath));
        foreach(var clinic in clinics)
        {
            var existingClinic = await context.Clinics.FirstOrDefaultAsync(c => c.Code.ToLower() == clinic.Code.ToLower());
            if(existingClinic is not null)
            {
                context.Clinics.Update(existingClinic);
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