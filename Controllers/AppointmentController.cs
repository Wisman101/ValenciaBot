using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ValenciaBot.Data;
using ValenciaBot.Data.Entities;

namespace ValenciaBot.Controllers.Appointments;

[ApiController]
[Route("api/[Controller]")]
public class AppointmentController : ControllerBase
{
    private readonly MainContext _context;
    private readonly IMapper _mapper;
    public AppointmentController(MainContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<AppointmentDto>>> GetAppointment()
    {
        var appointment = await _context.Appointments
                .Include(appointment => appointment.client)
                .Include(appointment => appointment.service)
                .Where(service => !service.IsDeleted)
                .ToListAsync();
        return _mapper.Map<List<AppointmentDto>>(appointment);
    }

    // GET: api/Appointment/5
    [HttpGet("{id}")]
    public async Task<ActionResult<AppointmentDto>> GetAppointment(int id)
    {
        var appointment = await _context.Appointments
                .Include(appointment => appointment.client)
                .Include(appointment => appointment.service)
                .FirstOrDefaultAsync(service => service.Id == id && !service.IsDeleted);

        if (appointment == null) return NotFound();
        
        return _mapper.Map<AppointmentDto>(appointment);
    }
   
    // PUT: api/Appointment/5/confirm
    [HttpPut("{id}/confirm")]
    public async Task<IActionResult> ConfirmAppointment(int id, AppointmentDtoPost AppointmentDto)
    {
        Appointment appointment = await _context.Appointments.FindAsync(id);
        if(appointment is null) return NotFound("Appointment Not Found");

       appointment.confirmed = AppointmentDto.confirmed;
       appointment.ConfirmedDate = AppointmentDto.ConfirmedDate;
       appointment.ConfirmedTime = AppointmentDto.ConfirmedTime;
       appointment.Remarks = AppointmentDto.Remarks;
      // appointment.ConfirmedBy = "system"; TODO: get from header - httpContext

        _context.Entry(appointment).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AppointmentExists(id))
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

    // DELETE: api/Appointment/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAppointment(int id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null)
        {
            return NotFound();
        }

        appointment.IsDeleted = true;
        _context.Appointments.Update(appointment);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool AppointmentExists(int id)
    {
        return _context.Appointments.Any(e => e.Id == id);
    }
}