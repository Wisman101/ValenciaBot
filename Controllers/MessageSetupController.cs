using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ValenciaBot.Data;
using ValenciaBot.Data.Dto;
using ValenciaBot.Data.Entities;

namespace ValenciaBot.Controllers.MessageSetups;

[ApiController] 
[Route("api/[Controller]")]
public class MessageSetupController : ControllerBase
{
    private readonly MainContext _context;
    private readonly IMapper _mapper;
    public MessageSetupController(MainContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<MessageSetupDto>>> GetMessageSetups()
    {
        return _mapper.Map<List<MessageSetupDto>>(await _context.MessageSetups.Where(MessageSetup => !MessageSetup.isDeleted).ToListAsync());
    }

    // GET: api/MessageSetup/5
    [HttpGet("{id}")]
    public async Task<ActionResult<MessageSetupDto>> GetMessageSetup(int id)
    {
        var MessageSetup = await _context.MessageSetups.FindAsync(id);

        if (MessageSetup == null)
        {
            return NotFound();
        }

        return _mapper.Map<MessageSetupDto>(MessageSetup);
    }
   
    // PUT: api/MessageSetup/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutMessageSetup(int id, MessageSetupDto MessageSetupDto)
    {
        MessageSetup MessageSetup = await _context.MessageSetups.FindAsync(id);
        if(MessageSetup is null) return NotFound();
        
        MessageSetup.Input = MessageSetupDto.Input;
        MessageSetup.Response = MessageSetupDto.Response;
        MessageSetup.isDynamic = MessageSetupDto.isDynamic;
        MessageSetup.isActive = MessageSetupDto.isActive;
        MessageSetup.key = MessageSetupDto.key;
        MessageSetup.Parent = _context.MessageSetups.Find(MessageSetupDto.ParentId);

        _context.Entry(MessageSetup).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!MessageSetupExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return Ok();
    }

    // POST: api/MessageSetup
    [HttpPost]
    public async Task<ActionResult<MessageSetupDto>> PostMessageSetup(MessageSetupDto MessageSetupDto)
    {
        var MessageSetup = new MessageSetup();
        MessageSetup.Input = MessageSetupDto.Input;
        MessageSetup.Response = MessageSetupDto.Response;
        MessageSetup.isDynamic = MessageSetupDto.isDynamic;
        MessageSetup.isActive = MessageSetupDto.isActive;
        MessageSetup.key = MessageSetupDto.key;
        MessageSetup.Parent = _context.MessageSetups.Find(MessageSetupDto.ParentId);
        _context.MessageSetups.Add(MessageSetup);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetMessageSetup", new { id = MessageSetupDto.Id }, MessageSetupDto);
    }

    // DELETE: api/MessageSetup/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMessageSetup(int id)
    {
        var MessageSetup = await _context.MessageSetups.FindAsync(id);
        if (MessageSetup == null)
        {
            return NotFound();
        }

        MessageSetup.isDeleted = true;
        _context.MessageSetups.Update(MessageSetup);
        await _context.SaveChangesAsync();

        return Ok();
    }

    private bool MessageSetupExists(int id)
    {
        return _context.MessageSetups.Any(e => e.Id == id);
    }
}