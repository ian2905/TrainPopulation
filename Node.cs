using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainPopulation.Models;

namespace TrainPopulation
{
    public class Node
    {
        public Location Location { get; }
        public Dictionary<Location, int> Edges = new Dictionary<Location, int>();
        public List<Train> Trains = new List<Train>();
        public List<Passenger> Passengers = new List<Passenger>();
        
        public Node(Location location)
        {
            Location = location;
        }

        public void addLocation(Location loc, int dist)
        {
            Edges.Add(loc, dist);
        }

        public void addTrain(Train t)
        {
            Trains.Add(t);
        }

        public void addPassenger(Passenger p)
        {
            Passengers.Add(p);
        }




    }
}
