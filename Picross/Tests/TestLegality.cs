using System;
using System.Collections.Generic;
using System.Linq;
using Picross.Game;

namespace Picross.Tests
{
    public static class TestLegality
    {
        private struct IsLegalTestCase
        {
            public string InputString;
            public int[] InputBlocks;
            public int TargetLength;
            public bool IsValid;
        }

        private static List<IsLegalTestCase> TestCases()
        {
            return new()
            {
                new() { InputString = "101101110000000", InputBlocks = new [] { 1,2,3 }, TargetLength = 15, IsValid = true },
                new() { InputString = "010110111000000", InputBlocks = new [] { 1,2,3 }, TargetLength = 15, IsValid = true },
                new() { InputString = "100001100000111", InputBlocks = new [] { 1,2,3 }, TargetLength = 15, IsValid = true },
                new() { InputString = "1000011000001111", InputBlocks = new [] { 1,2,3 }, TargetLength = 15, IsValid = false },
                new() { InputString = "101101111000000", InputBlocks = new [] { 1,2,3 }, TargetLength = 15, IsValid = false },
                new() { InputString = "010110111100000", InputBlocks = new [] { 1,2,3 }, TargetLength = 15, IsValid = false },
                new() { InputString = "100001100001111", InputBlocks = new [] { 1,2,3 }, TargetLength = 15, IsValid = false },
                new() { InputString = "110110111000000", InputBlocks = new [] { 1,2,3 }, TargetLength = 15, IsValid = false },
                new() { InputString = "101101110000000", InputBlocks = new [] { 1,2,3,1 }, TargetLength = 15, IsValid = false },
            };
        }
        
        public static void Run()
        {
            TestIsLegal();
            TestPermutations();
            TestMerge();
        }

        private struct PermutationTestCase
        {
            public int[] Blocks;
            public int ExpectedResult;
        }

        private static List<PermutationTestCase> PermutationTestCases()
        {
            return new()
            {
                new PermutationTestCase { Blocks = new []{1}, ExpectedResult = 15 },
                new PermutationTestCase { Blocks = new []{1,1}, ExpectedResult = 91 },
                new PermutationTestCase { Blocks = new []{1,2,2,3}, ExpectedResult = 70 },
                new PermutationTestCase { Blocks = new []{1,11,1}, ExpectedResult = 1 },
            };
        }
        
        private static void TestPermutations()
        {
            var b = new Board(15, 15);
            var clearRow = string.Concat(Enumerable.Repeat("0", 15));

            var testCases = PermutationTestCases();
            foreach (var testCase in testCases) {
                var rows = b.GenerateRows(testCase.Blocks, clearRow);
                ReportPermutationResult(rows, testCase.Blocks, testCase.ExpectedResult);
            }
        }

        private static void ReportPermutationResult(List<string> rows, int[] blocks, int expectedResult)
        {
            var blockString = string.Join(",", blocks);
            if (rows.Count == expectedResult)
            {
                Success("Successfully generated all permutations for: " + blockString);
            }
            else
            {
                Error("Error generating permutations for: " + blockString + $" - found {rows.Count} vs the expected {expectedResult}");
            }
        }

        private static void TestIsLegal()
        {
            var cases = TestCases();

            var b = new Board(15, 15);

            foreach (var c in cases)
            {
                if (b.IsLegal(c.InputString, c.InputBlocks) == c.IsValid)
                {
                    Success($"Success: {c.InputString} (valid: {c.IsValid})");
                }
                else
                {
                    Error($"Failed:  {c.InputString} (valid: {c.IsValid})");
                }
            }
        }

        private struct MergeTestCase
        {
            public string Existing;
            public string Proposal;
            public string Merged;
            public bool Valid;
        }

        private static List<MergeTestCase> MergeTestCases()
        {
            return new()
            {
                new MergeTestCase() { Existing = "00000", Proposal = "12112", Merged = "12112" },
                new MergeTestCase() { Existing = "00000", Proposal = "101101", Merged = "" },
                new MergeTestCase() { Existing = "11200", Proposal = "12112", Merged = "" },
                new MergeTestCase() { Existing = "01000", Proposal = "12112", Merged = "" },
                new MergeTestCase() { Existing = "21200", Proposal = "00212", Merged = "21212" },
                new MergeTestCase() { Existing = "12021", Proposal = "00221", Merged = "12221" },
                new MergeTestCase() { Existing = "12021", Proposal = "00000", Merged = "12021" },
            };
        }
        
        private static void TestMerge()
        {
            var cases = MergeTestCases();

            var b = new Board(15, 15);

            foreach (var c in cases)
            {
                var result = b.Merge(c.Existing, c.Proposal);
                if (c.Merged == result)
                {
                    if (result == null)
                    {
                        Success($"Success: {c.Existing} + {c.Proposal} = Invalid");
                    }
                    else
                    {
                        Success($"Success: {c.Existing} + {c.Proposal} = {c.Merged}");
                    }
                }
                else
                {
                    Error($"Failed:  {c.Existing} + {c.Proposal} = {c.Merged}");
                }
            }
        }

        private static void Error(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(error);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private static void Success(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}