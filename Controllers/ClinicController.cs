using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using ValenciaBot.Data;
using ValenciaBot.Data.Dto;
using ValenciaBot.Data.Entities;

namespace ValenciaBot.Controllers.Clinics;

[ApiController]
[Route("api/[Controller]")]
public class ClinicController : ControllerBase
{
    private readonly MainContext _context;
    private readonly IMapper _mapper;
    public ClinicController(MainContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<ClinicDto>>> GetClinics()
    {
        var clinics = await _context.Clinics.Where(clinic => !clinic.isDeleted).ToListAsync();
        return _mapper.Map<List<ClinicDto>>(await _context.Clinics.Where(clinic => !clinic.isDeleted).ToListAsync());
    }

    // GET: api/Clinic/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ClinicDto>> GetClinic(int id)
    {
        var Clinic = await _context.Clinics.FindAsync(id);

        if (Clinic == null)
        {
            return NotFound();
        }

        return _mapper.Map<ClinicDto>(Clinic);
    }
   
    // PUT: api/Clinic/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutClinic(int id, ClinicDto ClinicDto)
    {
        var Clinic = _mapper.Map<Clinic>(ClinicDto);
        if (id != ClinicDto.Id)
        {
            return BadRequest();
        }

        _context.Entry(Clinic).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ClinicExists(id))
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

    // POST: api/Clinic
    [HttpPost]
    public async Task<ActionResult<ClinicDto>> PostClinic(ClinicDto ClinicDto)
    {
        Clinic Clinic = new ();
        Clinic.Code = ClinicDto.Code;
        Clinic.Name = ClinicDto.Name;
        Clinic.County = ClinicDto.County;
        Clinic.SubCounty = ClinicDto.SubCounty;
        Clinic.Ward = ClinicDto.Ward;
        Clinic.Lattitude = ClinicDto.Lattitude;
        Clinic.Longitude = ClinicDto.Longitude;
        Clinic.Tel = ClinicDto.Tel;
        Clinic.Email = ClinicDto.Email;
        Clinic.OperatingHour = new ();

        foreach(var OperatingHour in ClinicDto.OperatingHour)
        {
            Clinic.OperatingHour.Add(new ClinicOperatingHour
            {
                Days = JToken.FromObject(OperatingHour.Days),
                Start = OperatingHour.Start,
                End = OperatingHour.End
            });
        }
        _context.Clinics.Add(Clinic);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetClinic", new { id = ClinicDto.Id }, ClinicDto);
    }

    // DELETE: api/Clinic/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClinic(int id)
    {
        var Clinic = await _context.Clinics.FindAsync(id);
        if (Clinic == null)
        {
            return NotFound();
        }

        Clinic.isDeleted = true;
        _context.Clinics.Update(Clinic);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ClinicExists(int id)
    {
        return _context.Clinics.Any(e => e.Id == id);
    }
}