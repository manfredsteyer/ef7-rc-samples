using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample02
{
    public class Flight
    {
        public int Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTime Date { get; set; }

        public List<Booking> Bookings { get; set; } = new List<Booking>();
    }

    public class Booking
    {
        public int BookingId { get; set; }

        public Flight Flight { get; set; }
        public int FlightId { get; set; }

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
            //
            // alter table flight add PlaneType varchar(50) null;
            // update flight set PlaneType = 'Airbus'
            //
            modelBuilder.Entity<Flight>().Property(typeof(String), "PlaneType");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=(localdb)\ProjectsV12;Initial Catalog=FlightDb-NDC2;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }

    }

    public class Program
    {
        static void CreateData()
        {
            using (FlugDbContext ctx = new FlugDbContext())
            {
                // ctx.Database.EnsureDeleted();
                ctx.Database.EnsureCreated();

                var f1 = new Flight { From = "Graz", To = "Hamburg", Date = DateTime.Now };
                var b1 = new Booking { Price = 300 };
                f1.Bookings.Add(b1);

                ctx.Flights.Add(f1);
                ctx.SaveChanges();

                #region addFurtherData

                var f2 = new Flight { From = "Vienna", To = "London", Date = DateTime.Now.AddHours(1) };
                var f3 = new Flight { From = "Graz", To = "Hamburg", Date = DateTime.Now.AddHours(2) };
                var f4 = new Flight { From = "Hamburg", To = "Graz", Date = DateTime.Now.AddHours(3) };
                var f5 = new Flight { From = "Vienna", To = "London", Date = DateTime.Now.AddHours(4) };
                var f6 = new Flight { From = "Graz", To = "Hamburg", Date = DateTime.Now.AddHours(5) };
                var f7 = new Flight { From = "Vienna", To = "London", Date = DateTime.Now.AddHours(6) };

                ctx.Flights.AddRange(f2, f3, f4, f5, f6);
                ctx.Flights.Add(f7);

                ctx.SaveChanges();

                #endregion

            }
        }

        static void RawSQL()
        {
            using (FlugDbContext ctx = new FlugDbContext())
            {
                // Please find the TVF GetFlights in the attached Textfile
                var flight = ctx.Flights
                                .FromSql("select * from dbo.GetFlights(@p0, @p1)", "Vienna", "London")
                                .Where(f => f.Date > DateTime.Today)
                                .ToList();

                Console.WriteLine("Count: " + flight.Count);
            }
        }
        
        static void ShadowProperties()
        {
            using (FlugDbContext ctx = new FlugDbContext())
            {
                var flight = ctx.Flights.First(f => f.Id == 1);
                var planeType = ctx.Entry(flight).Property("PlaneType").CurrentValue;

                Console.WriteLine("PlaneType: " + planeType);


                var flights = ctx.Flights
                                 .Where(f => EF.Property<string>(f, "PlaneType") == "Airbus")
                                 .ToList();

                Console.WriteLine("Count: " + flights.Count);
            }
        }

        public static void Main(string[] args)
        {
            CreateData();

            //RawSQL();

            ShadowProperties();

        }
    }
}
