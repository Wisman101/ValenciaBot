using System.Net;
using System.Net.Http.Headers;
using dotenv.net;
using Newtonsoft.Json;

namespace ValenciaBot.HelperFunctions.Clickatell;

class Api
{
    //This function is in charge of converting the data into a json array and sending it to the rest sending controller.
    public static Task<HttpResponseMessage> SendMessage(string to, string message)
    {
        DotEnv.Load();
        var token = Environment.GetEnvironmentVariable("Clickatell_Api_Key");
        Dictionary<string, string> Params = new Dictionary<string, string>();
        Params.Add("channel", "whatsapp");
        Params.Add("to", to);
        Params.Add("content", message);
        string JsonArray = JsonConvert.SerializeObject(Params, Formatting.None);
        JsonArray = $"{{\"messages\": [{JsonArray.Replace("\\\"", "\"").Replace("\"[", "[").Replace("]\"", "]")}]}}";
        return Rest.Post(token, JsonArray);
    }

    //This function converts the recipients list into an array string so it can be parsed correctly by the json array.
    public static string CreateRecipientList(string to)
    {
        string[] tmp = to.Split(',');
        to = "[\"";
        to = to + string.Join("\",\"", tmp);
        to = to + "\"]";
        return to;
    }
    
}

class Rest
{
    //This takes the API Key and JSON array of data and posts it to the Message URL to send the SMS's
    public async static Task<HttpResponseMessage> Post(string Token, string json)
    {
        using (var httpClient = new HttpClient())
        {
            using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://platform.clickatell.com/v1/message"))
            {
                request.Headers.TryAddWithoutValidation("Accept", "application/json");
                request.Headers.TryAddWithoutValidation("Authorization", Token); 

                request.Content = new StringContent(json);
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json"); 

                var response = await httpClient.SendAsync(request);
                return response;
            }
        }

        
    }


   
}