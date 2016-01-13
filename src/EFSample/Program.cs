using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var fm = new FlightManager();

            var flights = fm.FindByRoute("Vienna", "London");
            
            foreach (var flight in flights) {
                Console.WriteLine(flight.Id + ", " + flight.Date);
            }

            var f = fm.FindById(1);
            fm.Update(f);

            fm.ShowShadowState();

            Console.WriteLine("Done.");
            Console.ReadLine();

        }
    }
}
