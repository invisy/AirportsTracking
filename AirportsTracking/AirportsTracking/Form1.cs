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

using Airports;

namespace AirportsTracking
{
    public partial class Form1 : Form
    {
        private List<NextAirport> airports; 

        public Form1()
        {
            GMapProviders.GoogleMap.ApiKey = @"AIzaSyAPFEPeEVq_ECcRp6lcWzh-zpmyJG6nQKo";
            InitializeComponent();
        }

        private void buttonCalc_Click(object sender, EventArgs e)
        {
            Graph.DijkstraMinPath("DME", "LAS");


            gMap.MapProvider = GMapProviders.GoogleMap;

            GMapOverlay polyOverlay = new GMapOverlay("polygons");

            List<PointLatLng> points = new List<PointLatLng>();
            points.Add(new PointLatLng(23, 1));
            points.Add(new PointLatLng(11, 42));
            points.Add(new PointLatLng(21, 58));

            GMapPolygon polygon = new GMapPolygon(points, "mypolygon");
            polygon.Fill = new SolidBrush(Color.FromArgb(50, Color.Red));
            polygon.Stroke = new Pen(Color.Red, 1);

            polyOverlay.Polygons.Add(polygon);
            gMap.Overlays.Add(polyOverlay);
        }
    }
}
