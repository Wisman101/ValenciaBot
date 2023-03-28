using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public async Task<ActionResult<List<ClinicServiceDto>>> GetClinicServiceServices()
    {
        return _mapper.Map<List<ClinicServiceDto>>(await _context.ClinicServices.ToListAsync());
    }

    // GET: api/ClinicService/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ClinicServiceDto>> GetClinicServiceService(int id)
    {
        var ClinicService = await _context.ClinicServices.FindAsync(id);

        if (ClinicService == null)
        {
            return NotFound();
        }

        return _mapper.Map<ClinicServiceDto>(ClinicService);
    }
   
    // PUT: api/ClinicServiceService/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutClinicServiceService(int id, ClinicServiceDto ClinicServiceDto)
    {
        var ClinicService = _mapper.Map<ClinicService>(ClinicServiceDto);
        if (id != ClinicServiceDto.Id)
        {
            return BadRequest();
        }

        _context.Entry(ClinicService).State = EntityState.Modified;

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
    public async Task<ActionResult<ClinicServiceDto>> PostClinicService(ClinicServiceDto ClinicServiceDto)
    {
        var ClinicService = _mapper.Map<ClinicService>(ClinicServiceDto);
        _context.ClinicServices.Add(ClinicService);
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

        ClinicService.isDeleted = true;
        _context.ClinicServices.Update(ClinicService);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ClinicServiceExists(int id)
    {
        return _context.ClinicServices.Any(e => e.Id == id);
    }
}