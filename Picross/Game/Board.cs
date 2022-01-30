using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Picross.Game
{
    public class Board
    {
        private const int MaxBlocks = 5;
        
        private int _width;
        private int _height;

        private string[] _squares;

        private int[][] _xBlocks;
        private int[][] _yBlocks;
        
        public Board(int width, int height)
        {
            _width = width;
            _height = height;
            _squares = new string[width];
            for (var i = 0; i < _squares.Length; i++)
            {
                _squares[i] = string.Concat(Enumerable.Repeat("0", _height));
            }

            _xBlocks = new int[width][];
            _yBlocks = new int[height][];

            for (var i = 0; i < width; i++)
            {
                _xBlocks[i] = new int[MaxBlocks];
                _yBlocks[i] = new int[MaxBlocks];
            }

            CreateProblem();
        }

        public bool SolveIteration()
        {
            for (var x = 0; x < _width; x++)
            {
                //EvaluateRow(_xBlocks[x], _squares[x]);
            }
            
            return true;
        }

        public bool IsLegal(string row, int[] blocks)
        {
            return row.Length <= _width;
        }
        
        public List<string> GenerateRows(in int[] remainingBlocks, string row)
        {
            var rows = new List<string>();
            
            for (var b1 = 0; b1 < row.Length; b1++)
            {
                var b1Line = "";
                b1Line += string.Concat(Enumerable.Repeat("0", b1));
                b1Line += string.Concat(Enumerable.Repeat("1", remainingBlocks[0]));

                // Place B1
                for (var b2 = b1 + remainingBlocks[0] + 1; b2 < row.Length; b2++)
                {
                    var b2Line = "";
                    b2Line += string.Concat(Enumerable.Repeat("0", b2 - b1 - remainingBlocks[0]));
                    b2Line += string.Concat(Enumerable.Repeat("1", remainingBlocks[1]));
                    
                    for (var b3 = b2 + remainingBlocks[1] + 1; b3 < row.Length; b3++)
                    {
                        var b3Line = "";
                        b3Line += string.Concat(Enumerable.Repeat("0", b3 - b2 - remainingBlocks[1]));
                        b3Line += string.Concat(Enumerable.Repeat("1", remainingBlocks[2]));

                        for (var b4 = b3 + remainingBlocks[2] + 1; b4 < row.Length; b4++)
                        {
                            var b4Line = "";
                            
                            b4Line += string.Concat(Enumerable.Repeat("0", b4 - b3 - remainingBlocks[2]));
                            b4Line += string.Concat(Enumerable.Repeat("1", remainingBlocks[3]));

                            if (remainingBlocks.Length == 1)
                            {
                                var line = b1Line + b2Line + b3Line + b4Line;
                                line += string.Concat(Enumerable.Repeat("0", Math.Max(0, row.Length - line.Length)));

                                if (IsLegal(line, remainingBlocks))
                                {
                                    Console.WriteLine(line);
                                    // Yay
                                    rows.Add(row);
                                }
                            }
                            else
                            {
                                //
                            }
                        }
                    }    
                }
            }
            Console.WriteLine($"Found {rows.Count} rows.");
            return rows;
        }

        private bool CanPlaceBlock(int length, int startIndex, string row)
        {
            for (var b = 0; b < length; b++)
            {
                if (startIndex + b >= row.Length) return false;
                if (row[startIndex + b] != '0')
                {
                    return false;
                }
            }

            return true;
        }

        private int GetBlockCount(in int[] blocks)
        {
            var result = 0;
            for (var i = 0; i < MaxBlocks; i++)
            {
                if (blocks[i] > 0)
                {
                    result++;
                }
            }

            return result;
        }
        
        
        public void Print()
        {
            Console.OutputEncoding = Encoding.UTF8;

            for (var block = MaxBlocks - 1; block >= 0; block--)
            {
                for (var space = 0; space < MaxBlocks; space++)
                {
                    Console.Write("   ");
                }
                
                for (var index = 0; index < _width; index++)
                {
                    PrintBlockNumber(index, _yBlocks[index][block]);
                }
                Console.Write("\n");
            }
            
            for (var y = 0; y < _height; y++)
            {
                for (var bl = MaxBlocks - 1; bl >= 0; bl--)
                {
                    PrintBlockNumber(y, _xBlocks[y][bl]);
                }
                for (var x = 0; x < _width; x++)
                {
                    PrintSquare(_squares[x][y]);
                }
                Console.Write("\n");
            }
        }

        private static void PrintSquare(char square)
        {
            switch (square)
            {
                case '0':
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("██ ");
                    break;
                case '2':
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("██ ");
                    break;
                case '1':
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("██ ");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void PrintBlockNumber(int index, int block)
        {
            if (block > 0)
            {
                Console.Write((block < 10 ? block + " " : block) + " ");
            }
            else
            {
                Console.Write("   ");
            }
        }

        private void CreateProblem()
        {
            // Y
            _yBlocks[0][0] = 2;

            _yBlocks[1][0] = 1;
            _yBlocks[1][1] = 2;

            _yBlocks[2][0] = 1;
            _yBlocks[2][1] = 2;
            _yBlocks[2][2] = 1;

            _yBlocks[3][0] = 1;
            _yBlocks[3][1] = 1;
            _yBlocks[3][2] = 1;
            _yBlocks[3][3] = 2;

            _yBlocks[4][0] = 1;
            _yBlocks[4][1] = 4;
            _yBlocks[4][2] = 1;
            _yBlocks[4][3] = 1;

            _yBlocks[5][0] = 6;
            _yBlocks[5][1] = 1;
            _yBlocks[5][2] = 2;

            _yBlocks[6][0] = 1;
            _yBlocks[6][1] = 2;
            _yBlocks[6][2] = 1;
            _yBlocks[6][3] = 1;

            _yBlocks[7][0] = 2;
            _yBlocks[7][1] = 2;
            _yBlocks[7][2] = 1;

            _yBlocks[8][0] = 2;
            _yBlocks[8][1] = 3;
            _yBlocks[8][2] = 1;
            _yBlocks[8][3] = 1;

            _yBlocks[9][0] = 2;
            _yBlocks[9][1] = 3;
            _yBlocks[9][2] = 2;

            _yBlocks[10][0] = 11;
            _yBlocks[10][1] = 1;
            _yBlocks[10][2] = 1;

            _yBlocks[11][0] = 2;
            _yBlocks[11][1] = 5;
            _yBlocks[11][2] = 1;

            _yBlocks[12][0] = 1;
            _yBlocks[12][1] = 1;
            _yBlocks[12][2] = 1;
            _yBlocks[12][3] = 3;

            _yBlocks[13][0] = 1;
            _yBlocks[13][1] = 1;
            _yBlocks[13][2] = 1;

            _yBlocks[14][0] = 2;
            
            // X
            _xBlocks[0][0] = 4;
            
            _xBlocks[1][0] = 2;
            _xBlocks[1][1] = 2;
            
            _xBlocks[2][0] = 1;
            _xBlocks[2][1] = 1;
            _xBlocks[2][2] = 1;
            _xBlocks[2][3] = 1;
            
            _xBlocks[3][0] = 1;
            _xBlocks[3][1] = 1;
            _xBlocks[3][2] = 2;
            
            _xBlocks[4][0] = 5;
            _xBlocks[4][1] = 3;
            
            _xBlocks[5][0] = 3;
            _xBlocks[5][1] = 1;
            _xBlocks[5][2] = 1;
            _xBlocks[5][3] = 2;
            
            _xBlocks[6][0] = 3;
            _xBlocks[6][1] = 2;
            _xBlocks[6][2] = 2;
            _xBlocks[6][3] = 1;
            
            _xBlocks[7][0] = 2;
            _xBlocks[7][1] = 1;
            _xBlocks[7][2] = 2;
            _xBlocks[7][3] = 3;
            
            _xBlocks[8][0] = 1;
            _xBlocks[8][1] = 2;
            _xBlocks[8][2] = 3;
            _xBlocks[8][3] = 1;
            
            _xBlocks[9][0] = 1;
            _xBlocks[9][1] = 4;
            _xBlocks[9][2] = 1;

            _xBlocks[10][0] = 1;
            _xBlocks[10][1] = 1;
            _xBlocks[10][2] = 5;

            _xBlocks[11][0] = 3;
            _xBlocks[11][1] = 1;
            _xBlocks[11][2] = 2;

            _xBlocks[12][0] = 4;
            _xBlocks[12][1] = 1;

            _xBlocks[13][0] = 2;
            _xBlocks[13][1] = 2;
            _xBlocks[13][2] = 1;

            _xBlocks[14][0] = 2;
            _xBlocks[14][1] = 1;
        }
    }
}