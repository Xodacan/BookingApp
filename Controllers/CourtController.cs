using Microsoft.AspNetCore.Mvc;
using TennisCourtAPI.Models;
using Microsoft.EntityFrameworkCore;
using TennisCourtAPI.Data;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Runtime.CompilerServices;

namespace TennisCourtBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourtController : ControllerBase
    {
        private readonly  ApplicationDbContext _context;

        public CourtController( ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/bookings
        [HttpPost]
        public async Task<IActionResult> AddCourt([FromBody] Court newCourt)
        {   
            if(newCourt.Address == null || newCourt.CourtName == null || newCourt.CourtName == null)
                return BadRequest("All fields (Name, ID, Address) must be provided. Image is Optional");
            
            var isCourtBooked = true;
            if (_context.Courts == null) {
                isCourtBooked = false;
            } else {
                isCourtBooked = _context.Courts
                .Any(b => b.Address == newCourt.Address && b.CourtName == newCourt.CourtName);
            }
            if (isCourtBooked)
                return BadRequest("This court already exists. Court cannot have same name and address.");

            Random rand = new Random();
            if(_context.Courts == null){
                newCourt.CourtId = 1;
            } else {
                while (_context.Courts.Any(c => c.CourtId == newCourt.CourtId))
            {
                newCourt.CourtId = rand.Next(1, int.MaxValue);  
            }
            }
            _context.Courts.Add(newCourt);
            await _context.SaveChangesAsync();
            return Ok("Court Added Successfully!");
        }

        // GET: api/court
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Court>>> GetCourts()
        {
            if(_context.Courts == null)
            {
                return Ok("No courts Present");
            }
            var courts = await _context.Courts
                .Include(c => c.Bookings) // Include bookings for each court
                .ToListAsync();

            return Ok(courts);
        }

    }

}
