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
using System.Linq;
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

        var conversation = await _context.conversations
            .Include(convo => convo.MessageSetup)
            .FirstOrDefaultAsync(convo => convo.status == Status.Active);
    
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
                _context.conversations.Update(CreateMessage(client, requestContent, message, message.Response, conversation));
                await _context.SaveChangesAsync(cancellationToken);
                return NoContent();
            }
        }
       
        
        var response = "";
        if(conversation is null || conversation.LastModified < DateTime.UtcNow.AddDays(-1))
        {
            if(conversation is not null)
            {
                conversation.status = Status.TimedOut;
                _context.conversations.Update(conversation);
            }
           
            var newConversation = new Conversation
            {
                client = client,
                category = ServiceCategory.Intro,
                status = Status.Active
            };
            
            var message = await _context.MessageSetups.FirstOrDefaultAsync(message => message.Key == Key.Intro && !message.IsDeleted);
            await _context.conversations.AddAsync(CreateMessage(client, requestContent, message, $"Hey {client.Name},\n {message.Response}", newConversation));

            var message2 = await _context.MessageSetups.FirstOrDefaultAsync(message => message.Key == Key.Begin && !message.IsDeleted);
            await _context.conversations.AddAsync(CreateMessage(client, requestContent, message2, message2.Response, newConversation));
        }
        else if(conversation.LastModified < DateTime.UtcNow.AddMinutes(-10))
        {
             conversation.status = Status.TimedOut;
            _context.conversations.Update(conversation);

            var newConversation = new Conversation
            {
                client = client,
                category = ServiceCategory.Intro,
                status = Status.Active
            };
            var message = _context.MessageSetups.FirstOrDefault(message => message.Key == Key.Begin);
            response = "*Welcome Back!*\n\n" + message.Response;
            await _context.conversations.AddAsync(CreateMessage(client, requestContent, message, response, newConversation));
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
                    _context.conversations.Update(CreateMessage(client, requestContent, message, response, conversation));
                }
                else
                {
                    switch(message.Key)
                    {
                        case Key.CurrentLocation:
                            _context.conversations.Update(CreateMessage(client, requestContent, message, response, conversation));
                            break;
                        case Key.Services:
                            var services = _context.Services.ToList();
                            response = $"{message.Response}";
                            foreach(var service in services)
                            {
                                response += $"\n{service.Id}: {service.Name}";
                            }
                            response += "\n\n00: Home";
                            _context.conversations.Update(CreateMessage(client, requestContent, message, response, conversation));
                            break;
                        case Key.CountyName:                        
                            _context.conversations.Update(CreateMessage(client, requestContent, message, response, conversation));
                            break;
                        case Key.ClinicName:
                            _context.conversations.Update(CreateMessage(client, requestContent, message, response, conversation));
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
                        List<Model> clinics = ChatFunctions.NearestClinics(latitude, longitude, _context, _mapper);
                        if(clinics.Count() == 1)
                        {
                            var ClinicDetailsMessage = await _context.MessageSetups
                                .Include(setup => setup.Parent)
                                .FirstOrDefaultAsync(setup => setup.Key == Key.ClinicDetails);
                            var c = await _context.Clinics
                                .Include(clinic => clinic.OperatingHour)
                                .FirstOrDefaultAsync(clinic => clinic.Code == clinics.First().Clinic.Code);
                            response = GetClinicDetails(c, _context, _mapper);
                            _context.conversations.Update(CreateMessage(client, requestContent, ClinicDetailsMessage, response, conversation));                            

                            if(conversation.category == ServiceCategory.Appointment)
                            {
                                var appointmentDateMessage = _context.MessageSetups.FirstOrDefault(setup => setup.Key == Key.AppointmentDate);
                                var res = appointmentDateMessage.Response;
                                _context.conversations.Update(CreateMessage(client, requestContent, appointmentDateMessage, res, conversation));
                            }
                            else
                            {
                                conversation.status = Status.Complete;
                                _context.conversations.Update(conversation);
                            }
                        }
                        else
                        {
                            var NearestClinicMessage = _context.MessageSetups.FirstOrDefault(setup => setup.Key == Key.NearestClinic);
                            response = $"{NearestClinicMessage.Response}";
                            var countx = 1;
                            foreach(var clinic in clinics)
                            {
                                response += $"{countx}. {clinic.Clinic.Code} - {clinic.Clinic.Name} ({clinic.distance}Km)\n*Direction:* {GetDirectionLocationUrl(latitude, longitude, clinic.Clinic.Latitude, clinic.Clinic.Longitude)}\n";
                                countx ++;
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
                            
                            _context.conversations.Update(CreateMessage(client, requestContent, NearestClinicMessage, response, conversation));
                            _context.conversations.Update(CreateMessage(client, requestContent, NearestClinicMessage, response1, conversation));
                        }
                        break;
                    case Key.NearestClinic:
                    case Key.CountyClinics:
                    case Key.ClinicList:
                        var EQAclinic = await  _context.Clinics
                            .Include(clinic => clinic.OperatingHour)
                            .FirstOrDefaultAsync(clinic => clinic.Code == requestContent["content"].ToString());
                        if(EQAclinic is null)
                        {
                            response = $"Clinic with code {requestContent["content"].ToString()} Not found. Kindly reply with correct clinic code\n\n00: Home";
                            _context.conversations.Update(CreateMessage(client, requestContent, messageSetup, response, conversation));
                        }
                        else
                        {
                            var ClinicDetailsMessage = await _context.MessageSetups.FirstOrDefaultAsync(setup => setup.Key == Key.ClinicDetails);
                            response = GetClinicDetails(EQAclinic, _context, _mapper);

                            _context.conversations.Update(CreateMessage(client, requestContent, ClinicDetailsMessage, response, conversation));                            

                            string response1;
                            if(conversation.category == ServiceCategory.Appointment)
                            {
                                var appointmentDateMessage = _context.MessageSetups.FirstOrDefault(setup => setup.Key == Key.AppointmentDate);
                                response1 = appointmentDateMessage.Response;
                                _context.conversations.Update(CreateMessage(client, requestContent, appointmentDateMessage, response1, conversation));
                            }
                            else
                            {
                                conversation.status = Status.Complete;
                                _context.conversations.Update(conversation);
                            }
                        }
                        break;
                    case Key.Services:
                        if(conversation.category == ServiceCategory.Appointment)
                        {
                            conversation.TransitData = JToken.FromObject(new
                            {
                                serviceId = requestContent["content"]
                            });
                            var AppointmentClinicMessage = await _context.MessageSetups.FirstOrDefaultAsync(setup => setup.Key == Key.SearchByLocation);
                            response = $"Kindly proceed to search for a clinic to schedule the appointment\n\n{AppointmentClinicMessage.Response}";
                            _context.conversations.Update(CreateMessage(client, requestContent, AppointmentClinicMessage, response, conversation));
                        }
                        break;
                     case Key.CountyName:
                        var countyClinics = _context.Clinics
                            .Where(clinic => !clinic.IsDeleted && clinic.IsActive 
                                && EF.Functions.Like(clinic.County.ToLower(), $"%{requestContent["content"].ToString().ToLower()}%"))
                            .ToList();
                        
                        var clinicModels = _mapper.Map<List<Model>>(countyClinics);
                        
                        var countyClinicMessage = _context.MessageSetups.FirstOrDefault(setup => setup.Key == Key.CountyClinics);
                        response = $"{countyClinicMessage.Response} matching to *{requestContent["content"].ToString()}* are:\n";
                        var count = 1;
                        foreach(var clinic in clinicModels)
                        {
                            response += $"{count}. {clinic.Clinic.Code} - {clinic.Clinic.Name} *Pin:* {GetPinLocationUrl(clinic.Clinic.Latitude, clinic.Clinic.Longitude)}\n";
                            count ++;
                        }
                        
                        string response2;
                        _context.conversations.Update(CreateMessage(client, requestContent, countyClinicMessage, response, conversation));
                        if(conversation.category == ServiceCategory.Appointment)
                        {
                            response2 = "Kindly select the clinic you would wish to book an appointment from.\n\n Reply with the clinic code e.g 'EQA001'\n\n00: Home";
                        }
                        else
                        {
                            response2 = "To Get Clinic Details, Kindly Reply with the clinic code e.g 'EQA001'\n\n00: Home";
                        }
                    
                        _context.conversations.Update(CreateMessage(client, requestContent, countyClinicMessage, response2, conversation));
                        break;
                    case Key.ClinicName:
                        var clinicsx =  _context.Clinics.Where(clinic => !clinic.IsDeleted && clinic.IsActive && 
                            (clinic.Code == requestContent["content"].ToString() || EF.Functions.Like(clinic.Name.ToLower(), $"%{requestContent["content"].ToString().ToLower()}%")))
                        .ToList();
                        
                        clinicModels = _mapper.Map<List<Model>>(clinicsx);

                        if(clinicsx.Count() == 1)
                        {
                            var ClinicDetailsMessage = await _context.MessageSetups.FirstOrDefaultAsync(setup => setup.Key == Key.ClinicDetails);
                            response = GetClinicDetails(clinicsx.FirstOrDefault(), _context, _mapper);
                            _context.conversations.Update(CreateMessage(client, requestContent, ClinicDetailsMessage, response, conversation));                            

                            if(conversation.category == ServiceCategory.Appointment)
                            {
                                var appointmentDateMessage = _context.MessageSetups.FirstOrDefault(setup => setup.Key == Key.AppointmentDate);
                                response2 = appointmentDateMessage.Response;
                                _context.conversations.Update(CreateMessage(client, requestContent, appointmentDateMessage, response2, conversation));
                            }
                            else
                            {
                                conversation.status = Status.Complete;
                                _context.conversations.Update(conversation);
                            }
                        }
                        else
                        {
                            var clinicNameMessage = _context.MessageSetups.FirstOrDefault(setup => setup.Key == Key.ClinicList);
                            response = $"{clinicNameMessage.Response} *{requestContent["content"].ToString()}* are:\n";
                            count = 1;
                            foreach(var clinic in clinicModels)
                            {
                                response += $"{count}. {clinic.Clinic.Code} - {clinic.Clinic.Name} ({clinic.distance}Km)\n*Pin:* {GetPinLocationUrl(clinic.Clinic.Latitude, clinic.Clinic.Longitude)}\n";
                                count ++;
                            }

                            if(conversation.category == ServiceCategory.Appointment)
                            {
                                response2 = "Kindly select the clinic you would wish to book an appointment from.\n\n Reply with the clinic code e.g 'EQA001'\n\n00: Home";
                            }
                            else
                            {
                                response2 = "To Get Clinic Details, Kindly Reply with the clinic code e.g 'EQA001'\n\n00: Home";
                            }
                            
                            _context.conversations.Update(CreateMessage(client, requestContent, clinicNameMessage, response, conversation));
                            _context.conversations.Update(CreateMessage(client, requestContent, clinicNameMessage, response2, conversation));
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

}