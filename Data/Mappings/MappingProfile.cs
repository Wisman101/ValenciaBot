using AutoMapper;
using ValenciaBot.Data.Dto;
using ValenciaBot.Data.Entities;

namespace ValenciaBot.Data.Mappings;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Service, ServiceDto>();

        CreateMap<Clinic, ClinicDto>();

        CreateMap<ClinicOperatingHour, OperatingHourDto>();

        CreateMap<ClinicService, ClinicServiceDto>();
    }
}
