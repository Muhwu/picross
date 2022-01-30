using System;
using Picross.Tests;

namespace Picross
{
    static class Program
    {
        static void Main(string[] args)
        {
            //TestLegality.Run();
            
            var picross = new Game.Picross();
            picross.Solve();
        }
    }
}