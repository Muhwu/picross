﻿using System;
using System.Linq;

namespace Picross.Game
{
    public class Picross
    {
        private Board _board;
        
        public Picross()
        {
            _board = new Board(15, 15);
        }

        public void Solve()
        {
            var changed = true;
            var i = 0;
            Console.Write("Preparing solution... ");
            while (_board.SolveIteration())
            {
                i++;
            }
            Console.WriteLine("Done.");
            _board.Print();
        }
    }
}