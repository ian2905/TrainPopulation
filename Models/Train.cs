using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainPopulation.Models
{
    public class Train
    {
        public int TrainID { get; }
        public string Name { get; }
        public string Company { get; }
        public string Driver { get; }
        public int BaseSpeed { get; }
        public int CarCapacity { get; }
        public bool InTransit { get; set; }
        public int Distance { get; set; }
        public List<Car> Cars { get; set; }
        public Location CurrentLocation { get; set; }

        public Train(int trainID, string name, string company, string driver, int baseSpeed, int carCapacity)
        {
            TrainID = trainID;
            Name = name;
            Company = company;
            Driver = driver;
            BaseSpeed = baseSpeed;
            CarCapacity = carCapacity;
            InTransit = false;
            Cars = new List<Car>();
        }

        public void Advance()
        {
            Random rand = new Random();
            int rando = rand.Next(0, 100);
            if(rando >= 97)
            {
                Distance -= 2 * (BaseSpeed/60);
            }
            else if(rando > 2)
            {
                Distance -= (BaseSpeed/60);
            }

            if (Distance < 0)
            {
                Distance = 0;
            }
        }

        public void assignStartLocation(Location l)
        {
            CurrentLocation = l;
        }
    }
}
