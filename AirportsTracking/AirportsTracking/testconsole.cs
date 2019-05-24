using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace AirportsTracking
{
    class testConsole
    {
        [DllImport("kernel32.dll")]
        public static extern Boolean AllocConsole();
        [DllImport("kernel32.dll")]
        public static extern Boolean FreeConsole();

        static public void MainDebug()
        {
            //Console.WriteLine("Just Text!");
            ALGORITMxD();
            //Write your code HERE
        }

        static void ALGORITMxD()
        {
            //Console.Read();
            Console.WriteLine("     Dijkstra:");
            Graph.DijkstraMinPath("4029", "3877");
            Console.WriteLine("     A*:");
            AStar.AStarMinPath("4029", "3877");

            //AStar.AStarMinPath("4029", "3877");
            //AStar.AStarMinPath("3797", "3339");
            //AStar.AStarMinPath("1", "3"); //GKA HGU
            //Graph.DijkstraMinPath("1", "8");
            Console.Read();
        }
    }
}
