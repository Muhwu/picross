using System;
using System.Collections.Generic;
using System.Linq;
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

        private int[][] _horizontallyAligned;
        private int[][] _verticallyAligned;
        
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

            _horizontallyAligned = new int[width][];
            _verticallyAligned = new int[height][];

            for (var i = 0; i < width; i++)
            {
                _horizontallyAligned[i] = new int[MaxBlocks];
                _verticallyAligned[i] = new int[MaxBlocks];
            }

            CreateProblem();
        }

        public bool SolveIteration()
        {
            for (var y = 0; y < _height; y++)
            {
                var validOnes = GenerateRows(_horizontallyAligned[y], _squares[y]);
                var validCompatible = new List<string>();
                foreach (var valid in validOnes)
                {
                    var merged = Merge(_squares[y], valid);
                    if (merged != "")
                    {
                        validCompatible.Add(merged);
                    }
                }

                if (validCompatible.Count == 0) continue;

                // Check for present in all valid
                CalculateInevitables(validCompatible, out var counters1, out var counters2);

                for (var x = 0; x < _width; x++)
                {
                    if (counters1[x] == validCompatible.Count)
                    {
                        Console.WriteLine($"{counters1[x]} ones out of {validCompatible.Count} for ({x},{y})");
                        Set(x, y, "1");
                    }

                    if (counters2[x] == validCompatible.Count)
                    {
                        Console.WriteLine($"{counters2[x]} twos out of {validCompatible.Count} for ({x},{y})");
                        Set(x, y, "2");
                    }
                }
            }
            
            return true;
        }

        private void CalculateInevitables(List<string> validCompatible, out int[] counters1, out int[] counters2)
        {
            counters1 = new int[_width];
            counters2 = new int[_width];
            foreach (var vc in validCompatible)
            {
                for (var i = 0; i < vc.Length; i++)
                {
                    counters1[i] += vc[i] == '1' ? 1 : 0;
                    counters2[i] += vc[i] == '2' ? 1 : 0;
                }
            }
        }

        private void Set(int x, int y, string value)
        {
            _squaresVertical[y] = _squaresVertical[y].Remove(x, 1).Insert(x, value);
            _squares[x] = _squares[x].Remove(y, 1).Insert(y, value);
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
                    PrintBlockNumber(index, _verticallyAligned[index][block]);
                }
                Console.Write("\n");
            }
            
            for (var y = 0; y < _height; y++)
            {
                for (var bl = MaxBlocks - 1; bl >= 0; bl--)
                {
                    PrintBlockNumber(y, _horizontallyAligned[y][bl]);
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
                    Console.ForegroundColor = ConsoleColor.Red;
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
            _verticallyAligned[0][0] = 2;

            _verticallyAligned[1][0] = 1;
            _verticallyAligned[1][1] = 2;

            _verticallyAligned[2][0] = 1;
            _verticallyAligned[2][1] = 2;
            _verticallyAligned[2][2] = 1;

            _verticallyAligned[3][0] = 1;
            _verticallyAligned[3][1] = 1;
            _verticallyAligned[3][2] = 1;
            _verticallyAligned[3][3] = 2;

            _verticallyAligned[4][0] = 1;
            _verticallyAligned[4][1] = 4;
            _verticallyAligned[4][2] = 1;
            _verticallyAligned[4][3] = 1;

            _verticallyAligned[5][0] = 6;
            _verticallyAligned[5][1] = 1;
            _verticallyAligned[5][2] = 2;

            _verticallyAligned[6][0] = 1;
            _verticallyAligned[6][1] = 2;
            _verticallyAligned[6][2] = 1;
            _verticallyAligned[6][3] = 1;

            _verticallyAligned[7][0] = 2;
            _verticallyAligned[7][1] = 2;
            _verticallyAligned[7][2] = 1;

            _verticallyAligned[8][0] = 2;
            _verticallyAligned[8][1] = 3;
            _verticallyAligned[8][2] = 1;
            _verticallyAligned[8][3] = 1;

            _verticallyAligned[9][0] = 2;
            _verticallyAligned[9][1] = 3;
            _verticallyAligned[9][2] = 2;

            _verticallyAligned[10][0] = 11;
            _verticallyAligned[10][1] = 1;
            _verticallyAligned[10][2] = 1;

            _verticallyAligned[11][0] = 2;
            _verticallyAligned[11][1] = 5;
            _verticallyAligned[11][2] = 1;

            _verticallyAligned[12][0] = 1;
            _verticallyAligned[12][1] = 1;
            _verticallyAligned[12][2] = 1;
            _verticallyAligned[12][3] = 3;

            _verticallyAligned[13][0] = 1;
            _verticallyAligned[13][1] = 1;
            _verticallyAligned[13][2] = 1;

            _verticallyAligned[14][0] = 2;
            
            // X
            _horizontallyAligned[0][0] = 4;
            
            _horizontallyAligned[1][0] = 2;
            _horizontallyAligned[1][1] = 2;
            
            _horizontallyAligned[2][0] = 1;
            _horizontallyAligned[2][1] = 1;
            _horizontallyAligned[2][2] = 1;
            _horizontallyAligned[2][3] = 1;
            
            _horizontallyAligned[3][0] = 1;
            _horizontallyAligned[3][1] = 1;
            _horizontallyAligned[3][2] = 2;
            
            _horizontallyAligned[4][0] = 5;
            _horizontallyAligned[4][1] = 3;
            
            _horizontallyAligned[5][0] = 3;
            _horizontallyAligned[5][1] = 1;
            _horizontallyAligned[5][2] = 1;
            _horizontallyAligned[5][3] = 2;
            
            _horizontallyAligned[6][0] = 3;
            _horizontallyAligned[6][1] = 2;
            _horizontallyAligned[6][2] = 2;
            _horizontallyAligned[6][3] = 1;
            
            _horizontallyAligned[7][0] = 2;
            _horizontallyAligned[7][1] = 1;
            _horizontallyAligned[7][2] = 2;
            _horizontallyAligned[7][3] = 3;
            
            _horizontallyAligned[8][0] = 1;
            _horizontallyAligned[8][1] = 2;
            _horizontallyAligned[8][2] = 3;
            _horizontallyAligned[8][3] = 1;
            
            _horizontallyAligned[9][0] = 1;
            _horizontallyAligned[9][1] = 4;
            _horizontallyAligned[9][2] = 1;

            _horizontallyAligned[10][0] = 1;
            _horizontallyAligned[10][1] = 1;
            _horizontallyAligned[10][2] = 5;

            _horizontallyAligned[11][0] = 3;
            _horizontallyAligned[11][1] = 1;
            _horizontallyAligned[11][2] = 2;

            _horizontallyAligned[12][0] = 4;
            _horizontallyAligned[12][1] = 1;

            _horizontallyAligned[13][0] = 2;
            _horizontallyAligned[13][1] = 2;
            _horizontallyAligned[13][2] = 1;

            _horizontallyAligned[14][0] = 1;
            _horizontallyAligned[14][1] = 2;
        }
    }
}