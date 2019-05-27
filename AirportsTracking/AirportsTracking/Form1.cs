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
            InitializeComponent();
            AirlineData.LoadData();
            GMapProviders.GoogleMap.ApiKey = @"AIzaSyAPFEPeEVq_ECcRp6lcWzh-zpmyJG6nQKo";

            gMap.MapProvider = GMapProviders.GoogleMap;
            gMap.ShowCenter = false;
            gMap.Position = new PointLatLng(25, 35);

            List<String> citylist = MergeSortClass<String>.MergeSort(AirlineData.GetCityList());
            List<String> citylist2 = citylist.GetRange(0, citylist.Count);
            comboBoxCity1.DataSource = citylist;
            comboBoxCity2.DataSource = citylist2;
        }

        private void ShowAirportsInTwoCity(string city1, string city2)
        {
            List<int> PointColor = new List<int>();
            allAirports = AirlineData.ReturnListOfAirportsBy2City(city1, city2);
            for (int i = 0; i < allAirports.Count; i++)
            {
                PointLatLng point = new PointLatLng(allAirports[i].Latitude, allAirports[i].Longitude);
                if (allAirports[i].CityName == city1)
                {
                    GMapMarker marker = new GMarkerGoogle(point, GMarkerGoogleType.blue_dot);
                    marker.ToolTipText = Convert.ToString(i) + 'f';
                    marker.ToolTipMode = MarkerTooltipMode.Never;
                    overlay.Markers.Add(marker);
                }
                else
                {
                    GMapMarker marker = new GMarkerGoogle(point, GMarkerGoogleType.red_dot);
                    marker.ToolTipText = Convert.ToString(i) + 's';
                    marker.ToolTipMode = MarkerTooltipMode.Never;
                    overlay.Markers.Add(marker);
                }
            }
        }

        private void ShowCalculatedRoute(string IATA1, string IATA2)
        {
            if (AirlineData.FindIDByIATA(IATA1) == null || AirlineData.FindIDByIATA(IATA2) == null)
            {
                MessageBox.Show("Введіть коректні дані!");
                return;
            }

            airList = AStar.AStarMinPath(AirlineData.FindIDByIATA(IATA1), AirlineData.FindIDByIATA(IATA2));
            if (airList == null)
            {
                MessageBox.Show("На жаль такого маршруту не існує :(");
                return;
            }
            for (int i = 0; i < airList.Count; i++)
                allAirports.Add(airList[i].Current);

            for (int i = 0; i < allAirports.Count; i++)
            {
                List<PointLatLng> points = new List<PointLatLng>();
                if (i == allAirports.Count - 1)
                {
                    points.Add(new PointLatLng(allAirports[i].Latitude, allAirports[i].Longitude));

                    GMapMarker marker = new GMarkerGoogle(points[0], GMarkerGoogleType.red_dot);
                    marker.ToolTipText = Convert.ToString(i) + 's';
                    marker.ToolTipMode = MarkerTooltipMode.Never;
                    overlay.Markers.Add(marker);
                }
                else if (i == 0)
                {
                    points.Add(new PointLatLng(allAirports[i].Latitude, allAirports[i].Longitude));
                    points.Add(new PointLatLng(allAirports[i + 1].Latitude, allAirports[i + 1].Longitude));

                    GMapMarker marker = new GMarkerGoogle(points[0], GMarkerGoogleType.green_dot);
                    marker.ToolTipText = Convert.ToString(i) + 'f';
                    marker.ToolTipMode = MarkerTooltipMode.Never;
                    overlay.Markers.Add(marker);

                    var polygon = new GMapPolygon(points, "line");
                    overlay.Polygons.Add(polygon);
                }

                else
                {
                    points.Add(new PointLatLng(allAirports[i].Latitude, allAirports[i].Longitude));
                    points.Add(new PointLatLng(allAirports[i + 1].Latitude, allAirports[i + 1].Longitude));

                    GMapMarker marker = new GMarkerGoogle(points[0], GMarkerGoogleType.yellow_dot);
                    marker.ToolTipText = Convert.ToString(i) + 'm';
                    marker.ToolTipMode = MarkerTooltipMode.Never;
                    overlay.Markers.Add(marker);

                    var polygon = new GMapPolygon(points, "line");
                    overlay.Polygons.Add(polygon);
                }
            }
        }


        private void buttonCalc_Click(object sender, EventArgs e)
        {
            allAirports.Clear();
            overlay.Markers.Clear();
            overlay.Polygons.Clear();
            gMap.Overlays.Clear();

            string city1 = comboBoxCity1.Text;
            string city2 = comboBoxCity2.Text;

            //MAIN ACTION
            ShowAirportsInTwoCity(city1, city2);

            gMap.Overlays.Add(overlay);
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

            //MAIN ACTION
            ShowCalculatedRoute(IATA1, IATA2);

            gMap.Overlays.Add(overlay);
        }


        private void gMap_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            int k = int.Parse(item.ToolTipText[0].ToString());

            labelCountry.Text = allAirports[k].CountryName;
            labelCity.Text = allAirports[k].CityName;
            labelAirportName.Text = allAirports[k].AirportName;
            labelIATA.Text = allAirports[k].IATA;
            
            if (item.ToolTipText[1] == 'f')
                textBoxIATA1.Text = allAirports[k].IATA;
            else
                textBoxIATA2.Text = allAirports[k].IATA;
        }

    }
}
