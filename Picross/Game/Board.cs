﻿using System;
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
                var validCompatible = new List<string>();
                foreach (var valid in validOnes)
                {
                    var merged = Merge(_boardHorizontallyAligned[y], valid);
                    if (merged != "")
                    {
                        validCompatible.Add(merged);
                    }
                    else
                    {
                        LogError($"Merge failure between {_boardHorizontallyAligned[y]} and {valid}");
                    }
                }

                if (validCompatible.Count == 0)
                {
                    LogError($"Valid compatible not found for row {y}");
                    continue;
                }

                // Check for present in all valid
                CalculateInevitables(validCompatible, out var counters1, out var counters2);

                for (var x = 0; x < _width; x++)
                {
                    if (counters1[x] == validCompatible.Count)
                    {
                        Console.WriteLine($"{counters1[x]} ones out of {validCompatible.Count} for ({x},{y})");
                        Set(x, y, "1");
                    }
                    else if (counters2[x] == validCompatible.Count)
                    {
                        Console.WriteLine($"{counters2[x]} twos out of {validCompatible.Count} for ({x},{y})");
                        Set(x, y, "2");
                    }
                }
            }
            
            return true;
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
            if (depth == 0)
            {
                blocks = blocks.Where(b => b > 0).ToArray();
            }
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
                    PrintBlockNumber(index, _verticallyAlignedBlocks[index][block]);
                }
                Console.Write("\n");
            }
            
            for (var y = 0; y < _height; y++)
            {
                for (var bl = MaxBlocks - 1; bl >= 0; bl--)
                {
                    PrintBlockNumber(y, _horizontallyAlignedBlocks[y][bl]);
                }
                for (var x = 0; x < _width; x++)
                {
                    PrintSquare(_boardHorizontallyAligned[y][x]);
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
            _verticallyAlignedBlocks[0][0] = 2;

            _verticallyAlignedBlocks[1][0] = 1;
            _verticallyAlignedBlocks[1][1] = 2;

            _verticallyAlignedBlocks[2][0] = 1;
            _verticallyAlignedBlocks[2][1] = 2;
            _verticallyAlignedBlocks[2][2] = 1;

            _verticallyAlignedBlocks[3][0] = 1;
            _verticallyAlignedBlocks[3][1] = 1;
            _verticallyAlignedBlocks[3][2] = 1;
            _verticallyAlignedBlocks[3][3] = 2;

            _verticallyAlignedBlocks[4][0] = 1;
            _verticallyAlignedBlocks[4][1] = 4;
            _verticallyAlignedBlocks[4][2] = 1;
            _verticallyAlignedBlocks[4][3] = 1;

            _verticallyAlignedBlocks[5][0] = 6;
            _verticallyAlignedBlocks[5][1] = 1;
            _verticallyAlignedBlocks[5][2] = 2;

            _verticallyAlignedBlocks[6][0] = 1;
            _verticallyAlignedBlocks[6][1] = 2;
            _verticallyAlignedBlocks[6][2] = 1;
            _verticallyAlignedBlocks[6][3] = 1;

            _verticallyAlignedBlocks[7][0] = 2;
            _verticallyAlignedBlocks[7][1] = 2;
            _verticallyAlignedBlocks[7][2] = 1;

            _verticallyAlignedBlocks[8][0] = 2;
            _verticallyAlignedBlocks[8][1] = 3;
            _verticallyAlignedBlocks[8][2] = 1;
            _verticallyAlignedBlocks[8][3] = 1;

            _verticallyAlignedBlocks[9][0] = 2;
            _verticallyAlignedBlocks[9][1] = 3;
            _verticallyAlignedBlocks[9][2] = 2;

            _verticallyAlignedBlocks[10][0] = 11;
            _verticallyAlignedBlocks[10][1] = 1;
            _verticallyAlignedBlocks[10][2] = 1;

            _verticallyAlignedBlocks[11][0] = 2;
            _verticallyAlignedBlocks[11][1] = 5;
            _verticallyAlignedBlocks[11][2] = 1;

            _verticallyAlignedBlocks[12][0] = 1;
            _verticallyAlignedBlocks[12][1] = 1;
            _verticallyAlignedBlocks[12][2] = 1;
            _verticallyAlignedBlocks[12][3] = 3;

            _verticallyAlignedBlocks[13][0] = 1;
            _verticallyAlignedBlocks[13][1] = 1;
            _verticallyAlignedBlocks[13][2] = 1;

            _verticallyAlignedBlocks[14][0] = 2;
            
            // X
            _horizontallyAlignedBlocks[0][0] = 4;
            
            _horizontallyAlignedBlocks[1][0] = 2;
            _horizontallyAlignedBlocks[1][1] = 2;
            
            _horizontallyAlignedBlocks[2][0] = 1;
            _horizontallyAlignedBlocks[2][1] = 1;
            _horizontallyAlignedBlocks[2][2] = 1;
            _horizontallyAlignedBlocks[2][3] = 1;
            
            _horizontallyAlignedBlocks[3][0] = 1;
            _horizontallyAlignedBlocks[3][1] = 1;
            _horizontallyAlignedBlocks[3][2] = 2;
            
            _horizontallyAlignedBlocks[4][0] = 5;
            _horizontallyAlignedBlocks[4][1] = 3;
            
            _horizontallyAlignedBlocks[5][0] = 3;
            _horizontallyAlignedBlocks[5][1] = 1;
            _horizontallyAlignedBlocks[5][2] = 1;
            _horizontallyAlignedBlocks[5][3] = 2;
            
            _horizontallyAlignedBlocks[6][0] = 3;
            _horizontallyAlignedBlocks[6][1] = 2;
            _horizontallyAlignedBlocks[6][2] = 2;
            _horizontallyAlignedBlocks[6][3] = 1;
            
            _horizontallyAlignedBlocks[7][0] = 2;
            _horizontallyAlignedBlocks[7][1] = 1;
            _horizontallyAlignedBlocks[7][2] = 2;
            _horizontallyAlignedBlocks[7][3] = 3;
            
            _horizontallyAlignedBlocks[8][0] = 1;
            _horizontallyAlignedBlocks[8][1] = 2;
            _horizontallyAlignedBlocks[8][2] = 3;
            _horizontallyAlignedBlocks[8][3] = 1;
            
            _horizontallyAlignedBlocks[9][0] = 1;
            _horizontallyAlignedBlocks[9][1] = 4;
            _horizontallyAlignedBlocks[9][2] = 1;

            _horizontallyAlignedBlocks[10][0] = 1;
            _horizontallyAlignedBlocks[10][1] = 1;
            _horizontallyAlignedBlocks[10][2] = 5;

            _horizontallyAlignedBlocks[11][0] = 3;
            _horizontallyAlignedBlocks[11][1] = 1;
            _horizontallyAlignedBlocks[11][2] = 2;

            _horizontallyAlignedBlocks[12][0] = 4;
            _horizontallyAlignedBlocks[12][1] = 1;

            _horizontallyAlignedBlocks[13][0] = 2;
            _horizontallyAlignedBlocks[13][1] = 2;
            _horizontallyAlignedBlocks[13][2] = 1;

            _horizontallyAlignedBlocks[14][0] = 1;
            _horizontallyAlignedBlocks[14][1] = 2;
        }
    }
}