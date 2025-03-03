namespace TennisCourtAPI.Models
{
    public class Court
    {
        public int? CourtId { get; set; } // Unique identifier for the court
        public string? CourtName { get; set; } // Name or number of the court

        public string? Address { get; set; } // Address of the court

        public string? ImageUrl { get; set; } // Optional image URL for the court

        public ICollection<Booking>? Bookings { get; set; } // List of bookings for this court
    }
}

