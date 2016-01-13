using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Data.Entity;
using EFSample;

namespace DataAccessLayer
{
    public class FlightManager
    {
        static FlightManager()
        {
            using (FlugDbContext ctx = new FlugDbContext())
            {
                ctx.Database.EnsureDeleted();
                ctx.Database.EnsureCreated();
                 
                if (!ctx.Flights.Any()) {

                    var f1 = new Flight { From = "Graz", To = "Hamburg", Date = DateTime.Now };
                    var f2 = new Flight { From = "Vienna", To = "London",  Date = DateTime.Now.AddHours(1) };
                    var f3 = new Flight { From = "Graz", To = "Hamburg", Date = DateTime.Now.AddHours(2) };
                    var f4 = new Flight { From = "Hamburg", To = "Graz", Date = DateTime.Now.AddHours(3) };
                    var f5 = new Flight { From = "Vienna", To = "London", Date = DateTime.Now.AddHours(4) };
                    var f6 = new Flight { From = "Graz", To = "Hamburg", Date = DateTime.Now.AddHours(5) };
                    var f7 = new Flight { From = "Vienna", To = "London",  Date = DateTime.Now.AddHours(6) };

                    var b1 = new Booking { PassengerId = 4711, Price = 300 };
                    f1.Bookings.Add(b1);

                    ctx.Flights.AddRange(f1, f2, f3, f4, f5, f6, f7);
                    ctx.SaveChanges();
                }
            }
        }



        public List<Flight> FindAll() {
            using (FlugDbContext ctx = new FlugDbContext()) {
                return ctx.Flights.ToList();
            }
        }

        public List<Flight> FindByRoute(string from, string to) {
            using (FlugDbContext ctx = new FlugDbContext())
            {
                return ctx.Flights.Where(f => f.From == from && f.To == to).ToList();
            }
        }

        public Flight FindById(int id) {
            using (FlugDbContext ctx = new FlugDbContext())
            {
                return ctx.Flights
                            .Include(f => f.Bookings)
                            .Where(f => f.Id == id)
                            .FirstOrDefault();
            }
        }

        public void Create(Flight flight) {
            using (FlugDbContext ctx = new FlugDbContext())
            {
                ctx.Flights.Add(flight);
                ctx.SaveChanges();
            }
        }

        public void Update(Flight flight)
        {
            using (FlugDbContext ctx = new FlugDbContext())
            {
                ctx.Flights.Update(flight);
                //ctx.Entry(flight).Property("PlaneType").CurrentValue = "Boeing";
                ctx.SaveChanges();
            }
        }

        public void Delete(Flight flight)
        {
            using (FlugDbContext ctx = new FlugDbContext())
            {
                ctx.Flights.Remove(flight);
                ctx.SaveChanges();
            }
        }

        public void Merge(Flight flight) {

            using (FlugDbContext ctx = new FlugDbContext())
            {
                ctx.ChangeTracker.TrackGraph(flight, (node) => {
                    node.Entry.State = EntityState.Modified;
                });
                ctx.SaveChanges();
            }
        }

        public bool Check(Flight f) {
            return f.Id == 1;
        }

        public void ShowShadowState() {

            using (FlugDbContext ctx = new FlugDbContext())
            {
                // ctx.LogToConsole();
                var flights = ctx.Flights.Where(f => Check(f)).ToList();

                foreach(var f in flights) {
                    Console.WriteLine(f.Id);
                    //var planeType = ctx.Entry(f).Property("PlaneType").CurrentValue;
                    //Console.WriteLine(planeType);
                }

            }
        }
    }
}
