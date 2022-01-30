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

        private string[] _boardHorizontallyAligned;
        private string[] _boardVerticallyAligned;

        private int[][] _horizontallyAlignedBlocks;
        private int[][] _verticallyAlignedBlocks;
        
        public Board(int width, int height)
        {
            _width = width;
            _height = height;
            _boardHorizontallyAligned = new string[_height];
            _boardVerticallyAligned = new string[_width];
            for (var i = 0; i < _height; i++)
            {
                _boardHorizontallyAligned[i] = string.Concat(Enumerable.Repeat("0", _width));
            }

            for (var j = 0; j < _width; j++)
            {
                _boardVerticallyAligned[j] = string.Concat(Enumerable.Repeat("0", _height));
            }

            _horizontallyAlignedBlocks = new int[width][];
            _verticallyAlignedBlocks = new int[height][];

            for (var i = 0; i < width; i++)
            {
                _horizontallyAlignedBlocks[i] = new int[MaxBlocks];
                _verticallyAlignedBlocks[i] = new int[MaxBlocks];
            }

            CreateProblem();
        }

        public bool SolveIteration()
        {
            for (var y = 0; y < _height; y++)
            {
                var validOnes = GenerateRows(_horizontallyAlignedBlocks[y], _boardHorizontallyAligned[y]);
                var validCompatible = GetValidCompatible(validOnes, _boardHorizontallyAligned[y], _horizontallyAlignedBlocks[y]);
                if (validCompatible.Count == 0) continue;
            
                // Check for present in all valid
                CalculateInevitables(validCompatible, out var counters1, out var counters2);
            
                for (var x = 0; x < _width; x++)
                {
                    SetIfInevitable(x, y, Axis.Horizontal, validCompatible, counters1, counters2);
                }
            }
            
            for (var x = 0; x < _width; x++)
            {
                var validOnes = GenerateRows(_verticallyAlignedBlocks[x], _boardVerticallyAligned[x]);
                var validCompatible = GetValidCompatible(validOnes, _boardVerticallyAligned[x], _verticallyAlignedBlocks[x]);
                if (validCompatible.Count == 0) continue;

                // Check for present in all valid
                CalculateInevitables(validCompatible, out var counters1, out var counters2);

                for (var y = 0; y < _height; y++)
                {
                    SetIfInevitable(x, y, Axis.Vertical, validCompatible, counters1, counters2);
                }
            }
            
            return true;
        }

        private List<string> GetValidCompatible(List<string> validOnes, string boardAxis, int[] blocks)
        {
            var validCompatible = new List<string>();
            foreach (var valid in validOnes)
            {
                var merged = Merge(boardAxis, valid);
                if (merged != "" && IsLegal(merged, blocks))
                {
                    validCompatible.Add(merged);
                }
            }

            return validCompatible;
        }

        private enum Axis
        {
            Horizontal,
            Vertical
        }
        private void SetIfInevitable(int x, int y, Axis axis, List<string> validCompatible, int[] counters1, int[] counters2)
        {
            var index = axis == Axis.Horizontal ? x : y;
            if (counters1[index] == validCompatible.Count)
            {
                Console.WriteLine($"{counters1[index]} ones out of {validCompatible.Count} for ({x},{y})");
                Set(x, y, "1");
            }
            else if (counters2[index] == validCompatible.Count)
            {
                Console.WriteLine($"{counters2[index]} twos out of {validCompatible.Count} for ({x},{y})");
                Set(x, y, "2");
            }
        }

        private void LogError(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(error);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private void CalculateInevitables(List<string> validCompatible, out int[] counters1, out int[] counters2)
        {
            counters1 = new int[validCompatible[0].Length];
            counters2 = new int[validCompatible[0].Length];
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
            _boardVerticallyAligned[x] = _boardVerticallyAligned[x].Remove(y, 1).Insert(y, value);
            _boardHorizontallyAligned[y] = _boardHorizontallyAligned[y].Remove(x, 1).Insert(x, value);
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

            //Console.WriteLine(string.Join("\n", rows));
            //Console.WriteLine($"{rows.Count} permutations");
            
            return rows;
        }
        
        public List<string> GenerateRows(in int[] remainingBlocks, string row)
        {
            var blocks = remainingBlocks.ToArray();
            var rows = GenerateRow(row.Length, blocks);
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
                    PrintBlockNumber(_verticallyAlignedBlocks[index].Length > block ? _verticallyAlignedBlocks[index][block] : 0);
                }
                Console.Write("\n");
            }
            
            for (var y = 0; y < _height; y++)
            {
                for (var bl = MaxBlocks - 1; bl >= 0; bl--)
                {
                    PrintBlockNumber(_horizontallyAlignedBlocks[y].Length > bl ? _horizontallyAlignedBlocks[y][bl] : 0);
                }
                for (var x = 0; x < _width; x++)
                {
                    PrintSquare(_boardHorizontallyAligned[_height - y - 1][x]);
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

        private void PrintBlockNumber(int block)
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
            _verticallyAlignedBlocks[0] = new []{ 2 };
            _verticallyAlignedBlocks[1] = new []{ 1, 2 };
            _verticallyAlignedBlocks[2] = new []{ 1, 2, 1 };
            _verticallyAlignedBlocks[3] = new []{ 1, 1, 1, 2 };
            _verticallyAlignedBlocks[4] = new []{ 1, 4, 1, 1};
            _verticallyAlignedBlocks[5] = new []{ 6, 1, 2};
            _verticallyAlignedBlocks[6] = new []{ 1, 2, 1, 1 };
            _verticallyAlignedBlocks[7] = new []{ 2, 2, 1 };
            _verticallyAlignedBlocks[8] = new []{ 2, 3, 1, 1 };
            _verticallyAlignedBlocks[9] = new []{ 2, 3, 2 };
            _verticallyAlignedBlocks[10] = new []{ 11, 1, 1 };
            _verticallyAlignedBlocks[11] = new []{ 2, 5, 1 };
            _verticallyAlignedBlocks[12] = new []{ 1, 1, 1, 3 };
            _verticallyAlignedBlocks[13] = new []{ 1, 1, 1 };
            _verticallyAlignedBlocks[14] = new []{ 2 };
            
            // X
            _horizontallyAlignedBlocks[0] = new []{ 4 };
            _horizontallyAlignedBlocks[1] = new []{ 2, 2 };
            _horizontallyAlignedBlocks[2] = new []{ 1, 1, 1, 1 };
            _horizontallyAlignedBlocks[3] = new []{ 1, 1, 2 };
            _horizontallyAlignedBlocks[4] = new []{ 5, 3 };
            _horizontallyAlignedBlocks[5] = new []{ 3, 1, 1, 2 };
            _horizontallyAlignedBlocks[6] = new []{ 3, 2, 2, 1 };
            _horizontallyAlignedBlocks[7] = new []{ 2, 1, 2, 3 };
            _horizontallyAlignedBlocks[8] = new []{ 1, 2, 3, 1 };
            _horizontallyAlignedBlocks[9] = new []{ 1, 4, 1 };
            _horizontallyAlignedBlocks[10] = new []{ 1, 1, 5 };
            _horizontallyAlignedBlocks[11] = new []{ 3, 1, 2 };
            _horizontallyAlignedBlocks[12] = new []{ 4, 1 };
            _horizontallyAlignedBlocks[13] = new []{ 2, 2, 1 };
            _horizontallyAlignedBlocks[14] = new []{ 1, 2 };
        }
    }
}