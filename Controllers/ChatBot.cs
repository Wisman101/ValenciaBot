using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using ValenciaBot.Data;
using ValenciaBot.Data.Entities;
using ValenciaBot.HelperFunctions.Clickatell;
using dotenv.net;
using ValenciaBot.Data.Enum;
using CSharpFunctionalExtensions;

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

    [HttpPost("receive_message")]
    public async Task<IActionResult> ReceiveMessage([FromBody] JObject request, CancellationToken cancellationToken)
    {
        var requestContent = request["event"]["moText"][0];
        var PhoneNumber = requestContent["from"].ToString();
        var profileName =  requestContent["whatsapp"]["profileName"].ToString();
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
        if(conversation is null || conversation.Created < DateTime.UtcNow.AddDays(-1))
        {
            var message = await _context.MessageSetups.FirstOrDefaultAsync(message => message.key == Key.Intro && !message.isDeleted);
            await _context.conversations.AddAsync(CreateConversation(client, requestContent, message, $"Hey {client.Name}, {message.Response}"));

            var message2 = await _context.MessageSetups.FirstOrDefaultAsync(message => message.key == Key.Begin && !message.isDeleted);
            await _context.conversations.AddAsync(CreateConversation(client, requestContent, message2, message2.Response));
        }
        else if(conversation.Created < DateTime.UtcNow.AddMinutes(-10))
        {
            var message = _context.MessageSetups.FirstOrDefault(message => message.key == Key.Begin);
            response = "Welcome Back!/n" + message.Response;
            await _context.conversations.AddAsync(CreateConversation(client, requestContent, message, response));
        }
        else
        {
            // if(!conversation.MessageSetup.isDynamic)
            // {
            //     var message = _context
            // }
        }
        await _context.SaveChangesAsync(cancellationToken);
        Console.Write(request);

        return NoContent();
    }

    public static Conversation CreateConversation(Client client, JToken data, MessageSetup message, string response)
    {
        var convo = new Conversation
        {
            MetaData = JToken.FromObject(data),
            client = client,
            MessageSetup = message,
            Input =  data["content"].ToString(),
            Response = response,
            CreatedBy = "System"
        };
        
        var httpResponse = SendMessage(client.PhoneNumber,message.Response).Result;
        convo.sent = httpResponse.IsSuccessStatusCode;
        convo.log = JToken.FromObject(httpResponse);

        return convo;
    }

    public static Task<HttpResponseMessage> SendMessage(string to, string message)
    {
        DotEnv.Load();
        Dictionary<string, string> Params = new Dictionary<string, string>();
        Params.Add("channel", "whatsapp");
        Params.Add("to", to);
        Params.Add("content", message);
        
        var response = Api.SendSMS(Environment.GetEnvironmentVariable("Clickatell_Api_Key"), Params);
        return response;
    }

}