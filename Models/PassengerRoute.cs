using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainPopulation.Models
{
    public class PassengerRoute
    {
        public int PassengerRouteID { get; }
        public int PassengerID { get; }
        public int RouteID { get; }
        public int CarID { get; }
        public int TicketPrice { get; }

        public PassengerRoute(int passengerRouteID, int passengerID, int routeID, int carID, int ticketPrice)
        {
            PassengerRouteID = passengerRouteID;
            PassengerID = passengerID;
            RouteID = routeID;
            CarID = carID;
            TicketPrice = ticketPrice;
        }
    }
}
