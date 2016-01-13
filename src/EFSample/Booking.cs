using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class Booking
    {
        public int BookingId { get; set; }
        public Flight Flight { get; set; }
        public int FlightId { get; set; }
        public int PassengerId { get; set; }
        public decimal Price { get; set; }
    }
}
