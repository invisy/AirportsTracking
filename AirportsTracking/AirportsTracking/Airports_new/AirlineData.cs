using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportsTracking
{
    class AirlineData
    {
        const string airports_path = "Resources/airports.txt";
        const string routs_path = "Resources/routes.txt";
        protected internal static Dictionary<string, Airport> DictOfAirports { get; set; }
        static public void LoadData()
        {
            try
            {
                DictOfAirports = new Dictionary<string, Airport>();
                string[] Airports_str = GetAllInfoFromFile(airports_path);
                string[] Routes_str = GetAllInfoFromFile(routs_path);

                foreach (var line in Airports_str)
                {
                    string[] temp = ParseLine(line);
                    LinkedList<Route> routes = new LinkedList<Route>();
                    var airport = new Airport(temp, routes);
                    DictOfAirports.Add(airport.ID, airport);
                }
                foreach (var line in Routes_str)
                {
                    string[] temp = ParseLine(line);
                    if (temp[3] == "\\N")
                        temp[3] = FindIDByIATA(temp[2]);
                    if (temp[5] == "\\N")
                        temp[5] = FindIDByIATA(temp[4]);
                    if (temp[3] != null || temp[5] != null)
                    {
                        Route route = new Route(temp[5], temp[7]);
                        if (DictOfAirports.ContainsKey(temp[3]) && DictOfAirports.ContainsKey(temp[5]))
                            DictOfAirports[temp[3]].routes.AddLast(route);
                    }
                }
            }
            catch (ArgumentException a)
            {
                Console.WriteLine(a.Message);
            }
            // return null;
        }
        protected internal static string[] GetAllInfoFromFile(string resource)
        {
            try
            {
                using (var textFile = System.IO.File.OpenText(resource))
                {
                    string[] buffer = textFile.ReadToEnd().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    return buffer;
                }

            }
            catch (Exception a)
            {
                Console.WriteLine(a.Message);
            }
            return null;

        }

        protected internal static string[] ParseLine(string str)
        {
            List<string> result = new List<string>();
            int curr_symbol = 0;
            bool flag = false;
            string tmp = String.Empty;
            while(curr_symbol < str.Length)
            {
                if(!flag)
                {
                    if (str[curr_symbol] == '\"')
                        flag = true;
                    else if (str[curr_symbol] != ',')
                        tmp += str[curr_symbol];
                    else
                    {
                        result.Add(tmp);
                        tmp = String.Empty;
                    }
                }
                else
                {
                    if(str[curr_symbol] == '\"' && (curr_symbol + 1 >= str.Length || str[curr_symbol + 1] == ','))
                        flag = false;
                    else
                        tmp += str[curr_symbol];
                }
                curr_symbol++;
            }
            return result.ToArray();
        }

        protected internal static string FindIDByIATA(string code)
        {
            foreach(KeyValuePair<String, Airport> curr in DictOfAirports)
            {
                if (curr.Value.IATA == code)
                    return curr.Key;
            }
            return null;
        }

        protected internal static Airport GetAirPort(string code)
        {
            return DictOfAirports[code];
        }

        protected internal static NextAirport GetNextStation(string stationID)
        {
            Airport curr = GetAirPort(stationID);
            if (curr != null)
            {
                NextAirport nextAirport = new NextAirport(curr, curr.AirportName, int.MaxValue, curr.routes);
                return nextAirport;
            }
            else
                return null;
        }

        protected static internal List<Airport> ReturnListOfAirportsBy2City(string city1, string city2)
        {
            var list = new List<Airport>();
            foreach (var airport in DictOfAirports)
            {
                if (airport.Value.CityName == city1 || airport.Value.CityName == city2)
                {
                    list.Add(airport.Value);
                }
            }
            return list;
        }
    }
}
