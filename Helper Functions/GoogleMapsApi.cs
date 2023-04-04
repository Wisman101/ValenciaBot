// using AutoMapper;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using Newtonsoft.Json.Linq;
// using ValenciaBot.Data;
// using ValenciaBot.Data.Dto;
// using ValenciaBot.Data.Entities;

// namespace ValenciaBot.Controllers.Clinics;

// [ApiController]
// [Route("api/[Controller]")]
// public class LocationController : ControllerBase
// {
//     private readonly MainContext _context;
//     private readonly IMapper _mapper;
//     public LocationController(MainContext context, IMapper mapper)
//     {
//         _context = context;
//         _mapper = mapper;
//     }

//     [HttpGet]
//     public async Task<ActionResult<List<ClinicServiceDto>>> GetClinicServiceServices()
//     {
//         var placesService = new PlacesService(apiKey);
//         var request = new PlacesSearchRequest
//         {
//             Location = new Location(latitude, longitude),
//             Radius = 5000, // search within 5 kilometers
//             Keyword = "Equity Afia"
//         };

//         var response = placesService.Search(request);
//         var facilities = response.Results;

//         return _mapper.Map<List<ClinicServiceDto>>(await _context.ClinicServices.ToListAsync());
//     }
// }