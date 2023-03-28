using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ValenciaBot.Data;
using ValenciaBot.Data.Dto;
using ValenciaBot.Data.Entities;

namespace ValenciaBot.Controllers.Services;

[ApiController]
[Route("api/[Controller]")]
public class ServiceController : ControllerBase
{
    private readonly MainContext _context;
    private readonly IMapper _mapper;
    public ServiceController(MainContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<ServiceDto>>> GetServices()
    {
        return _mapper.Map<List<ServiceDto>>(await _context.Services.ToListAsync());
    }

    // GET: api/Service/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceDto>> GetService(int id)
    {
        var service = await _context.Services.FindAsync(id);

        if (service == null)
        {
            return NotFound();
        }

        return _mapper.Map<ServiceDto>(service);
    }
   
    // PUT: api/Service/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutService(int id, ServiceDto serviceDto)
    {
        Service service = new ();
        service.Code = serviceDto.Code;
        service.Name = serviceDto.Name;
        if (id != serviceDto.Id)
        {
            return BadRequest();
        }

        _context.Entry(service).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ServiceExists(id))
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

    // POST: api/Service
    [HttpPost]
    public async Task<ActionResult<ServiceDto>> PostService(ServiceDto serviceDto)
    {
        var service = _mapper.Map<Service>(serviceDto);
        _context.Services.Add(service);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetService", new { id = serviceDto.Id }, serviceDto);
    }

    // DELETE: api/Service/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteService(int id)
    {
        var Service = await _context.Services.FindAsync(id);
        if (Service == null)
        {
            return NotFound();
        }

        Service.isDeleted = true;
        _context.Services.Update(Service);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ServiceExists(int id)
    {
        return _context.Services.Any(e => e.Id == id);
    }
}