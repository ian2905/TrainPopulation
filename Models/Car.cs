using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TrainPopulation.Models
{
    public class Car
    {
        public int CarID { get; }
        public int TrainID { get; }
        public int CarTypeID { get; }
        public int TicketPrice { get; }
        public int PassengerCapacity { get; }
        public List<Passenger> Passengers { get; set; }

        public Car(int carID, int trainID, int carTypeID, int ticketPrice, int passengerCapacity)
        {
            CarID = carID;
            TrainID = trainID;
            CarTypeID = carTypeID;
            TicketPrice = ticketPrice;
            PassengerCapacity = passengerCapacity;
            Passengers = new List<Passenger>();
        }
    }
}
