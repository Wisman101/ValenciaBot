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

namespace ValenciaBot.Controllers.Chatbot;

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
        if(contentType == null) return Ok("Invalid reply object sent");

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
            .Include(conversation => conversation.MessageSetup)
            .FirstOrDefaultAsync(conversation => conversation.status == Status.Active && conversation.client == client);

        var response = "";

        if(conversation is null || conversation.LastModified < DateTime.UtcNow.AddDays(-1) || conversation.LastModified < DateTime.UtcNow.AddMinutes(-10))
        {
            if(conversation is not null)
            {
                conversation.status = Status.TimedOut;
                _context.conversations.Update(conversation);
                await _context.SaveChangesAsync(cancellationToken);
            }

            var newConversation = new Conversation
            {
                client = client,
                category = ServiceCategory.Intro,
                status = Status.Active
            };

            if(conversation is not null && conversation.LastModified < DateTime.UtcNow.AddMinutes(-10) && conversation.LastModified > DateTime.UtcNow.AddDays(-1))
            {
                var message = _context.MessageSetups.FirstOrDefault(message => message.Key == Key.Begin);
                response = "*Welcome Back!*\n\n" + message.Response;
                _context.conversations.Update(CreateMessage(client, requestContent, message, response, newConversation));
                await _context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                var message = await _context.MessageSetups.FirstOrDefaultAsync(message => message.Key == Key.Intro && !message.IsDeleted);
                _context.conversations.Update(CreateMessage(client, requestContent, message, $"Hey {client.Name},\n {message.Response}", newConversation));
                await _context.SaveChangesAsync(cancellationToken);

                var message2 = await _context.MessageSetups.FirstOrDefaultAsync(message => message.Key == Key.Begin && !message.IsDeleted);
                response = message2.Response;
                _context.conversations.Update(CreateMessage(client, requestContent, message2, response, newConversation));
                await _context.SaveChangesAsync(cancellationToken);
            }
            
            return Ok(response);
        }
    
        if(request["event"]["moText"] == null && conversation.MessageSetup.Key != Key.CurrentLocation)
        {
            response = InvalidInput(requestContent, client, conversation);
            return Ok(response);
        }
        
        if(requestContent["content"]?.ToString() == "00")
        {
            var message = await _context.MessageSetups.FirstOrDefaultAsync(message => message.Key == Key.Begin && !message.IsDeleted);
            _context.conversations.Update(CreateMessage(client, requestContent, message, message.Response, conversation));
            await _context.SaveChangesAsync(cancellationToken);
            return Ok(message.Response);
        }
       
        MessageSetup messageSetup = conversation.MessageSetup;
        
        if(!messageSetup.IsDynamic)
        {
            var message = await _context.MessageSetups
                .FirstOrDefaultAsync(setup => setup.Parent == messageSetup 
                    && setup.Input == requestContent["content"].ToString());
            
            if(message is null)
            {
                response = InvalidInput(requestContent, client, conversation);
                return Ok(response);
            }

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
                    if(requestContent["latitute"] is null || requestContent["longitude"] is null)
                    {
                        response = "Invalid Input! Kindly respond with your current pin location\n\n00. Home";
                        response = InvalidInput(requestContent, client, conversation, response);
                        return Ok(response);
                    }

                    double latitude = requestContent["latitute"].Value<double>();
                    double longitude = requestContent["longitude"].Value<double>();
                    List<Model> clinics = ChatFunctions.NearestClinics(latitude, longitude, _context, _mapper);
                    // if(conversation.category == ServiceCategory.Appointment)
                    // {
                    //     clinics = clinics.Where(clinic => clinic.Clinic.se)
                    // }
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
                        int serviceId;
                        bool result = int.TryParse(requestContent["content"].ToString(), out serviceId);
                        var service = result ? await _context.Services.FindAsync(serviceId) : null;
                        if(service is null)
                        {
                            response = "Invalid Input! Kindly respond with a service number in the service list above\n\n00. Home";
                            response = InvalidInput(requestContent, client, conversation, response);
                            return Ok(response);
                        }

                        conversation.TransitData = JToken.FromObject(new
                        {
                            serviceId = requestContent["content"].ToString()
                        });
                        var AppointmentClinicMessage = await _context.MessageSetups.FirstOrDefaultAsync(setup => setup.Key == Key.SearchByLocation);
                        response = $"Kindly proceed to search for a clinic to schedule the appointment\n\n{AppointmentClinicMessage.Response}";
                        _context.conversations.Update(CreateMessage(client, requestContent, AppointmentClinicMessage, response, conversation));
                    }
                    break;
                    case Key.CountyName:
                    var countyClinics = _context.Clinics
                        .Include(clinic => clinic.OperatingHour)
                        .Where(clinic => !clinic.IsDeleted && clinic.IsActive 
                            && EF.Functions.Like(clinic.County.ToLower(), $"%{requestContent["content"].ToString().ToLower()}%"))
                        .ToList();

                    if(countyClinics.Count() == 0)
                    {
                        response = "No clinic found in the given county, Try another county name\n\n00. Home";
                        response = InvalidInput(requestContent, client, conversation, response);
                        return Ok(response);
                    }

                    if(countyClinics.Count() == 1)
                    {
                        var ClinicDetailsMessage = await _context.MessageSetups.FirstOrDefaultAsync(setup => setup.Key == Key.ClinicDetails);
                        response = $"Clinic Located in the county matching to *{requestContent["content"].ToString()}* is:\n\n{GetClinicDetails(countyClinics.FirstOrDefault(), _context, _mapper)}";
                        _context.conversations.Update(CreateMessage(client, requestContent, ClinicDetailsMessage, response, conversation));                            

                        if(conversation.category == ServiceCategory.Appointment)
                        {
                            var appointmentDateMessage = _context.MessageSetups.FirstOrDefault(setup => setup.Key == Key.AppointmentDate);
                            response = appointmentDateMessage.Response;
                            _context.conversations.Update(CreateMessage(client, requestContent, appointmentDateMessage, response, conversation));
                        }
                        else
                        {
                            conversation.status = Status.Complete;
                            _context.conversations.Update(conversation);
                        }
                    }
                    else
                    {
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
                    }
                    break;
                case Key.ClinicName:
                    var clinicsx =  _context.Clinics
                        .Include(clinic => clinic.OperatingHour)
                        .Where(clinic => !clinic.IsDeleted && clinic.IsActive && 
                            (clinic.Code == requestContent["content"].ToString() || EF.Functions.Like(clinic.Name.ToLower(), $"%{requestContent["content"].ToString().ToLower()}%")))
                    .ToList();

                    if(clinicsx is null)
                    {
                        response = "Clinic with the given name or code not found! try another name or code\n\n00. Home";
                        response = InvalidInput(requestContent, client, conversation, response);
                        return Ok(response);
                    }

                    if(clinicsx.Count() == 1)
                    {
                        var ClinicDetailsMessage = await _context.MessageSetups.FirstOrDefaultAsync(setup => setup.Key == Key.ClinicDetails);
                        response = GetClinicDetails(clinicsx.FirstOrDefault(), _context, _mapper);
                        _context.conversations.Update(CreateMessage(client, requestContent, ClinicDetailsMessage, response, conversation));                            

                        if(conversation.category == ServiceCategory.Appointment)
                        {
                            var appointmentDateMessage = _context.MessageSetups.FirstOrDefault(setup => setup.Key == Key.AppointmentDate);
                            response = appointmentDateMessage.Response;
                            _context.conversations.Update(CreateMessage(client, requestContent, appointmentDateMessage, response, conversation));
                        }
                        else
                        {
                            conversation.status = Status.Complete;
                            _context.conversations.Update(conversation);
                        }
                    }
                    else
                    {
                        var clinicModels = _mapper.Map<List<Model>>(clinicsx);
                        var clinicNameMessage = _context.MessageSetups.FirstOrDefault(setup => setup.Key == Key.ClinicList);
                        response = $"{clinicNameMessage.Response} *{requestContent["content"].ToString()}* are:\n";
                        var count = 1;
                        foreach(var clinic in clinicModels)
                        {
                            response += $"{count}. {clinic.Clinic.Code} - {clinic.Clinic.Name} ({clinic.distance}Km)\n*Pin:* {GetPinLocationUrl(clinic.Clinic.Latitude, clinic.Clinic.Longitude)}\n";
                            count ++;
                        }

                        _context.conversations.Update(CreateMessage(client, requestContent, clinicNameMessage, response, conversation));

                        if(conversation.category == ServiceCategory.Appointment)
                        {
                            response = "Kindly select the clinic you would wish to book an appointment from.\n\n Reply with the clinic code e.g 'EQA001'\n\n00: Home";
                        }
                        else
                        {
                            response = "To Get Clinic Details, Kindly Reply with the clinic code e.g 'EQA001'\n\n00: Home";
                        }
                    
                        _context.conversations.Update(CreateMessage(client, requestContent, clinicNameMessage, response, conversation));
                    }
                    break;
                default:
                    return BadRequest();
            }
        }
        
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(response);
    }

}
