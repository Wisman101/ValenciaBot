using System.Collections.Generic;
using AutoMapper;
using Geolocation;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using ValenciaBot.Data;
using ValenciaBot.Data.Dto;
using ValenciaBot.Data.Entities;
using ValenciaBot.Data.Enum;
using ValenciaBot.HelperFunctions.Clickatell;

class ChatFunctions
{
    public class Model
    {
        public double distance { get; set; }
        public ClinicDto Clinic { get; set; }
    }
    public static List<Model> NearestClinics(double latitude, double longitude, MainContext _context, IMapper _mapper)
    {
        Coordinate origin = new Coordinate(latitude, longitude);
        var Clinics = _context.Clinics.Where(clinic => !clinic.IsDeleted && clinic.IsActive);
        List<Model> NearestClinics = new();
        foreach(var clinic in Clinics)
        {
            var clinicLocation = new Coordinate(clinic.Latitude, clinic.Longitude);
            var distance = GeoCalculator.GetDistance(origin, clinicLocation, 4);
            if(distance <= 1)
            {
                NearestClinics.Add(new Model
                {
                    distance =distance,
                    Clinic = _mapper.Map<ClinicDto>(clinic)
                });
            }
        }
        NearestClinics.Sort((a, b) => a.distance.CompareTo(b.distance));
        NearestClinics.ForEach(model => model.distance = ConvertMilesToKMs(model.distance));
        return NearestClinics;
    }

    public static double ConvertMilesToKMs(double distance)
    {
        var km = distance*1.60934;
        return Math.Round(km, 2);
    }


    public static string GetClinicDetails(Clinic clinic, MainContext _context, IMapper _mapper)
    {
        var response = 
@$"*{clinic.Name}*
--------------------------------
*Location:* {clinic.LocationDescription}
*Pin:* {GetPinLocationUrl(clinic.Latitude,clinic.Longitude)}
*Tel:* {clinic.Tel}
*Email:* {clinic.Email}

*Operating Hours*{GetOperatingHours(_mapper.Map<List<OperatingHourDto>>(clinic.OperatingHour))}

{GetServicesAvailable(clinic, _context, _mapper)}

00: Home";
        return response;
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
    public static Conversation CreateMessage(Client client, JToken data, MessageSetup messageSetup, string response, Conversation conversation)
    {
        if(messageSetup?.Parent?.Key == Key.Begin)
        {
            switch(data["content"].ToString())
            {
                case "1":
                    conversation.category = ServiceCategory.ClinicLocation;
                    break;
                case "2":
                    conversation.category = ServiceCategory.Appointment;
                    break;
                case "3":
                    conversation.category = ServiceCategory.Feedback;
                    break;
                default:
                    break;
            }
        }
        var message = new Messages
        {
            MetaData = JToken.FromObject(data),
            MessageSetup = messageSetup,
            Input = messageSetup.Key == Key.NearestClinic ? "Shared Current Location Refer metadata" : data["content"].ToString(),
            Response = response,
            CreatedBy = "System"
        };
        
        var httpResponse = Api.SendMessage(client.PhoneNumber,response).Result;
        message.sent = httpResponse.IsSuccessStatusCode;
        message.log = JToken.FromObject(httpResponse);
        conversation.MessageSetup = messageSetup;
        conversation.LastModified = DateTime.UtcNow;
        conversation.Messages.Add(message);

        return conversation;
    }

}