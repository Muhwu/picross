using System;
using System.Diagnostics;
using Picross.Tests;

namespace Picross
{
    static class Program
    {
        private static void RunTests()
        {
            TestLegality.Run();
        }
        
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            
            var picross = new Game.Picross();
            var sw = Stopwatch.StartNew();
            picross.Solve();
            Console.WriteLine($"\nSolved in {sw.ElapsedMilliseconds} milliseconds");
        }
    }
}