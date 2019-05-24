using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;

namespace AirportsTracking
{
    public class NextAirport
    {
        public Airport Current { get; set; }
        public double Weight { get; set; }
        public NextAirport previous { get; set; } // точка з якої прийшли
        public LinkedList<Route> adjacementList = new LinkedList<Route>(); // список сусідніх аеропортів
        public double PathLengthFromStart { get; set; } // довжина шляху від старту
        public double HeuristicEstimatePathLength { get; set; } // приблизна відстань до кінцевого аеропорту
        public double EstimateFullPathLength // очікувана довжина шляху до кінцевого аеропорта
        {
            get
            {
                return this.PathLengthFromStart + this.HeuristicEstimatePathLength;
            }
        }
        public NextAirport(Airport cur, string name, double price, LinkedList<Route> nextPorts)
        {
            Current = cur;
            Weight = price;
            adjacementList = nextPorts;
        }

    }
    
    public class Graph
    {
        private const int PRICE_FOR_1_KILOMETR = 32;
        static Dictionary<NextAirport, NextAirport> Path { get; set; }

        public static NextAirport Source { get; set; }

        static NextAirport Destination { get; set; }

        ///protected internal static Airport Airport { get; set; }
        // protected internal static LinkedList<Airport> adjacementList = new LinkedList<Airport>();


        ///https://stackoverflow.com/questions/6366408/calculating-distance-between-two-latitude-and-longitude-geocoordinates/44703178#44703178
        protected internal static double GetPath(Airport source, Airport destination)
        {
            var first = Math.PI * source.Latitude / 180;
            var firts2 = Math.PI * destination.Latitude / 180;
            var second = Math.PI * source.Longitude / 180;
            var second2 = Math.PI * destination.Latitude / 180;
            var theta = source.Longitude - destination.Longitude;
            var rTheta = Math.PI * theta / 180;
            var distance = Math.Sin(first) * Math.Sin(firts2) + Math.Cos(first) * Math.Cos(firts2) * Math.Cos(rTheta);
            distance = Math.Acos(distance);
            distance = distance * 180 / Math.PI;
            distance = distance * 60 * 1.1515 * 1.609344;//in kilometers
            return distance;
        }
        protected internal static double GetPriceByPath(string sourceCode, string destinationCode)
        {
            var source = AirlineData.GetAirPort(sourceCode);
            var destination = AirlineData.GetAirPort(destinationCode);
            var distance = GetPath(source, destination);
            return distance * PRICE_FOR_1_KILOMETR;

        }

        protected static internal void DijkstraMinPath(string sourceID, string destinationID)
        {
            try
            {
                var watch = Stopwatch.StartNew();
                var priotityQueue = new Queue<NextAirport>();
                List<string> visited = new List<string>();
                var path = new Dictionary<NextAirport, NextAirport>();
                Airport source = AirlineData.GetAirPort(sourceID);
                var next = AirlineData.GetNextStation(sourceID);
                next.Weight = 0;//sorce
                priotityQueue.Enqueue(next);
                Graph.Source = next;
                double BestPrice = 0;

                path[next] = null;
                while (priotityQueue.Count > 0)
                {

                    var _flight = priotityQueue.Dequeue();
                    if (_flight.Current.ID == destinationID)
                    {
                        watch.Stop();
                        Graph.Destination = _flight;
                        Graph.Path = path;
                        Console.WriteLine("Time spended {0}", watch.ElapsedMilliseconds);
                        Console.WriteLine("Count of visited vertices = {0}", visited.Count);
                        ReturnMinPath();

                        return;
                    }
                    visited.Add(_flight.Current.ID);

                    foreach (var neighbour in _flight.adjacementList)
                    {
                        NextAirport _next = AirlineData.GetNextStation(neighbour.DestinationId); ;
                        if (!visited.Contains(neighbour.DestinationId) && _next != null)
                        {
                            var best = GetPriceByPath(_flight.Current.ID, neighbour.DestinationId) + _flight.Weight;//maybe best price
                            if (best < _next.Weight)
                            {
                                _next.Weight = best;
                                priotityQueue.Enqueue(_next);
                                path[_next] = _flight;
                                SortQueue(ref priotityQueue);
                            }
                        }
                    }
                }

                Console.ReadKey();
            }
            catch (Exception r)

            {
                Console.WriteLine(r);
            }


        }
        protected static internal void ReturnMinPath()
        {

            List<NextAirport> result = new List<NextAirport>();
            var current = Destination;
            result.Add(current);
            while (current != Source)
            {
                current = Path[current];
                result.Add(current);
            }
            Console.WriteLine("From airport with code {0} and name {1}  " +
                "to airport  {2} and name {3}\n you can get with the best price {4}",
                Source.Current.ID, Source.Current.AirportName, Destination.Current.ID, Destination.Current.AirportName, Destination.Weight/32.0);
            Console.WriteLine(" Count of vetrices between two vertices = {0}", result.Count);
            Console.WriteLine("Path with minimal cost: ");
            for (int i = result.Count - 1; i >= 0; i--)
                Console.Write(result[i].Current.ID + " ->>>");
            Console.WriteLine();




        }

