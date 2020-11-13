/* 1.   If it is a departure time (8, noon, 4, or 8) check each node,
 *      if a node has any waiting trains give each train a 33% chance to 
 *      depart. If a train departs give each passenger a 40% chance to depart
 *      as well(may need to add more passengers). Choose a random location
 *      from the Node's dictionary for that train to travel to. At that 
 *      moment, create the Route and PassengerRoute entries, add departing 
 *      passengers to departing train, and remove those passangers and the 
 *      train from the node. Also put the distance from the dictionary into
 *      the trains distance, set that trains current location to its destination
 *      and set routeID equal to the route it is on. Also need to give each 
 *      passenger the carID of the car they are on.
 * 2.   Call a travel function on each train. Travel function generates a 
 *      number 1-100. If it is 3 or less, do nothing. If it is 98 or
 *      greater, subtract 2 to travel. Otherwise, subtract 1 from travel.
 * 3.   Check if any trains have 0 in their travel value. If that is the case
 *      put all passengers on that train into the location.
 *      Update the Route to add in the arrival time.
 * 4.   Repeat until a years worth of minutes have passed.
 * 
*/




using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using TrainPopulation.Models;

namespace TrainPopulation
{
    class Program
    {
        static void Main(string[] args)
        {
            Random rand = new Random();
            int carID = 1;
            DateTimeOffset date = new DateTimeOffset(2019, 1, 1, 0, 0, 0, new TimeSpan(0, 0, 0));

            const string connectionString = @"Server=(localdb)\MSSQLLocalDb;Database=CIS 560;Integrated Security=SSPI;";
            List<Train> trains;
            List<Car> cars = new List<Car>();
            List<Passenger> passengers;
            List<Route> routes = new List<Route>();
            List<PassengerRoute> passengerRoutes = new List<PassengerRoute>();

            List<Node> nodes;

            //Create train, car, and passenger tables
            trains = createTrains(connectionString);
            foreach(Train t in trains)
            {
                cars.AddRange(createCars(connectionString, t, carID));
                carID = cars.Count + 1;
            }
            passengers = createPassengers(connectionString);

            //Randomly assign starting locations
            foreach(Passenger p in passengers)
            {
                p.assignStartLocation((Location)rand.Next(0, 20));
            }
            foreach (Train t in trains)
            {
                t.assignStartLocation((Location)rand.Next(0, 20));
            }

            //Populate Graph
            nodes = populateGraph();
            addTrainsToNodes(nodes, trains);
            addPassengersToNodes(nodes, passengers);


            //for the year of 2019
            while (date.Year != 2020)
            {
                //if it is a departure time
                if( (date.Hour == 8  && date.Minute == 0) ||
                    (date.Hour == 12 && date.Minute == 0) ||
                    (date.Hour == 16 && date.Minute == 0) ||
                    (date.Hour == 20 && date.Minute == 0))
                {
                    foreach (Node n in nodes)
                    {
                        foreach (Train t in n.Trains)
                        {
                            if (t.InTransit == 0)
                            {
                                if (rand.Next(0, 3) == 0)
                                {
                                    Dictionary<Passenger, Car> departingPasssengers = new Dictionary<Passenger, Car>();
                                    foreach (Passenger p in n.Passengers)
                                    {
                                        if (rand.Next(0, 2) == 0)
                                        {
                                            int carChoice = rand.Next(0, t.Cars.Count);
                                            //randomly choose one of the cars in that particular train and see if it has open seats
                                            if (t.Cars[carChoice].Passengers.Count < t.Cars[carChoice].PassengerCapacity)
                                            {//why am i so bad at programming
                                                departingPasssengers.Add(p, t.Cars[carChoice]);
                                            }
                                        }
                                    }
                                    //Create the Route object for the train's journey and insert it into the database
                                    Location arrivalLocation = n.Edges.Keys.ElementAt(rand.Next(0, n.Edges.Keys.Count));
                                    Route newRoute = new Route(routes.Count + 1, t.TrainID, n.Location, arrivalLocation, date, n.Edges[arrivalLocation]);
                                    routes.Add(insertRoute(connectionString, newRoute));

                                    //set the trains travel distance
                                    t.InTransit = newRoute.Distance;

                                    for(int i = 0; i < departingPasssengers.Count; i++)
                                    {
                                        //create a PassengerRoute object for each departingPassenger, then remove the passenger from the location
                                        //yall want some long varible names
                                        PassengerRoute newPR = new PassengerRoute(passengerRoutes.Count + 1, departingPasssengers.Keys.ElementAt(i).PassengerID, newRoute.RouteID, departingPasssengers.Values.ElementAt(1).CarID, departingPasssengers.Values.ElementAt(1).TicketPrice);
                                        passengerRoutes.Add(insertPassengerRoute(connectionString, newPR));
                                        n.Passengers.RemoveAt(n.Passengers.IndexOf(departingPasssengers.Keys.ElementAt(i)));
                                    }
                                    //set the trains travel distance
                                    t.InTransit = newRoute.Distance;

                                    //add the train to the desination node and remove it from the current node
                                    foreach(Node no in nodes)
                                    {
                                        if(no.Location == newRoute.ArrivalLocation)
                                        {
                                            no.Trains.Add(t);
                                            t.CurrentLocation = no.Location;
                                        }
                                    }
                                    n.Trains.RemoveAt(n.Trains.IndexOf(t));
                                    break;
                                }
                            }
                        }
                    }
                }

                //Advance each train on their paths and check to see if they have arrived.
                //This is done after the departure check because it would be weird for people to instantly leave a place they just arrived at
                foreach(Train t in trains)
                {
                    t.Advance();
                    if(t.InTransit == 0)
                    {
                        for(int i = 0; i < routes.Count; i++)
                        {
                            if(routes[i].TrainID == t.TrainID)
                            {
                                updateRoute(connectionString, routes[i], date);
                                break;
                            }
                        }

                        int index = -1;
                        for (int i = 0; i < nodes.Count; i++)
                        {
                            if(nodes[i].Location == t.CurrentLocation)
                            {
                                index = i;
                                break;
                            }
                        }
                        foreach(Car c in t.Cars)
                        {
                            foreach(Passenger p in c.Passengers)
                            {
                                if(index != -1)
                                {
                                    nodes[index].Passengers.Add(p);
                                }
                                else
                                {
                                    throw new System.ArgumentException("Missing or incorrect Location");
                                }
                            }
                        }
                    }
                }

                date = date.AddMinutes(1);
            }
        }

