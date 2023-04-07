using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using ValenciaBot.Data;
using ValenciaBot.Data.Entities;
using ValenciaBot.HelperFunctions.Clickatell;
using dotenv.net;
using ValenciaBot.Data.Enum;

namespace ValenciaBot.Controllers.Clinics;

[ApiController]
[Route("api/[Controller]")]
public class ChatBotController : ControllerBase
{
    private readonly MainContext _context;
    private readonly IMapper _mapper;
    public ChatBotController(MainContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpPost]
    public void SendMessage(string to, string message)
    {
        DotEnv.Load();
        Dictionary<string, string> Params = new Dictionary<string, string>();
        Params.Add("channel", "whatsapp");
        Params.Add("to", to);
        Params.Add("content", message);
        
        var response = Api.SendSMS(Environment.GetEnvironmentVariable("Clickatell_Api_Key"), Params);
        Console.WriteLine(response);
    }

    [HttpPost("receive_message")]
    public async void ReceiveMessage([FromBody] JObject request, CancellationToken cancellationToken)
    {
        dynamic requestContent = request;
        var PhoneNumber = request["event"]["moText"][0]["from"].ToString();
        var profileName =  request["event"]["moText"][0]["whatsapp"]["profileName"].ToString();
        var client = await _context.Clients.FirstOrDefaultAsync(client => client.PhoneNumber == PhoneNumber);
        if(client is null)
        {
            client = new Client();
            client.PhoneNumber = PhoneNumber;
            client.CreatedBy = "System";
            client.Name = profileName;
            await _context.Clients.AddAsync(client);
        }

        var conversation = await _context.conversations.OrderBy(convo => convo.Created).LastOrDefaultAsync(convo => convo.client == client);
        var response = "";
        if(conversation is null)
        {
            var message = await _context.MessageSetups.FirstOrDefaultAsync(message => message.key == Key.Intro && !message.isDeleted);
            var convo = new Conversation
            {
                MetaData = JToken.FromObject(requestContent),
                client = client,
                MessageSetup = message,
                Input =  request["event"]["moText"][0]["content"].ToString(),
                Response = message.Response,
                CreatedBy = "System"
            };
            await _context.conversations.AddAsync(convo);
            
            this.SendMessage(PhoneNumber,message.Response);
        }
        else if(conversation.Created < DateTime.UtcNow.AddMinutes(-10))
        {
            var message = _context.MessageSetups.FirstOrDefault(message => message.key == Key.Begin);
            response = "Welcome Back!/n" + message.Response;
        }
        await _context.SaveChangesAsync(cancellationToken);
        Console.Write(request);
        // Conversation convo = new Conversation();
        // convo.MetaData = request;
        // await _context.conversations.AddAsync(convo);
        // await _context.SaveChangesAsync();
    }


}