using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using ValenciaBot.Data;
using ValenciaBot.Data.Dto;
using ValenciaBot.Data.Entities;

namespace ValenciaBot.Controllers.ClinicServices;

[ApiController]
[Route("api/[Controller]")]
public class ClinicServiceController : ControllerBase
{
    private readonly MainContext _context;
    private readonly IMapper _mapper;
    public ClinicServiceController(MainContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<ClinicServiceDto>>> GetClinicService()
    {
        var clinicService = await _context.ClinicServices
                .Include(service => service.DaysOffered)
                .Include(service => service.Clinic)
                .Include(service => service.Service)
                .Where(service => !service.IsDeleted)
                .ToListAsync();
        return _mapper.Map<List<ClinicServiceDto>>(clinicService);
    }

    // GET: api/ClinicService/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ClinicServiceDto>> GetClinicService(int id)
    {
        var clinicService = await _context.ClinicServices
                .Include(service => service.DaysOffered)
                .Include(service => service.Clinic)
                .Include(service => service.Service)
                .FirstOrDefaultAsync(service => service.Id == id && !service.IsDeleted);

        if (clinicService == null) return NotFound();
        
        return _mapper.Map<ClinicServiceDto>(clinicService);
    }
   
    // PUT: api/ClinicServiceService/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutClinicService(int id, ClinicServiceDtoPost ClinicServiceDto)
    {
        ClinicService clinicService = await _context.ClinicServices.FindAsync(id);
        if(clinicService is null) return NotFound("Clinic Service Not Found");

        Clinic clinic = await _context.Clinics
            .FirstOrDefaultAsync(clinic => clinic.Id == ClinicServiceDto.ClinicId && !clinic.IsDeleted);
        if(clinic is null) return NotFound("Clinic Not Found");

        Service service = await _context.Services
            .FirstOrDefaultAsync(service => service.Id == ClinicServiceDto.ServiceId && !service.IsDeleted);
        if(service is null) return NotFound("Service Not Found");

        clinicService.Clinic = clinic;
        clinicService.Service = service;
        clinicService.IsAvailable = ClinicServiceDto.IsAvailable;
        clinicService.IsSpecial = ClinicServiceDto.IsSpecial;
        clinicService.DaysOffered = _mapper.Map<List<ServiceOperatingHour>>(ClinicServiceDto.DaysOffered);

        _context.Entry(clinicService).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ClinicServiceExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/ClinicService
    [HttpPost]
    public async Task<ActionResult<ClinicServiceDto>> PostClinicService(ClinicServiceDtoPost ClinicServiceDto)
    {
        Clinic clinic = await _context.Clinics
            .FirstOrDefaultAsync(clinic => clinic.Id == ClinicServiceDto.ClinicId && !clinic.IsDeleted);
        if(clinic is null) return NotFound("Clinic Not Found");

        Service service = await _context.Services
            .FirstOrDefaultAsync(service => service.Id == ClinicServiceDto.ServiceId && !service.IsDeleted);
        if(service is null) return NotFound("Service Not Found");

        ClinicService clinicService = new ClinicService();

        clinicService.Clinic = clinic;
        clinicService.Service = service;
        clinicService.IsAvailable = ClinicServiceDto.IsAvailable;
        clinicService.IsSpecial = ClinicServiceDto.IsSpecial;

        clinicService.DaysOffered = new();

        foreach(var OperatingHour in ClinicServiceDto.DaysOffered)
        {
            clinicService.DaysOffered.Add(new ServiceOperatingHour
            {
                Days = JToken.FromObject(OperatingHour.Days),
                Start = OperatingHour.Start,
                End = OperatingHour.End
            });
        }

        await _context.ClinicServices.AddAsync(clinicService);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetClinicService", new { id = ClinicServiceDto.Id }, ClinicServiceDto);
    }

    // DELETE: api/ClinicService/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClinicService(int id)
    {
        var ClinicService = await _context.ClinicServices.FindAsync(id);
        if (ClinicService == null)
        {
            return NotFound();
        }

        ClinicService.IsDeleted = true;
        _context.ClinicServices.Update(ClinicService);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ClinicServiceExists(int id)
    {
        return _context.ClinicServices.Any(e => e.Id == id);
    }
}