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
        public int InTransit { get; set; }
        public List<Car> Cars { get; set; }

        public Train(int trainID, string name, string company, string driver, int baseSpeed, int carCapacity)
        {
            TrainID = trainID;
            Name = name;
            Company = company;
            Driver = driver;
            BaseSpeed = baseSpeed;
            CarCapacity = carCapacity;
            InTransit = 0;
            Cars = new List<Car>();
        }

        public void Advance()
        {
            Random rand = new Random();
            int rando = rand.Next(0, 100);
            if(rando >= 97)
            {
                InTransit -= 2;
            }
            else if(rando > 2)
            {
                InTransit--;
            }

            if (InTransit < 0)
            {
                InTransit = 0;
            }
        }
    }
}
