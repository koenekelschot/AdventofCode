using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AoC2017
{
    class Day21
    {
        public static void Part1()
        {
            //You find a program trying to generate some art. It uses a strange process that involves repeatedly enhancing the 
            //detail of an image through a set of rules.
            //The image consists of a two-dimensional square grid of pixels that are either on (#) or off (.). The program 
            //always begins with this pattern:
            // .#.
            // ..#
            // ###
            //Because the pattern is both 3 pixels wide and 3 pixels tall, it is said to have a size of 3.
            //Then, the program repeats the following process:
            // - If the size is evenly divisible by 2, break the pixels up into 2x2 squares, and convert each 2x2 square into a 
            //   3x3 square by following the corresponding enhancement rule.
            // - Otherwise, the size is evenly divisible by 3; break the pixels up into 3x3 squares, and convert each 3x3 square 
            //   into a 4x4 square by following the corresponding enhancement rule.
            //Because each square of pixels is replaced by a larger one, the image gains pixels and so its size increases.
            //The artist's book of enhancement rules is nearby (your puzzle input); however, it seems to be missing rules. The 
            //artist explains that sometimes, one must rotate or flip the input pattern to find a match. (Never rotate or flip the 
            //output pattern, though.) Each pattern is written concisely: rows are listed as single units, ordered top-down, and 
            //separated by slashes. For example, the following rules correspond to the adjacent patterns:
            // ../.#  =  ..
            //           .#
            //
            //                 .#.
            // .#./..#/###  =  ..#
            //                 ###
            //
            //                         #..#
            // #..#/..../#..#/.##.  =  ....
            //                         #..#
            //                         .##.
            //When searching for a rule to use, rotate and flip the pattern as necessary. For example, all of the following patterns 
            //match the same rule:
            // .#.   .#.   #..   ###
            // ..#   #..   #.#   ..#
            // ###   ###   ##.   .#.
            //Suppose the book contained the following two rules:
            // ../.# => ##./#../...
            // .#./..#/### => #..#/..../..../#..#
            //As before, the program begins with this pattern:
            // .#.
            // ..#
            // ###
            //The size of the grid (3) is not divisible by 2, but it is divisible by 3. It divides evenly into a single square; the 
            //square matches the second rule, which produces:
            // #..#
            // ....
            // ....
            // #..#
            //The size of this enhanced grid (4) is evenly divisible by 2, so that rule is used. It divides evenly into four squares:
            // #.|.#
            // ..|..
            // --+--
            // ..|..
            // #.|.#
            //Each of these squares matches the same rule (../.# => ##./#../...), three of which require some flipping and rotation to 
            //line up with the rule. The output for the rule is the same in all four cases:
            // ##.|##.
            // #..|#..
            // ...|...
            // ---+---
            // ##.|##.
            // #..|#..
            // ...|...
            //Finally, the squares are joined into a new grid:
            // ##.##.
            // #..#..
            // ......
            // ##.##.
            // #..#..
            // ......
            //Thus, after 2 iterations, the grid contains 12 pixels that are on.
            //How many pixels stay on after 5 iterations?

            Run(5);
        }

        public static void Part2()
        {
            Run(18);
        }

        private static void Run(int iterations)
        {
            IDictionary<string, string> inputFilters = new Dictionary<string, string>();
            string program = ".#./..#/###";

            using (StringReader reader = new StringReader(Input))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(new[] { " => " }, StringSplitOptions.RemoveEmptyEntries);
                    string rot0 = parts[0];
                    string flip0 = Flip(rot0);
                    string rot1 = Rotate(rot0);
                    string flip1 = Flip(rot1);
                    string rot2 = Rotate(rot1);
                    string flip2 = Flip(rot2);
                    string rot3 = Rotate(rot2);
                    string flip3 = Flip(rot3);

                    foreach (string filter in new string[] { rot0, flip0, rot1, flip1, rot2, flip2, rot3, flip3 }.Distinct())
                    {
                        if (!inputFilters.ContainsKey(filter))
                        {
                            inputFilters.Add(filter, parts[1]);
                        }
                    }
                }
            }

            for (var i = 0; i < iterations; i++)
            {
                string[] rows = program.Split('/');
                IList<string> parts = new List<string>();
                if (rows.Length % 2 == 0)
                {
                    parts = SplitParts(program, 2);
                }
                else if (rows.Length % 3 == 0)
                {
                    parts = SplitParts(program, 3);
                }

                for (var j = 0; j < parts.Count; j++)
                {
                    if (inputFilters.ContainsKey(parts[j]))
                    {
                        parts[j] = inputFilters[parts[j]];
                    }
                }

                program = JoinParts(parts);
                Debug.WriteLine(i + ": " + program);
            }
            
            Debug.WriteLine("On: " + program.Count(c => c == '#'));
            Debug.WriteLine("Off: " + program.Count(c => c == '.'));
        }

        private static string Rotate(string input)
        {
            string test = MirrorDiagonally(input);
            return Flip(test);
        }

        private static string MirrorDiagonally(string input)
        {
            string[] rows = input.Split(new[] { '/' });
            char[][] output = new char[rows.Length][];
            for (var i = 0; i < rows.Length; i++)
            {
                output[i] = new char[rows.Length];
            }
            
            for (var y = 0; y < rows.Length; y++)
            {
                for (var x = 0; x < rows.Length; x++)
                {
                    output[x][y] = rows[y][x];
                }
            }
            
            return string.Join("/", output.Select(r => string.Join("", r)).ToArray());
        }

        private static string Flip(string input)
        {
            string[] rows = input.Split(new[] { '/' });
            string[] output = new string[rows.Length];

            for (var i = 0; i < rows.Length; i++)
            {
                output[i] = string.Join("", rows[i].Reverse().ToArray());
            }

            return string.Join("/", output);
        }

        private static IList<string> SplitParts(string input, int size)
        {
            string[] rows = input.Split('/');
            IList<string> parts = new List<string>();

            if (rows.Length > size)
            {
                for (var i = 0; i < rows.Length / size; i++)
                {
                    for (var j = 0; j < rows.Length / size; j++)
                    {
                        string part = "";
                        for (var k = 0; k < size; k++)
                        {
                            part += string.Join("", rows[(i * size) + k].Skip(j * size).Take(size)) + "/";
                        }
                        parts.Add(part.Trim('/'));
                    }
                }
            }
            else
            {
                parts.Add(input);
            }

            return parts;
        }

        private static string JoinParts(IList<string> parts)
        {
            if (parts.Count == 1)
            {
                return parts[0];
            }
            int partsPerRow = (int)Math.Sqrt(parts.Count);
            int partSize = parts[0].Split('/').Length;
            string[] rows = new string[partsPerRow * partSize];
            for (var i = 0; i < parts.Count; i++)
            {
                int rowOffset = (int)Math.Floor((double)i / partsPerRow);
                string[] rowParts = parts[i].Split('/');
                for (var j = 0; j < rowParts.Length; j++)
                {
                    rows[j + (rowOffset * partSize)] += rowParts[j];
                }
            }
            return string.Join("/", rows);
        }

        private static string TestInput = @"../.# => ##./#../...
