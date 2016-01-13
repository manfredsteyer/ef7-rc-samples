using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample03
{
    public class Flight
    {
        public int Id { get; set; }
        public string FlightNumber { get; set; } // LH 0815 
        public string From { get; set; }
        public string To { get; set; }
        public DateTime Date { get; set; }

        public List<Booking> Bookings { get; set; } = new List<Booking>();
    }

    public class Booking
    {
        public int BookingId { get; set; }

        public Flight Flight { get; set; }
        public string FlightNumber { get; set; }

        public Passenger Passenger { get; set; }
        public int? PassengerId { get; set; }

        public decimal Price { get; set; }
    }

    public class Passenger
    {
        public int PassengerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public List<Booking> Bookings { get; set; }
    }

    public class FlugDbContext : DbContext
    {
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Passenger> Passengers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Flight>().HasAlternateKey(f => f.FlightNumber);

            modelBuilder.Entity<Flight>()
                        .HasMany(f => f.Bookings)
                        .WithOne(b => b.Flight)
                        .HasForeignKey(b => b.FlightNumber)
                        .HasPrincipalKey(f => f.FlightNumber);

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=(localdb)\ProjectsV12;Initial Catalog=FlightDb-NDC3;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }

    }

    public class Program
    {
        static void CreateData()
        {
            using (FlugDbContext ctx = new FlugDbContext())
            {
                ctx.Database.EnsureDeleted();
                ctx.Database.EnsureCreated();

                var f1 = new Flight { FlightNumber = "LH0815", From = "Graz", To = "Hamburg", Date = DateTime.Now };
                var b1 = new Booking { Price = 300 };
                f1.Bookings.Add(b1);

                ctx.Flights.Add(f1);
                ctx.SaveChanges();

                ctx.SaveChanges();
            }
        }

        public static void Main(string[] args)
        {
            CreateData();

          

        }
    }

}
