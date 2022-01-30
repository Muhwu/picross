using System;

namespace Picross
{
    static class Program
    {
        static void Main(string[] args)
        {
            var picross = new Game.Picross();
            picross.Solve();
        }
    }
}