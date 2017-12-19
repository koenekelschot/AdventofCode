using System;
using System.Diagnostics;
using System.Linq;

namespace AoC2017
{
    class Day14
    {
        public static void Part1()
        {
            //The disk in question consists of a 128x128 grid; each square of the grid is either 
            //free or used. On this disk, the state of the grid is tracked by the bits in a sequence 
            //of knot hashes.
            //A total of 128 knot hashes are calculated, each corresponding to a single row in the grid; 
            //each hash contains 128 bits which correspond to individual grid squares. Each bit of a 
            //hash indicates whether that square is free (0) or used (1).
            //The hash inputs are a key string (your puzzle input), a dash, and a number from 0 to 127 
            //corresponding to the row. For example, if your key string were flqrgnkx, then the first row 
            //would be given by the bits of the knot hash of flqrgnkx-0, the second row from the bits of 
            //the knot hash of flqrgnkx-1, and so on until the last row, flqrgnkx-127.
            //The output of a knot hash is traditionally represented by 32 hexadecimal digits; each of 
            //these digits correspond to 4 bits, for a total of 4 * 32 = 128 bits. To convert to bits, 
            //turn each hexadecimal digit to its equivalent binary value, high - bit first: 0 becomes 0000, 
            //1 becomes 0001, e becomes 1110, f becomes 1111, and so on; a hash that begins with a0c2017... 
            //in hexadecimal would begin with 10100000110000100000000101110000... in binary.
            //Continuing this process, the first 8 rows and columns for key flqrgnkx appear as follows, using 
            //# to denote used squares, and . to denote free ones:
            // ##.#.#..-->
            // .#.#.#.#
            // ....#.#.
            // #.#.##.#
            // .##.#...
            // ##..#..#
            // .#...#..
            // ##.#.##.-->
            // |      |
            // V      V
            //In this example, 8108 squares are used across the entire 128x128 grid.
            //Given your actual key string, how many squares are used?

            int used = 0;
            for (var i = 0; i < 128; i++)
            {
                string bin = BinaryKnotHash(Input + "-" + i);
                Debug.WriteLine($"Bin  {i}: {bin}");
                used += bin.Count(c => c == '1');
            }
            Debug.WriteLine("Used squares: " + used);
        }

        public static void Part2()
        {
            //Now, all the defragmenter needs to know is the number of regions. A region is a group of used 
            //squares that are all adjacent, not including diagonals. Every used square is in exactly one 
            //region: lone used squares form their own isolated regions, while several adjacent squares all 
            //count as a single region.
            //In the example above, the following nine regions are visible, each marked with a distinct digit:
            // 11.2.3..-->
            // .1.2.3.4
            // ....5.6.
            // 7.8.55.9
            // .88.5...
            // 88..5..8
            // .8...8..
            // 88.8.88.-->
            // |      |
            // V      V
            //Of particular interest is the region marked 8; while it does not appear contiguous in this small 
            //view, all of the squares marked 8 are connected when considering the whole 128x128 grid. In total, 
            //in this example, 1242 regions are present.
            //How many regions are present given your key string?

            string[] gridRows = new string[128];
            int?[][] grid = new int?[128][];
            for (var i = 0; i < 128; i++)
            {
                grid[i] = new int?[128];
                gridRows[i] = BinaryKnotHash(Input + "-" + i);
            }

            int nextRegionId = 0;
            int currentRow = -1;
            char lastValue = '1';
            for (var i = 0; i < 128; i++)
            {
                if (i > currentRow)
                {
                    currentRow++;
                    if (lastValue == '1')
                    {
                        nextRegionId++;
                        lastValue = '0';
                    }
                }
                for (var j = 0; j < 128; j++)
                {
                    if (gridRows[i][j] == '1' && !grid[i][j].HasValue)
                    {
                        grid[i][j] = nextRegionId;
                        CheckAdjecentSquares(i, j, nextRegionId, ref grid, ref gridRows);
                        lastValue = '1';
                    }
                    else if (lastValue == '1')
                    {
                        nextRegionId++;
                        lastValue = '0';
                    }
                }
            }

            for (var i = 0; i < 128; i++)
            {
                var length = nextRegionId.ToString().Length;
                Debug.WriteLine(string.Join("|", grid[i].Select(g => g.HasValue ? g.Value.ToString().PadLeft(length) : "".PadLeft(length))));
            }

            Debug.WriteLine($"Regions: {nextRegionId - 1}");
        }

        private static string BinaryKnotHash(string input)
        {
            string hash = Day10.KnotHash(input);
            return string.Join(string.Empty, hash.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));
        }

        private static void CheckAdjecentSquares(int sourceY, int sourceX, int nextRegionId, ref int?[][] grid, ref string[] gridRows)
        {
            if (sourceX - 1 > -1 && !grid[sourceY][sourceX - 1].HasValue && gridRows[sourceY][sourceX - 1] == '1')
            {
                grid[sourceY][sourceX - 1] = nextRegionId;
                CheckAdjecentSquares(sourceY, sourceX - 1, nextRegionId, ref grid, ref gridRows);
            }
            if (sourceX + 1 < 128 && !grid[sourceY][sourceX + 1].HasValue && gridRows[sourceY][sourceX + 1] == '1')
            {
                grid[sourceY][sourceX + 1] = nextRegionId;
                CheckAdjecentSquares(sourceY, sourceX + 1, nextRegionId, ref grid, ref gridRows);
            }
            if (sourceY - 1 > -1 && !grid[sourceY - 1][sourceX].HasValue && gridRows[sourceY - 1][sourceX] == '1')
            {
                grid[sourceY - 1][sourceX] = nextRegionId;
                CheckAdjecentSquares(sourceY - 1, sourceX, nextRegionId, ref grid, ref gridRows);
            }
            if (sourceY + 1 < 128 && !grid[sourceY + 1][sourceX].HasValue && gridRows[sourceY + 1][sourceX] == '1')
            {
                grid[sourceY + 1][sourceX] = nextRegionId;
                CheckAdjecentSquares(sourceY + 1, sourceX, nextRegionId, ref grid, ref gridRows);
            }
        }

        public static string TestInput = "flqrgnkx";
        public static string Input = "nbysizxe";
    }
}
