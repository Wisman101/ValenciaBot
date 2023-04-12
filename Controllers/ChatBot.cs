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
using static ChatFunctions;
using ValenciaBot.Data.Dto;
//using ValenciaBot.HelperFunctions.ChatFunctions;

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
        var contentType = request["event"]["moText"] == null ? request["event"]["moLocation"] : request["event"]["moText"];
        if(contentType == null) return BadRequest();

        var requestContent = contentType[0];
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

        var conversation = await _context.conversations.Include(convo => convo.MessageSetup)
            .OrderBy(convo => convo.Created)
            .LastOrDefaultAsync(convo => convo.client == client);
    
        if(request["event"]["moText"] == null)
        {
            if(conversation is null || conversation.MessageSetup.Key != Key.CurrentLocation) 
            {
                // TODO: code to return message invalid input.
                return BadRequest();
            }
        }
        else
        {
            if(requestContent["content"].ToString() == "00")
            {
                var message = await _context.MessageSetups.FirstOrDefaultAsync(message => message.Key == Key.Begin && !message.IsDeleted);
                await _context.conversations.AddAsync(CreateConversation(client, requestContent, message, message.Response));
                await _context.SaveChangesAsync(cancellationToken);
                return NoContent();
            }
        }
       
        
        var response = "";
        if(conversation is null || conversation.Created < DateTime.UtcNow.AddDays(-1))
        {
            var message = await _context.MessageSetups.FirstOrDefaultAsync(message => message.Key == Key.Intro && !message.IsDeleted);
            await _context.conversations.AddAsync(CreateConversation(client, requestContent, message, $"Hey {client.Name},\n {message.Response}"));

            var message2 = await _context.MessageSetups.FirstOrDefaultAsync(message => message.Key == Key.Begin && !message.IsDeleted);
            await _context.conversations.AddAsync(CreateConversation(client, requestContent, message2, message2.Response));
        }
        else if(conversation.Created < DateTime.UtcNow.AddMinutes(-10))
        {
            var message = _context.MessageSetups.FirstOrDefault(message => message.Key == Key.Begin);
            response = "*Welcome Back!*\n\n" + message.Response;
            await _context.conversations.AddAsync(CreateConversation(client, requestContent, message, response));
        }
        else
        {
            MessageSetup messageSetup = conversation.MessageSetup;
            
            if(!messageSetup.IsDynamic)
            {
                var message = await _context.MessageSetups
                    .FirstOrDefaultAsync(setup => setup.Parent == messageSetup 
                        && setup.Input == requestContent["content"].ToString());
                response = $"{message.Response}\n\n00: Home";
                if(!message.IsDynamic)
                {
                    await _context.conversations.AddAsync(CreateConversation(client, requestContent, message, response, conversation.serviceId));
                }
                else
                {
                    switch(message.Key)
                    {
                        case Key.CurrentLocation:
                            await _context.conversations.AddAsync(CreateConversation(client, requestContent, message, response, conversation.serviceId));
                            break;
                        case Key.Services:
                            var services = _context.Services.ToList();
                            response = $"{message.Response}";
                            foreach(var service in services)
                            {
                                response += $"\n{service.Id}: {service.Name}";
                            }
                            response += "\n\n00: Home";
                            await _context.conversations.AddAsync(CreateConversation(client, requestContent, message, response, conversation.serviceId));
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                switch(messageSetup.Key)
                {
                    case Key.CurrentLocation:
                        double latitude = requestContent["latitute"].Value<double>();
                        double longitude = requestContent["longitude"].Value<double>();
                        List<Model> clinics = await ChatFunctions.NearestClinics(latitude, longitude, _context, _mapper);
                        var NearestClinicMessage = _context.MessageSetups.FirstOrDefault(setup => setup.Key == Key.NearestClinic);
                        response = $"{NearestClinicMessage.Response}";
                        var count = 1;
                        foreach(var clinic in clinics)
                        {
                            response += $"{count}. {clinic.Clinic.Code} - {clinic.Clinic.Name} ({clinic.distance}Km)\nDirection: {GetDirectionLocationUrl(latitude, longitude, clinic.Clinic.Latitude, clinic.Clinic.Longitude)}\n";
                            count ++;
                        }

                        string response1;
                        if(conversation.category == ServiceCategory.Appointment)
                        {
                            response1 = "Kindly select the clinic you would wish to book an appointment from.\n\n Reply with the clinic code e.g 'EQA001'\n\n00: Home";

                        }
                        else
                        {
                            response1 = "To Get Clinic Details, Kindly Reply with the clinic code e.g 'EQA001'\n\n00: Home";
                        }
                        
                        await _context.conversations.AddAsync(CreateConversation(client, requestContent, NearestClinicMessage, response, conversation.serviceId));
                        await _context.conversations.AddAsync(CreateConversation(client, requestContent, NearestClinicMessage, response1, conversation.serviceId));
                        break;
                    case Key.NearestClinic:
                        var EQAclinic = await  _context.Clinics
                            .Include(clinic => clinic.OperatingHour)
                            .FirstOrDefaultAsync(clinic => clinic.Code == requestContent["content"].ToString());
                        if(EQAclinic is null)
                        {
                            NearestClinicMessage = await _context.MessageSetups.FirstOrDefaultAsync(setup => setup.Key == Key.NearestClinic);
                            response = $"Clinic with code {requestContent["content"].ToString()} Not found. Kindly reply with correct clinic code\n\n00: Home";
                            await _context.conversations.AddAsync(CreateConversation(client, requestContent, NearestClinicMessage, response, conversation.serviceId));
                        }
                        else
                        {
                            var ClinicDetailsMessage = await _context.MessageSetups.FirstOrDefaultAsync(setup => setup.Key == Key.ClinicDetails);
                            response = 
@$"*{EQAclinic.Name}*
--------------------------------
*Location:* {EQAclinic.LocationDescription}
*Pin:* {GetPinLocationUrl(EQAclinic.Latitude,EQAclinic.Longitude)}
*Tel:* {EQAclinic.Tel}
*Email:* {EQAclinic.Email}

*Operating Hours*{GetOperatingHours(_mapper.Map<List<OperatingHourDto>>(EQAclinic.OperatingHour))}

{GetServicesAvailable(EQAclinic, _context, _mapper)}

00: Home";

                            await _context.conversations.AddAsync(CreateConversation(client, requestContent, ClinicDetailsMessage, response, conversation.serviceId));

                            if(conversation.category == ServiceCategory.Appointment)
                            {
                                var appointmentDateMessage = _context.MessageSetups.FirstOrDefault(setup => setup.Key == Key.AppointmentDate);
                                response1 = appointmentDateMessage.Response;
                                await _context.conversations.AddAsync(CreateConversation(client, requestContent, appointmentDateMessage, response1, conversation.serviceId));
                            }
                           

                        }
                        
                        break;
                    case Key.Services:
                        if(conversation.category == ServiceCategory.Appointment)
                        {
                            var AppointmentClinicMessage = await _context.MessageSetups.FirstOrDefaultAsync(setup => setup.Key == Key.SearchByLocation);
                            response = $"Kindly proceed to search for a clinic to schedule the appointment\n\n{AppointmentClinicMessage.Response}";
                            await _context.conversations.AddAsync(CreateConversation(client, requestContent, AppointmentClinicMessage, response, requestContent["content"].ToString()));
                        }
                        break;
                    default:
                        return BadRequest();
                }
            }
        }
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(response);
    }


    public static string GetServicesAvailable(Clinic clinic, MainContext _context, IMapper _mapper)
    {
        var servicesString = "";
        var specialServiceString = "";
        var services = _context.ClinicServices
            .Include(service => service.DaysOffered)
            .Include(service => service.Service)
            .Where(service => service.IsActive && !service.IsDeleted && service.IsAvailable);
        foreach(var service in services)
        {
            if(service.IsSpecial)
            {
                specialServiceString += $"\n{service.Service.Name} {GetOperatingHours(_mapper.Map<List<OperatingHourDto>>(service.DaysOffered))}";
            }
            else
            {
                servicesString += $"\n{service.Service.Name}";
            }
        }

        return $"*Services Available*{servicesString}\n\n*Special Services*{specialServiceString}";
    }
    public static string GetOperatingHours(List<OperatingHourDto> OperatingHours)
    {
        var opHours = "";
        foreach(var operatingHour in OperatingHours)
        {
            opHours += $"\n{operatingHour.DaysDescription} {operatingHour.Start} - {operatingHour.End}";
        }
        return opHours;
    }

    public static string GetDirectionLocationUrl(double originLatitude, double originLongitude, double latitude, double longitude)
    {
        var url = new UriBuilder("https://www.google.com/maps/dir/")
        {
            Query = $"api=1&origin={originLatitude},{originLongitude}&destination={latitude},{longitude}"
        };
        return url.ToString();
    }

    public static string GetPinLocationUrl(double latitude, double longitude)
    {
        var url = new UriBuilder("https://www.google.com/maps/search/")
        {
            Query = $"api=1&query={latitude},{longitude}"
        };
        return url.ToString();
    }
    public static Conversation CreateConversation(Client client, JToken data, MessageSetup message, string response, string serviceId = null,ServiceCategory category = ServiceCategory.Intro)
    {
        if(message?.Parent?.Key == Key.Begin)
        {
            switch(data["content"].ToString())
            {
                case "1":
                    category = ServiceCategory.ClinicLocation;
                    break;
                case "2":
                    category = ServiceCategory.Appointment;
                    break;
                case "3":
                    category = ServiceCategory.Feedback;
                    break;
                default:
                    break;
            }
        }
        var convo = new Conversation
        {
            MetaData = JToken.FromObject(data),
            client = client,
            MessageSetup = message,
            Input = message.Key == Key.NearestClinic ? "Shared Current Location Refer metadata" : data["content"].ToString(),
            Response = response,
            CreatedBy = "System",
            category = category,
            serviceId = serviceId
        };
        
        var httpResponse = Api.SendMessage(client.PhoneNumber,response).Result;
        convo.sent = httpResponse.IsSuccessStatusCode;
        convo.log = JToken.FromObject(httpResponse);

        return convo;
    }

}