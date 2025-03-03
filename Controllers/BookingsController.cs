using Microsoft.AspNetCore.Mvc;
using TennisCourtAPI.Models;
using Microsoft.EntityFrameworkCore;
using TennisCourtAPI.Data;    

namespace TennisCourtBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly  ApplicationDbContext _context;

        public BookingsController( ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/bookings
        [HttpPost]
        public async Task<IActionResult> BookCourt([FromBody] Booking request)
        {   
            // Step 1: Validate the input
            if (request.Date < DateTime.Now.Date || request.Date > DateTime.Now.AddDays(7).Date)
                return BadRequest("Booking date must be within the next 7 days.");
            if(string.IsNullOrEmpty(request.TimeSlot))
                return BadRequest("Time Slot needs to be mentioned.");
            if (!IsValidTimeSlot(request.TimeSlot))
                return BadRequest("Time slot must be between 6 AM and 10 PM in hourly slots.");

            // Step 2: Check availability
            var court = await _context.Courts.FindAsync(request.CourtId);
            if (court == null)
                return NotFound("Court not found.");
                
            var isSlotBooked = true;
            if (_context.Bookings == null) {
                isSlotBooked = false;
            } else {
                isSlotBooked = _context.Bookings
                .Any(b => b.CourtId == court.CourtId && b.Date == request.Date && b.TimeSlot == request.TimeSlot);
            }
            if (isSlotBooked)
                return BadRequest("The selected time slot is already booked.");

            var bookingId = $"{request.Date:yyyyMMdd}-{request.TimeSlot.Replace(" ", "").Replace(":", "")}-Court{request.CourtId}";

            // Step 3: Create the booking
            var booking = new Booking
            {
                BookingId = bookingId,
                Name = request.Name,
                PhoneNumber = request.PhoneNumber,
                Date = request.Date,
                TimeSlot = request.TimeSlot,
                CourtId = court.CourtId
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return Ok("Booking successful!, BookingID : " + booking.BookingId);
        }

        // Helper method to validate time slot
        private bool IsValidTimeSlot(string timeSlot)
        {
            var validTimeSlots = new[] { "6-7 AM", "7-8 AM", "8-9 AM", "9-10 AM", "10-11 AM", "11-12 PM", 
                                         "12-1 PM", "1-2 PM", "2-3 PM", "3-4 PM", "4-5 PM", "5-6 PM", 
                                         "6-7 PM", "7-8 PM", "8-9 PM", "9-10 PM" };

            return validTimeSlots.Contains(timeSlot);
        }

        [HttpDelete("cancel")]
        public async Task<IActionResult> CancelBooking([FromBody] Cancel cancel)
        {   
            if(_context.Bookings == null) {
                return BadRequest("Booking not found");
            }
            // Step 1: Validate the input
            if (string.IsNullOrEmpty(cancel.Name) || 
                string.IsNullOrEmpty(cancel.PhoneNumber) || 
                string.IsNullOrEmpty(cancel.BookingId) || cancel.CourtId == 0)
            {
                return BadRequest("All fields (Name, PhoneNumber, BookingId, CourtId) must be provided.");
            }
            
            // Step 2: Check if the booking exists and match the cancellation details
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingId == cancel.BookingId &&
                                          b.Name == cancel.Name &&
                                          b.PhoneNumber == cancel.PhoneNumber);

            if (booking == null)
                return NotFound("Booking not found or details do not match.");

            // Step 3: Cancel the booking (Delete it from the database)
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return Ok("Booking canceled successfully!");
        }
    }
}