        public static List<Train> createTrains(string connectionString)
        {
            int trainID = 1;
            List<Train> newTrains = new List<Train>();

            using(StreamReader reader = new StreamReader("Trains.txt"))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    if (line[0] != '|')
                    {
                        string[] columns = line.Split('|');

                        newTrains.Add(new Train(trainID, columns[0], columns[1], columns[2], Convert.ToInt32(columns[3]), Convert.ToInt32(columns[4])));
                        trainID++;
                    }
                }
            }
            using(var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                
                foreach(Train t in newTrains)
                {
                    string sqlRequest = "INSERT Trains.Train([Name], Company, Driver, BaseSpeed, CarCapacity) " +
                                        "VALUES(@Name, @Company, @Driver, @BaseSpeed, @CarCapacity)";

                    using (var command = new SqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar);
                        command.Parameters.Add("@Company", System.Data.SqlDbType.NVarChar);
                        command.Parameters.Add("@Driver", System.Data.SqlDbType.NVarChar);
                        command.Parameters.Add("@BaseSpeed", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@CarCapacity", System.Data.SqlDbType.Int);

                        command.Parameters["@Name"].Value = t.Name;
                        command.Parameters["@Company"].Value = t.Company;
                        command.Parameters["@Driver"].Value = t.Driver;
                        command.Parameters["@BaseSpeed"].Value = t.BaseSpeed;
                        command.Parameters["@CarCapacity"].Value = t.CarCapacity;

                        int rows = command.ExecuteNonQuery();
                    }
                }
            }
            return newTrains;
        }

        public static List<Car> createCars(string connectionString, Train t, int carID)
        {
            int ID = carID;
            Random rand = new Random();
            List<Car> cars = new List<Car>();

            using(var connection = new SqlConnection(connectionString))
            {
                //Console.WriteLine(t.TrainID);
                //Console.WriteLine(t.CarCapacity);
                connection.Open();
                for (int j = 0; j < t.CarCapacity; j++)
                {
                    int carType = rand.Next(1, 4);
                    Car newCar = new Car(ID, t.TrainID, carType, ( rand.Next(0, 11) + (20 * carType) ), rand.Next(10, 20));
                    string sqlRequest = "INSERT Trains.Car(TrainID, CarTypeID, TicketPrice, PassengerCapacity)" +
                                        "VALUES(@TrainID, @CarTypeID, @TicketPrice, @PassengerCapacity)";
                    using (var command = new SqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.Add("@TrainID", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@CarTypeID", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@TicketPrice", System.Data.SqlDbType.Int);
                        command.Parameters.Add("@PassengerCapacity", System.Data.SqlDbType.Int);

                        command.Parameters["@TrainID"].Value = newCar.TrainID;
                        command.Parameters["@CarTypeID"].Value = newCar.CarTypeID;
                        command.Parameters["@TicketPrice"].Value = newCar.TicketPrice;
                        command.Parameters["@PassengerCapacity"].Value = newCar.PassengerCapacity;

                        command.ExecuteNonQuery();
                    }
                    cars.Add(newCar);
                    ID++;
                }
            }
            t.Cars.AddRange(cars);
            return cars;
        }

        public static List<Passenger> createPassengers(string connectionString)
        {
            int passengerID = 1;
            List<Passenger> passengers = new List<Passenger>();

            using (StreamReader reader = new StreamReader("Passengers.txt"))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    if (line[0] != '|')
                    {
                        string[] columns = line.Split('|');
                        passengers.Add(new Passenger(passengerID, columns[0], columns[1], columns[2]));
                        passengerID++;
                    }
                }
            }
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                foreach (Passenger p in passengers)
                {
                    string sqlRequest = "INSERT Trains.Passenger(FirstName, LastName, Email)" +
                                        "VALUES(@FirstName, @LastName, @Email)";
                    using (var command = new SqlCommand(sqlRequest, connection))
                    {
                        command.Parameters.Add("@FirstName", System.Data.SqlDbType.NVarChar);
                        command.Parameters.Add("@LastName", System.Data.SqlDbType.NVarChar);
                        command.Parameters.Add("@Email", System.Data.SqlDbType.NVarChar);

                        command.Parameters["@FirstName"].Value = p.FirstName;
                        command.Parameters["@LastName"].Value = p.LastName;
                        command.Parameters["@Email"].Value = p.Email;

                        command.ExecuteNonQuery();
                    }
                }
            }
            return passengers;
        }

        public static List<Node> populateGraph()
        {
            //Assign each node a location
            List<Node> nodes = new List<Node>();
            for(int i = 0; i < 20; i++)
            {
                nodes.Add(new Node((Location)i));
            }

            using (StreamReader reader = new StreamReader("Locations.txt"))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    string[] location = line.Split('|');
                    for(int j = 0; j < Convert.ToInt32(location[1]); j++)
                    {
                        line = reader.ReadLine();
                        string[] edge = line.Split('|');
                        foreach(Node n in nodes)
                        {
                            if(n.Location.ToString() == location[0])
                            {
                                Location loc;
                                if(Enum.TryParse<Location>(edge[0], out loc))
                                {
                                    n.addLocation(loc, Convert.ToInt32(edge[1]));
                                }
                                else
                                {
                                    throw new System.ArgumentException("Location.txt does not match Location enum", edge[0]);
                                }
                            }
                        }
                    }
                }
            }
            return nodes;
        }

        public static void addTrainsToNodes(List<Node> nodes, List<Train> trains)
        {
            Random rand = new Random();
            foreach(Train t in trains)
            {
                nodes[rand.Next(0, nodes.Count)].addTrain(t);
            }
        }

        public static void addPassengersToNodes(List<Node> nodes, List<Passenger> passengers)
        {
            Random rand = new Random();
            foreach (Passenger p in passengers)
            {
                nodes[rand.Next(0, nodes.Count)].addPassenger(p);
            }
        }

        public static Route insertRoute(string connectionString, Route route)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sqlRequest = "INSERT Trains.[Route](TrainID, DepartureLocation, ArrivalLocation, DepartureTime, Distance) " +
                                    "VALUES(@TrainID, @DepartureLocation, @ArrivalLocation, @DepartureTime, @Distance)";
                using (var command = new SqlCommand(sqlRequest, connection))
                {
                    command.Parameters.Add("@TrainID", System.Data.SqlDbType.Int);
                    command.Parameters.Add("@DepartureLocation", System.Data.SqlDbType.NVarChar);
                    command.Parameters.Add("@ArrivalLocation", System.Data.SqlDbType.NVarChar);
                    command.Parameters.Add("@DepartureTime", System.Data.SqlDbType.DateTimeOffset);
                    command.Parameters.Add("@Distance", System.Data.SqlDbType.Int);


                    command.Parameters["@TrainID"].Value = route.TrainID;
                    command.Parameters["@DepartureLocation"].Value = route.DepartureLocation;
                    command.Parameters["@ArrivalLocation"].Value = route.ArrivalLocation;
                    command.Parameters["@DepartureTime"].Value = route.DepartureTime;
                    command.Parameters["@Distance"].Value = route.Distance;

                    command.ExecuteNonQuery();
                }
            }
            return route;
        }

        public static PassengerRoute insertPassengerRoute(string connectionString, PassengerRoute passengerRoute)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sqlRequest = "INSERT TrainsCar(PassengerID, CarID, RouteID, TicketPrice) " +
                                    "VALUES(@PassengerID, @CarID, @RouteID, @TicketPrice)";
                using (var command = new SqlCommand(sqlRequest, connection))
                {
                    command.Parameters.Add("@PassengerID", System.Data.SqlDbType.Int);
                    command.Parameters.Add("@CarID", System.Data.SqlDbType.Int);
                    command.Parameters.Add("@RouteID", System.Data.SqlDbType.Int);
                    command.Parameters.Add("@TicketPrice", System.Data.SqlDbType.Int);

                    command.Parameters["@PassengerID"].Value = passengerRoute.PassengerID;
                    command.Parameters["@CarID"].Value = passengerRoute.CarID;
                    command.Parameters["@RouteID"].Value = passengerRoute.RouteID;
                    command.Parameters["@TicketPrice"].Value = passengerRoute.TicketPrice;

                    command.ExecuteNonQuery();
                }
            }
            return passengerRoute;
        }

        public static void updateRoute(string connectionString, Route route, DateTimeOffset date)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sqlRequest = "UPDATE Trains.[Route] " +
                                    "SET ArrivalTime = @ArrivalTime " +
                                    "WHERE RouteID = @RouteID";
                using (var command = new SqlCommand(sqlRequest, connection))
                {
                    command.Parameters.Add("@ArrivalTime", System.Data.SqlDbType.DateTimeOffset);
                    command.Parameters.Add("@RouteID", System.Data.SqlDbType.Int);

                    command.Parameters["@ArrivalTime"].Value = date;
                    command.Parameters["@RouteID"].Value = route.RouteID;

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
