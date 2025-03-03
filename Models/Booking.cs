namespace TennisCourtAPI.Models
{
    public class Booking
    {
        public string? BookingId { get; set; } // Unique identifier for the booking
        public string? Name { get; set; } // Name of the person booking
        public string? PhoneNumber { get; set; } // Phone number of the person booking
        public DateTime Date { get; set; } // Date of the booking
        public string? TimeSlot { get; set; } // Time slot (e.g., "6-7 AM", "7-8 AM")
        
        // Foreign Key to Court
        public int? CourtId { get; set; }
        public Court? Court { get; set; } // Navigation property to Court
    }

    public class Cancel {
        public string? BookingId { get; set; } // Unique identifier for the booking
        public string? Name { get; set; } // Name of the person booking
        public string? PhoneNumber { get; set; } // Phone number of the person booking    
        public int CourtId { get; set; } // Court ID where the booking was made

    }

}

