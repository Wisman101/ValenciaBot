using AutoMapper;
using ValenciaBot.Data.Dto;
using ValenciaBot.Data.Entities;
using static ChatFunctions;

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

        CreateMap<Appointment, AppointmentDto>();
        CreateMap<AppointmentDto, Appointment>();

        CreateMap<ClinicService, ClinicServiceDtoPost>();
        CreateMap<ClinicServiceDtoPost, ClinicService>();

        CreateMap<MessageSetup, MessageSetupDto>()
            .ForMember(messageDto => messageDto.ParentId, message => message.MapFrom(m => m.Parent.Id));
        CreateMap<MessageSetupDto, MessageSetup>();

        CreateMap<Clinic, Model>()
            .ForMember(m => m.Clinic, c => c.MapFrom(c => c));
    }
}
