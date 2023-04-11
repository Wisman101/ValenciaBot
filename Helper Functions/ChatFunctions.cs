using System.Collections.Generic;
using AutoMapper;
using Geolocation;
using ValenciaBot.Data;
using ValenciaBot.Data.Dto;
using ValenciaBot.Data.Entities;

class ChatFunctions
{
    public class Model
    {
        public double distance { get; set; }
        public ClinicDto Clinic { get; set; }
    }
    public async static Task<List<Model>> NearestClinics(double latitude, double longitude, MainContext _context, IMapper _mapper)
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
}