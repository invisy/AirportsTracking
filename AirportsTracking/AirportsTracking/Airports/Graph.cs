﻿using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;

namespace Airports
{
   public class NextAirport
   {  
        public Airport Current { get; set; }
        public string IATA { get; set; }
        public string Name { get; set; }
        public double Weight { get; set; }
        public NextAirport previous { get; set; } // точка з якої прийшлиі
        public LinkedList<Airport> adjacementList = new LinkedList<Airport>(); // список сусідніх аеропортів
        public double PathLengthFromStart { get; set; } // довжина шляху від старту
        public double HeuristicEstimatePathLength { get; set; } // приблизна відстань до кінцевого аеропорту
        public double EstimateFullPathLength // очікувана довжина шляху до кінцевого аеропорта
        {
            get
            {
                return this.PathLengthFromStart + this.HeuristicEstimatePathLength;
            }
        }
        public NextAirport(string cur, string name, double price, LinkedList<Airport> nextPorts)
        {
            IATA = cur;
            Name = name;
            Weight = price;
            adjacementList = nextPorts;
        }
        public NextAirport(Airport cur, string name, double price, LinkedList<Airport> nextPorts)
        {
            Current = cur;
            IATA = cur.IATA;
            Name = name;
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

        protected static internal List<NextAirport> DijkstraMinPath(string sourceCode, string destinationCode)
        {
            try
            {
                AirlineData.LoadData();
                var dictOfNeighboursByCode = AirlineData.DictOfNeighbours;
                var watch = Stopwatch.StartNew();
                var priotityQueue  = new Queue<NextAirport>();
                List<string> visited = new List<string>();
                var path = new Dictionary<NextAirport, NextAirport>();
                Airport source = AirlineData.GetAirPort(sourceCode);
                var next = AirlineData.GetNextStation(sourceCode);
                next.Weight = 0;//sorce
                priotityQueue.Enqueue(next);
                Graph.Source = next;
                double BestPrice = 0;
   
                path[next] = null;
                while (priotityQueue.Count > 0)
                {

                    var _flight = priotityQueue.Dequeue();
                    if (_flight.Current.IATA == destinationCode)
                    {
                        watch.Stop();
                        Graph.Destination = _flight;
                        Graph.Path = path;
                        Console.WriteLine("Time spended {0}", watch.ElapsedMilliseconds);
                        Console.WriteLine("Count of visited vertices = {0}", visited.Count);
                        
                        return ReturnMinPath();
                    }
                    visited.Add(_flight.IATA);

                    foreach (var neighbour in _flight.adjacementList)
                    {

                        if (neighbour != null)
                        {
                            NextAirport _next = dictOfNeighboursByCode[neighbour.IATA];
                            if (!visited.Contains(neighbour.IATA) && _next != null)
                            {
                                var best = GetPriceByPath(_flight.IATA, neighbour.IATA) + _flight.Weight;//maybe best price
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
                }

                Console.ReadKey();
            }
            catch (Exception r)

            {
                Console.WriteLine(r);
            }
            //Мейбі тут не так ретурн
            return null;
        
        }
        protected static internal List<NextAirport> ReturnMinPath()
        {
           
            List<NextAirport> result = new List<NextAirport>();
            var current = Destination;
            result.Add(current);
            while (current!= Source)
            {
                current = Path[current];
                result.Add(current);              
            }
            Console.WriteLine("From airport with code {0} and name {1}  " +
                "to airport  {2} and name {3}\n you can get with the best price {4}",
                Source.IATA, Source.Current.AirportName, Destination.IATA, Destination.Name, Destination.Weight);
            Console.WriteLine(" Count of vetrices between two vertices = {0}", result.Count);
            Console.WriteLine("Path with minimal cost: ");
            for (int i = result.Count-1; i >= 0; i--)
                Console.WriteLine(result[i].IATA +" ->>>");

            result.Reverse();
            return result;
           

        }
     
        protected static internal void SearchPointsWithBfs(string sourceCode, string searchCode)
        {
            try
            {
                var watch = Stopwatch.StartNew();
                AirlineData.LoadData();
                var dictOfNeighboursByCode = AirlineData.DictOfNeighbours;
                Airport source = AirlineData.GetAirPort(sourceCode);
                var next = AirlineData.GetNextStation(sourceCode);
                var que = new Queue<NextAirport>();
                var visited = new List<Airport>();
                que.Enqueue(next);
                visited.Add(source);
                var counter = 0;
                while(que.Count>0)
                {
                    var vertex = que.Dequeue();
                    if (vertex.IATA == searchCode)
                    {                
                        watch.Stop();
                        Console.WriteLine("Airport with code {0} and name {1} finded from start airport {2}", vertex.IATA, vertex.Current.AirportName, sourceCode);
                        Console.WriteLine("Count of vetrices between two vertices = {0}", counter);
                        Console.WriteLine("Count of visited vertices = {0}", visited.Count);
                        Console.WriteLine("Time spended {0}", watch.ElapsedMilliseconds);
                        return;
                    }
                    foreach(var neighbour in vertex.adjacementList)
                    {
                        if (neighbour != null)
                        {
                            if (!(visited.Contains(neighbour)))
                            {
                                que.Enqueue(dictOfNeighboursByCode[neighbour.IATA]);
                                visited.Add(neighbour);
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
        
        protected static internal void SortQueue( ref Queue<NextAirport> queue)
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

        protected static internal List<NextAirport> AStarMinPath(string sourceCode, string destinationCode)
        {
            // отримуємо найкоротший шлях від аеропорта А до аеропорта Б 
            AirlineData.LoadData();
            var watch = Stopwatch.StartNew();
            var closedSet = new Queue<NextAirport>();
            var openSet = new Queue<NextAirport>();

            Airport source = AirlineData.GetAirPort(sourceCode);
            var next = AirlineData.GetNextStation(sourceCode);
            Graph.Source = next;
            var qq = AirlineData.GetAirPort(destinationCode);
            next.HeuristicEstimatePathLength = GetPath(source, qq);
            next.previous = null;
            next.PathLengthFromStart = 0;
            next.HeuristicEstimatePathLength = GetPath(source, AirlineData.GetAirPort(destinationCode));

            openSet.Enqueue(next);
            while (openSet.Count > 0)
            {
                var currentAirport = openSet.OrderBy( node => node.EstimateFullPathLength).First();
                if (currentAirport.Current.IATA == destinationCode)
                {
                    //Console.WriteLine("SUCCESS!");
                    Console.WriteLine("Time spended {0}", watch.ElapsedMilliseconds);
                    Console.WriteLine($"There is the way from {source.AirportName} to {AirlineData.GetAirPort(destinationCode).AirportName} ");
                    
                    return ReturnMinPath(currentAirport);
                    // ReturnMinPath();                   
                }
                var x = openSet.Dequeue(); // openSet.(currentAirport);
               // Console.WriteLine($"{x.IATA} ------- {x.Current.CountryName}");
                closedSet.Enqueue(x);
                //Console.WriteLine($"Number of neighbours ={currentAirport.adjacementList.Count}");
                foreach (var neighbourNode in currentAirport.adjacementList)
                {
                    var neighbourNode1 = AirlineData.GetNextStation(neighbourNode.IATA);
                    neighbourNode1.previous = currentAirport;
                    neighbourNode1.PathLengthFromStart = currentAirport.PathLengthFromStart + GetPath(currentAirport.Current, AirlineData.GetAirPort(destinationCode)); // виглядає дивно, працює швидко, результат коректний
                    //neighbourNode1.PathLengthFromStart = currentAirport.PathLengthFromStart + GetPath(currentAirport.Current, AirlineData.GetAirPort(neighbourNode.IATA)); // виглядає правильніше, працює довше, результат коректний
                    neighbourNode1.HeuristicEstimatePathLength = GetPath(neighbourNode1.Current, AirlineData.GetAirPort(destinationCode));

                    if (closedSet.Count( node => node.IATA == neighbourNode1.IATA) > 0)
                    {
                        continue;
                    }
                    var openNode = openSet.FirstOrDefault(node => node.IATA == neighbourNode1.IATA);
                    if (openNode == null)
                        openSet.Enqueue(neighbourNode1);

                    else
                    {
                        if (openNode.PathLengthFromStart > neighbourNode1.PathLengthFromStart)
                        {
                            openNode.previous = currentAirport;
                            openNode.PathLengthFromStart = neighbourNode1.PathLengthFromStart;
                        }
                    }
                    
                }
            }
            Console.WriteLine($"There is no way from airport with name {source.AirportName} to airport {AirlineData.GetAirPort(destinationCode).AirportName}");
            return null;
        }

        protected static internal List<NextAirport> ReturnMinPath(NextAirport pathNode) // відновлення шляху після алгоритму А*
        {
            var result = new List<NextAirport>();
            var currentNode = pathNode;
            result.Add(pathNode);
            while(currentNode != null)
            {
                result.Add(currentNode.previous);
                currentNode = currentNode.previous;
            }
            result.Reverse();
            result.RemoveAt(0);
            foreach (var x in result)
                Console.Write($" {x.IATA} ->>>");

            return result;

        }

    }

}


