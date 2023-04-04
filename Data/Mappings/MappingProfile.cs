using AutoMapper;
using ValenciaBot.Data.Dto;
using ValenciaBot.Data.Entities;

namespace ValenciaBot.Data.Mappings;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Service, ServiceDto>();
        CreateMap<ServiceDto, Service>();

        CreateMap<Clinic, ClinicDto>();
        CreateMap<ClinicDto, Clinic>();

        CreateMap<ClinicOperatingHour, OperatingHourDto>();
        CreateMap<OperatingHourDto, ClinicOperatingHour>();

        CreateMap<ServiceOperatingHour, OperatingHourDto>();
        CreateMap<OperatingHourDto, ServiceOperatingHour>();

        CreateMap<ClinicService, ClinicServiceDto>();
        CreateMap<ClinicServiceDto, ClinicService>();
    }
}
