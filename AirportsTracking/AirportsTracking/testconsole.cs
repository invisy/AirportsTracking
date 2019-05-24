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
            AirlineData.LoadData();               //ОБОВ'ЯЗКОВО!
             /*Console.WriteLine("     A*:");
             AStar.AStarMinPath("3462", "246");
             Console.WriteLine("     Dijkstra:");
             Graph.DijkstraMinPath("3462", "246"); //   PHX OUA interesting test  3462 ->>>3682 ->>>302 ->>>246 ->>>
             */
            /*Console.WriteLine("     A*:");
            AStar.AStarMinPath("340", "13671");
            Console.WriteLine("     Dijkstra:");
            Graph.DijkstraMinPath("340", "13671"); // frankfurt - "Noonkanbah Airport","Noonkanbah","Australia"*/


            // "340", "3877" straight
            //AStar.AStarMinPath("4029", "3877");
            //AStar.AStarMinPath("3797", "3339");
            //AStar.AStarMinPath("1", "3"); //GKA HGU
            //Graph.DijkstraMinPath("1", "8");
            Console.Read();
        }
    }
}