        protected static internal void SearchPointsWithBfs(string sourceID, string searchID)
        {
            try
            {
                var watch = Stopwatch.StartNew();
                Airport source = AirlineData.GetAirPort(sourceID);
                var next = AirlineData.GetNextStation(sourceID);
                var que = new Queue<NextAirport>();
                var visited = new List<Airport>();
                que.Enqueue(next);
                visited.Add(source);
                var counter = 0;
                while (que.Count > 0)
                {
                    var vertex = que.Dequeue();
                    if (vertex.Current.ID == searchID)
                    {
                        watch.Stop();
                        Console.WriteLine("Airport with code {0} and name {1} finded from start airport {2}", vertex.Current.ID, vertex.Current.AirportName, sourceID);
                        Console.WriteLine("Count of vetrices between two vertices = {0}", counter);
                        Console.WriteLine("Count of visited vertices = {0}", visited.Count);
                        Console.WriteLine("Time spended {0}", watch.ElapsedMilliseconds);
                        return;
                    }
                    foreach (var neighbour in vertex.adjacementList)
                    {
                        if (neighbour.DestinationId != null)
                        {
                            if (!(visited.Contains(AirlineData.GetAirPort(neighbour.DestinationId))))
                            {
                                que.Enqueue(AirlineData.GetNextStation(neighbour.DestinationId));
                                visited.Add(AirlineData.GetAirPort(neighbour.DestinationId));
                                counter++;
                            }
                        }
                    }

                }
            }
            catch (Exception r)
            {
                Console.WriteLine(r);
            }

        }

        protected static internal void SortQueue(ref Queue<NextAirport> queue)
        {

            queue.OrderBy(d => d.Weight);

        }
    }

    public class AStar
    {
        // added new class
        // 
        static NextAirport Source { get; set; }

        static NextAirport Destination { get; set; }

        protected internal static double GetPath(Airport source, Airport destination)
        {
            var first = Math.PI * source.Latitude / 180;
            var firts2 = Math.PI * destination.Latitude / 180;
            var second = Math.PI * source.Longitude / 180;
            var second2 = Math.PI * destination.Latitude / 180;
            var theta = source.Longitude - destination.Longitude;
            var rTheta = Math.PI * theta / 180;
            var distance = Math.Sin(first) * Math.Sin(firts2) + Math.Cos(first) * Math.Cos(firts2) * Math.Cos(rTheta);
            distance = Math.Acos(distance);
            distance = distance * 180 / Math.PI;
            distance = distance * 60 * 1.1515 * 1.609344; //in kilometers
            return distance;
        }

        protected static internal List<NextAirport> AStarMinPath(string sourceID, string destinationID)
        {
            // отримуємо найкоротший шлях від аеропорта А до аеропорта Б

            NextAirport neighbourNode1;

            var watch = Stopwatch.StartNew();
            var closedSet = new Queue<NextAirport>();
            var openSet = new Queue<NextAirport>();

            AStar.Source = AirlineData.GetNextStation(sourceID);
            AStar.Destination = AirlineData.GetNextStation(destinationID);

            AStar.Source.previous = null;
            AStar.Source.PathLengthFromStart = 0;
            AStar.Source.HeuristicEstimatePathLength = GetPath(AStar.Source.Current, AStar.Destination.Current);

            openSet.Enqueue(AStar.Source);

            // Console.WriteLine($"Start searching path from {AStar.Source.Current.AirportName} to {AStar.Destination.Current.AirportName} ");
            while (openSet.Count > 0)
            {
                var currentAirport = openSet.OrderBy(node => node.EstimateFullPathLength).First();
                currentAirport = openSet.Dequeue();

                if (currentAirport.Current.ID == destinationID) // mccarran
                {
                    Console.WriteLine("Time spended {0}", watch.ElapsedMilliseconds);
                    Console.WriteLine($"There is the way from {AStar.Source.Current.AirportName} to {AStar.Destination.Current.AirportName} ");
                    return ReturnMinPath(currentAirport);
                }

                closedSet.Enqueue(currentAirport);

                foreach (var neighbourAirport in currentAirport.adjacementList)
                {                    
                    neighbourNode1 = AirlineData.GetNextStation(neighbourAirport.DestinationId);
                    
                    if (closedSet.Count(node => node.Current.ID == neighbourNode1.Current.ID) > 0)
                         continue;

                    double temp_PathLenghtFromStart = currentAirport.PathLengthFromStart + GetPath(currentAirport.Current, neighbourNode1.Current);                                                                                                                                                     // neighbourNode1.PathLengthFromStart = currentAirport.PathLengthFromStart + GetPath(currentAirport.Current, AirlineData.GetAirPort(neighbourNode.ITA)); // виглядає правильніше, працює довше, результат коректний

                    var openNode = openSet.FirstOrDefault(node => node.Current.ID == neighbourNode1.Current.ID);
                    if (openNode == null || temp_PathLenghtFromStart <= neighbourNode1.PathLengthFromStart)
                    {
                        neighbourNode1.previous = currentAirport;
                        neighbourNode1.PathLengthFromStart = temp_PathLenghtFromStart;
                        neighbourNode1.HeuristicEstimatePathLength = GetPath(neighbourNode1.Current, AStar.Destination.Current);
                    }
                    if (openNode == null)
                        openSet.Enqueue(neighbourNode1);

                }

            }
            Console.WriteLine($"There is no way from airport with name {AStar.Source.Current.AirportName} to airport {AStar.Destination.Current.AirportName}");
            Console.WriteLine("Time spended {0}", watch.ElapsedMilliseconds);

            return null;
        }

        protected static internal List<NextAirport> ReturnMinPath(NextAirport pathNode) // відновлення шляху після алгоритму А*
        {
            List<NextAirport> result = new List<NextAirport>();
            var currentNode = pathNode;
            Console.WriteLine(currentNode.PathLengthFromStart);
            result.Add(pathNode);
            while (currentNode != null)
            {
                result.Add(currentNode.previous);
                currentNode = currentNode.previous;
            }

            result.Reverse();
            result.RemoveAt(0);
            foreach (var x in result)
                Console.Write($" {x.Current.ID} ->>>");
            Console.WriteLine();
            return result;
        }

    }

}


