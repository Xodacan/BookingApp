using TennisCourtAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace TennisCourtAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<Booking>? Bookings { get; set; } // Example DbSet
        public DbSet<Court>? Courts { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Seed some courts if none exist
        modelBuilder.Entity<Court>().HasData(
            new Court { CourtId = 1, CourtName = "Court 1" , Address = "add1"},
            new Court { CourtId = 2, CourtName = "Court 2" , Address = "add2"}
        );
    }
    }
}
