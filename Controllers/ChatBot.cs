using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using ValenciaBot.Data;
using ValenciaBot.Data.Dto;
using ValenciaBot.Data.Entities;
using ValenciaBot.HelperFunctions.Clickatell;

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
    public void SendMessage()
    {
        Dictionary<string, string> Params = new Dictionary<string, string>();
        Params.Add("channel", "whatsapp");
        Params.Add("to", "254702964173");
        Params.Add("content", "Iko sawa");
        
        var response = Api.SendSMS("LNbvdZ8GQ1aHFpVYHIWjSw==", Params);
        Console.WriteLine(response);
    }
}