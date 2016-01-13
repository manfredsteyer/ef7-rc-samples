using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample01
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

    public class Passenger {
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
            modelBuilder.Entity<Flight>().ToTable("Flights");

            // modelBuilder.HasSequence("Seq");
            // modelBuilder.Entity<Flight>().HasKey(p => p.Id).ForSqlServerIsClustered();

            // "Custom-Conventions light"
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                modelBuilder.Entity(entity.Name).ToTable(entity.ClrType.Name + "s");
            }

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer(@"Data Source=(localdb)\ProjectsV12;Initial Catalog=FlightDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
                //.DisableQueryClientEvaluation();
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

                var f1 = new Flight { From = "Vienna", To = "London", Date = DateTime.Now };
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
        static void UpdateDetached()
        {
            Flight flight;

            using (FlugDbContext ctx = new FlugDbContext())
            {
                flight = ctx.Flights
                            .Include(f => f.Bookings)
                            .FirstOrDefault(f => f.Id == 1);
            }

            flight.Date = flight.Date.AddMinutes(30);
            flight.Bookings.Add(new Booking { Price = 222 });

            using (FlugDbContext ctx = new FlugDbContext())
            {
                ctx.Flights.Update(flight); 
                ctx.SaveChanges();          
            }
        }
        static void Merge()
        {
            Flight flight;

            using (FlugDbContext ctx = new FlugDbContext())
            {
                flight = ctx.Flights
                            .Include(f => f.Bookings)
                            .FirstOrDefault(f => f.Id == 1);
            }

            flight.Date = flight.Date.AddMinutes(30);

            using (FlugDbContext ctx = new FlugDbContext())
            {
                ctx.ChangeTracker.TrackGraph(flight, (node) => {
                    node.Entry.State = EntityState.Modified;
                });
                ctx.SaveChanges();
            }
        }
        static void EagerLoading()
        {
            using (FlugDbContext ctx = new FlugDbContext())
            {
                var flight = ctx.Flights
                                //.Include(f => f.Bookings.Select(b => b.Passenger))
                                .Include(f => f.Bookings)
                                .ThenInclude(b => b.Passenger)
                                .Where(f => f.From == "Vienna")
                                .ToList();

                // ... do stuff ...
            }
        }

        // Mixed Server/Client-Evaluation
        static void ClientEval()
        {
            using (FlugDbContext ctx = new FlugDbContext())
            {
                ctx.LogToConsole();

                var flight = ctx.Flights
                                .Where(f => f.Date >= DateTime.Today
                                                && CheckRoute(f, "Vienna-London"))
                                .ToList();

                // ... do stuff ...
            }
        }

        static bool CheckRoute(Flight f, string route) {
            return (f.From + "-" + f.To) == route;
        }


        public static void Main(string[] args)
        {
            CreateData();
            // UpdateDetached();
            // Merge();
            // EagerLoading();
            // ClientEval();

            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}
