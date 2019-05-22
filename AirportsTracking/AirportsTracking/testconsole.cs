using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Airports;

class testConsole
{
    [DllImport("kernel32.dll")]
    public static extern Boolean AllocConsole();
    [DllImport("kernel32.dll")]
    public static extern Boolean FreeConsole();
    
    static public void MainDebug()
    {
        Console.WriteLine("Just Text!");
        ALGORITMxD();
        //Write your code HERE
    }

    static void ALGORITMxD()
    {
        AStar.AStarMinPath("PSX", "PBF");
    }
}