.#./..#/### => #..#/..../..../#..#";

        private static string Input = @"../.. => .../.##/.##
#./.. => .#./.#./##.
##/.. => ##./.../..#
.#/#. => #../..#/##.
##/#. => .../.#./..#
##/## => #.#/.##/.##
.../.../... => ##../.#../##../#..#
#../.../... => ..#./##.#/#.##/....
.#./.../... => ####/#.##/..../...#
##./.../... => ####/...#/.###/..##
#.#/.../... => ..#./..#./##../##.#
###/.../... => ..#./..#./##../...#
.#./#../... => ##.#/###./###./#..#
##./#../... => .#../..##/#.#./#.#.
..#/#../... => .##./..../...#/.###
#.#/#../... => ##../#..#/#..#/....
.##/#../... => ..../#.../..##/##..
###/#../... => ####/#.../.##./#...
.../.#./... => ####/#.../.###/###.
#../.#./... => #.#./.###/#.../##.#
.#./.#./... => .##./##.#/..##/.#..
##./.#./... => ..##/.#../..##/##.#
#.#/.#./... => .##./.#.#/.#.#/....
###/.#./... => ..../##.#/#.#./.###
.#./##./... => ..#./#.../#.../..##
##./##./... => ##.#/##.#/#.##/#...
..#/##./... => .#../.#.#/#.##/####
#.#/##./... => ..#./#.##/..../.##.
.##/##./... => #.##/..##/...#/....
###/##./... => ..#./#.../#.##/.#.#
.../#.#/... => ..##/#.#./##../#...
#../#.#/... => #.#./..#./.#../..##
.#./#.#/... => #.#./.#.#/.#../..##
##./#.#/... => ###./##.#/#..#/####
#.#/#.#/... => ##.#/..##/#.../...#
###/#.#/... => ##.#/..##/###./##..
.../###/... => ..../...#/##../.###
#../###/... => .##./##.#/..../#...
.#./###/... => ###./..##/.##./#...
##./###/... => .##./#..#/.###/.#..
#.#/###/... => ..../#.#./#.../#..#
###/###/... => .#../#.#./#.##/##.#
..#/.../#.. => ##../...#/.#../###.
#.#/.../#.. => #..#/.#../#.#./..#.
.##/.../#.. => #.##/.#../...#/.#.#
###/.../#.. => .#.#/#.../.#.#/.#..
.##/#../#.. => ..#./..../###./#...
###/#../#.. => .##./##../.#.#/##.#
..#/.#./#.. => ###./.##./###./.###
#.#/.#./#.. => ..../..../#.##/.#..
.##/.#./#.. => .#.#/.#.#/#.../####
###/.#./#.. => #.../####/#.##/#.#.
.##/##./#.. => #.../#.##/#.../###.
###/##./#.. => ...#/.##./#.../.##.
#../..#/#.. => ##../##../..##/....
.#./..#/#.. => #.#./##../.###/#.##
##./..#/#.. => #.#./####/.###/...#
#.#/..#/#.. => #..#/##.#/.#../..#.
.##/..#/#.. => .###/.#../#.##/.##.
###/..#/#.. => .###/#.##/..#./..##
#../#.#/#.. => ####/#.../####/##.#
.#./#.#/#.. => .###/####/####/.#..
##./#.#/#.. => ##.#/...#/..../##.#
..#/#.#/#.. => .#../..#./.##./.#..
#.#/#.#/#.. => ...#/###./..##/.###
.##/#.#/#.. => ####/##../#..#/##..
###/#.#/#.. => .#.#/..##/.###/##..
#../.##/#.. => #..#/#.##/#..#/.###
.#./.##/#.. => ##../.###/..../###.
##./.##/#.. => .###/.###/##../.##.
#.#/.##/#.. => ..#./.##./##../#.#.
.##/.##/#.. => ####/#..#/..#./....
###/.##/#.. => #.../.#../#..#/.#..
#../###/#.. => ..../.#../.##./.#.#
.#./###/#.. => ..../####/#.##/###.
##./###/#.. => ...#/.#../#.../##.#
..#/###/#.. => ####/###./###./....
#.#/###/#.. => .#../.###/##.#/.###
.##/###/#.. => #.##/##../##../.#..
###/###/#.. => .###/###./#..#/.#.#
.#./#.#/.#. => ###./.###/.###/.##.
##./#.#/.#. => .#.#/##../###./..#.
#.#/#.#/.#. => .#.#/##../###./#.##
###/#.#/.#. => ..#./.#../.#../..#.
.#./###/.#. => #..#/..##/#.#./#.#.
##./###/.#. => .#../#..#/#.#./.##.
#.#/###/.#. => .#.#/.##./.###/....
###/###/.#. => #.#./#.#./##../.#..
#.#/..#/##. => .#.#/.#.#/#..#/.#.#
###/..#/##. => #.#./##.#/.#../#.##
.##/#.#/##. => #.##/#.##/#.##/##.#
###/#.#/##. => ###./##../.#.#/#...
#.#/.##/##. => ##.#/.#.#/.#.#/.#.#
###/.##/##. => .#.#/#.##/####/....
.##/###/##. => #.../####/###./.###
###/###/##. => .##./#.#./#.##/##..
#.#/.../#.# => #.../##.#/#.##/##.#
###/.../#.# => #.#./#.##/##.#/.##.
###/#../#.# => ##../.#.#/##.#/#...
#.#/.#./#.# => .##./.#../#.../.#.#
###/.#./#.# => #.#./..##/###./..##
###/##./#.# => .###/..##/..#./..#.
#.#/#.#/#.# => .#../##.#/.#.#/.#.#
###/#.#/#.# => ##.#/.#.#/...#/...#
#.#/###/#.# => ##.#/.#../####/#..#
###/###/#.# => ...#/..##/##../#..#
###/#.#/### => ..##/.##./.##./#.##
###/###/### => #.#./.#.#/#.../.##.";
    }
}
