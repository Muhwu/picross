using System;
using System.Diagnostics;

namespace Picross
{
    static class Program
    {
        // Y
        private static readonly int[][] VerticallyAlignedBlocks = {
            new []{ 2 },
            new []{ 1, 2 },
            new []{ 1, 2, 1 },
            new []{ 1, 1, 1, 2 },
            new []{ 1, 4, 1, 1},
            new []{ 6, 1, 2},
            new []{ 1, 2, 1, 1 },
            new []{ 2, 2, 1 },
            new []{ 2, 3, 1, 1 },
            new []{ 2, 3, 2 },
            new []{ 11, 1, 1 },
            new []{ 2, 5, 1 },
            new []{ 1, 1, 1, 3 },
            new []{ 1, 1, 1 },
            new []{ 2 }
        };
            
        // X
        private static readonly int[][] HorizontallyAlignedBlocks = {
            new []{ 4 },
            new []{ 2, 2 },
            new []{ 1, 1, 1, 1 },
            new []{ 1, 1, 2 },
            new []{ 5, 3 },
            new []{ 3, 1, 1, 2 },
            new []{ 3, 2, 2, 1 },
            new []{ 2, 1, 2, 3 },
            new []{ 1, 2, 3, 1 },
            new []{ 1, 4, 1 },
            new []{ 1, 1, 5 },
            new []{ 3, 1, 2 },
            new []{ 4, 1 },
            new []{ 2, 2, 1 },
            new []{ 1, 2 }
        };
        
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            
            var picross = new Game.Picross();
            picross.Initialize(VerticallyAlignedBlocks, HorizontallyAlignedBlocks);
            
            var sw = Stopwatch.StartNew();
            picross.Solve();
            sw.Stop();
            
            Console.WriteLine($"\nSolved in {sw.ElapsedMilliseconds} milliseconds");
        }
    }
}