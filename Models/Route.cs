using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace TrainPopulation.Models
{
    public enum Location
    {
        Güssing,
        Minitonas,
        Bontang,
        Woodstock,
        Villahermosa,
        Pomarolo,
        Essex,
        Innisfail,
        HŽvillers,
        Brodick,
        Morvi,
        Yeovil,
        Portsmouth,
        Pakpatan,
        Nandyal,
        Selkirk,
        Bolhe,
        Wilskerke,
        GrandManil,
        Rhayader
    }


    public class Route
    {
        public int RouteID { get; }
        public int TrainID { get; }
        public Location DepartureLocation { get; }
        public Location ArrivalLocation { get; }
        public DateTimeOffset DepartureTime { get; }
        public DateTimeOffset ArrivalTime { get; set; }
        public int Distance { get;  }
        public int InTransit { get; set; }

        public Route(int routeID, int trainID, Location departureLocation, Location arrivalLocation, DateTimeOffset departureTime, int distance)
        {
            RouteID = routeID;
            TrainID = trainID;
            DepartureLocation = departureLocation;
            ArrivalLocation = arrivalLocation;
            DepartureTime = departureTime;
            ArrivalTime = new DateTimeOffset(2018, 1, 1, 0, 0, 0, new TimeSpan(0, 0, 0));
            Distance = distance;
        }

        public void SetArrivalTime(DateTimeOffset arrivalTime)
        {
            ArrivalTime = arrivalTime;
        }


    }
}
