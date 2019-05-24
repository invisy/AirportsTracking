using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

namespace AirportsTracking
{
    public partial class Form1 : Form
    {
        private List<NextAirport> airList = new List<NextAirport>();
        private List<Airport> allAirports = new List<Airport>();
        GMapOverlay overlay = new GMapOverlay("FoundAirports");

        public Form1()
        {
            AirlineData.LoadData();
            GMapProviders.GoogleMap.ApiKey = @"AIzaSyAPFEPeEVq_ECcRp6lcWzh-zpmyJG6nQKo";
            InitializeComponent();
        }

        private void buttonCalc_Click(object sender, EventArgs e)
        {
            gMap.MapProvider = GMapProviders.GoogleMap;

            allAirports.Clear();
            overlay.Markers.Clear();
            overlay.Polygons.Clear();
            gMap.Overlays.Clear();

            string city1 = textBoxCity1.Text;
            string city2 = textBoxCity2.Text;

            allAirports = AirlineData.ReturnListOfAirportsBy2City(city1, city2);
            for (int i = 0; i < allAirports.Count; i++)
            {
                PointLatLng point = new PointLatLng(allAirports[i].Latitude, allAirports[i].Longitude);
                if (allAirports[i].CityName == city1)
                {
                    GMapMarker marker = new GMarkerGoogle(point, GMarkerGoogleType.blue_dot);
                    marker.ToolTipText = Convert.ToString(i);
                    overlay.Markers.Add(marker);

                }
                else
                {
                    GMapMarker marker = new GMarkerGoogle(point, GMarkerGoogleType.red_dot);
                    marker.ToolTipText = Convert.ToString(i);
                    overlay.Markers.Add(marker);
                }               
            }          
            gMap.Overlays.Add(overlay);
        }

        private void gMap_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            labelCountry.Text = allAirports[Convert.ToInt32(item.ToolTipText)].CountryName;
            labelCity.Text = allAirports[Convert.ToInt32(item.ToolTipText)].CityName;
            labelAirportName.Text = allAirports[Convert.ToInt32(item.ToolTipText)].AirportName;
            labelIATA.Text = allAirports[Convert.ToInt32(item.ToolTipText)].IATA;
        }

        private void buttonRoute_Click(object sender, EventArgs e)
        {
            gMap.MapProvider = GMapProviders.GoogleMap;

            overlay.Markers.Clear();
            overlay.Polygons.Clear();
            gMap.Overlays.Clear();
            allAirports.Clear();

            string IATA1 = textBoxIATA1.Text;
            string IATA2 = textBoxIATA2.Text;
            if(AirlineData.FindIDByIATA(IATA1) == null || AirlineData.FindIDByIATA(IATA2) == null)
            {
                MessageBox.Show("Введіть коректні дані!");
                return;
            }

            airList = AStar.AStarMinPath(AirlineData.FindIDByIATA(IATA1), AirlineData.FindIDByIATA(IATA2));
            if(airList == null)
            {
                MessageBox.Show("На жаль такого маршруту не існує :(");
                return;
            }
            for (int i = 0; i < airList.Count; i++)
                allAirports.Add(airList[i].Current);


            //--------------------------------
            Airport[] airports = new Airport[2];
            airports[0] = new Airport();
            airports[1] = new Airport();

            airports[0].ID = "2188";
            airports[0].AirportName = "Dubai International Airport";
            airports[0].CityName = "Dubai";
            airports[0].CountryName = "United Arab Emirates";
            airports[0].IATA = "DXB";
            airports[0].Latitude = 25.2527999878;
            airports[0].Longitude = 55.3643989563;

            airports[1].ID = "3797";
            airports[1].AirportName = "John F Kennedy International Airport";
            airports[1].CityName = "New York";
            airports[1].CountryName = "United States";
            airports[1].IATA = "JKK";
            airports[1].Latitude = 40.63980103;
            airports[1].Longitude = -73.77890015;
            //--------------------------------

            for (int i = 0; i < allAirports.Count; i++)
            {
                List<PointLatLng> points = new List<PointLatLng>();
                if (i == allAirports.Count - 1)
                {
                    points.Add(new PointLatLng(allAirports[i].Latitude, allAirports[i].Longitude));

                    GMapMarker marker = new GMarkerGoogle(points[0], GMarkerGoogleType.red_dot);
                    marker.ToolTipText = Convert.ToString(i);
                    overlay.Markers.Add(marker);
                }
                else if (i == 0)
                {
                    points.Add(new PointLatLng(allAirports[i].Latitude, allAirports[i].Longitude));
                    points.Add(new PointLatLng(allAirports[i + 1].Latitude, allAirports[i + 1].Longitude));

                    GMapMarker marker = new GMarkerGoogle(points[0], GMarkerGoogleType.green_dot);
                    marker.ToolTipText = Convert.ToString(i);
                    overlay.Markers.Add(marker);

                    var polygon = new GMapPolygon(points, "line");
                    overlay.Polygons.Add(polygon);
                }
                
                else
                {
                    points.Add(new PointLatLng(allAirports[i].Latitude, allAirports[i].Longitude));
                    points.Add(new PointLatLng(allAirports[i + 1].Latitude, allAirports[i + 1].Longitude));

                    GMapMarker marker = new GMarkerGoogle(points[0], GMarkerGoogleType.yellow_dot);
                    marker.ToolTipText = Convert.ToString(i);
                    overlay.Markers.Add(marker);

                    var polygon = new GMapPolygon(points, "line");
                    overlay.Polygons.Add(polygon);
                }
            }
            gMap.Overlays.Add(overlay);
        }

    }
}
