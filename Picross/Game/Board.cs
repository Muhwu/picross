using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace Picross.Game
{
    public class Board
    {
        private const int MaxBlocks = 5;
        
        private int _width;
        private int _height;

        private string[] _squares;
        private string[] _squaresVertical;

        private int[][] _xBlocks;
        private int[][] _yBlocks;
        
        public Board(int width, int height)
        {
            _width = width;
            _height = height;
            _squares = new string[_height];
            _squaresVertical = new string[_width];
            for (var i = 0; i < _height; i++)
            {
                _squares[i] = string.Concat(Enumerable.Repeat("0", _width));
            }

            for (var j = 0; j < _width; j++)
            {
                _squaresVertical[j] = string.Concat(Enumerable.Repeat("0", _height));
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
            for (var y = 0; y < _height; y++)
            {
                var validOnes = GenerateRows(_xBlocks[y], _squares[y]);
                var validCompatible = new List<string>();
                foreach (var valid in validOnes)
                {
                    var merged = Merge(_squares[y], valid);
                    if (merged != "")
                    {
                        validCompatible.Add(merged);
                        Console.WriteLine(merged);
                    }
                }

                break;
            }
            
            return true;
        }

        public string Merge(string existing, string proposal)
        {
            if (existing.Length != proposal.Length) return "";

            var merged = "";
            for (var i = 0; i < existing.Length; i++)
            {
                if (existing[i] == proposal[i])
                {
                    merged += existing[i];
                    continue;
                }

                if (existing[i] == '0')
                {
                    merged += proposal[i];
                    continue;
                }

                if (proposal[i] == '0')
                {
                    merged += existing[i];
                    continue;
                }

                return "";
            }

            return merged;
        }

        public bool IsLegal(string row, int[] blocks)
        {
            if (row.Length != _width) return false;

            var regEx = "^[02]*";
            for (var index = 0; index < blocks.Length; index++)
            {
                if (index > 0)
                {
                    regEx += "[02]+";
                }
                var b = blocks[index];
                regEx += $"1{{{b}}}";
            }

            regEx += "[02]*$";

            return Regex.IsMatch(row, regEx);
        }

        private List<string> GenerateRow(int rowLength, int[] blocks, int depth = 0)
        {
            var rows = new List<string>();

            for (var startIndex = 0; startIndex < rowLength; startIndex++)
            {
                var lineSegment = "";
                lineSegment += string.Concat(Enumerable.Repeat("0", startIndex));
                if (startIndex > 0)
                {
                    lineSegment = lineSegment.Remove(lineSegment.Length - 1, 1) + "2";
                }
                lineSegment += string.Concat(Enumerable.Repeat("1", blocks[0]));
                
                var remainingRow = rowLength - startIndex - blocks[0];
                if (remainingRow < 0) continue;
                
                if (blocks.Length == 1 || blocks[1] == 0)
                {
                    if (remainingRow > 0)
                    {
                        lineSegment += "2";
                    }

                    if (remainingRow - 1 > 0)
                        lineSegment += string.Concat(Enumerable.Repeat("0", remainingRow - 1));
                    
                    rows.Add(lineSegment);
                }
                else
                {
                    var subrows = GenerateRow(remainingRow - 1, blocks.Skip(1).ToArray(), depth + 1);
                    foreach (var sr in subrows)
                    {
                        if (depth == 0 && !IsLegal(lineSegment + "2" + sr, blocks)) continue;
                        rows.Add(lineSegment + "2" + sr);
                    }
                }
            }

            return rows;
        }
        
        public List<string> GenerateRows(in int[] remainingBlocks, string row)
        {
            var rows = GenerateRow(row.Length, remainingBlocks);
            return rows;
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

            _xBlocks[14][0] = 1;
            _xBlocks[14][1] = 2;
        }
    }
}