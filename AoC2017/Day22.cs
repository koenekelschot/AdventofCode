using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AoC2017
{
    class Day22
    {
        public static void Part1()
        {
            //Diagnostics indicate that the local grid computing cluster has been contaminated with the 
            //Sporifica Virus. The grid computing cluster is a seemingly-infinite two-dimensional grid of 
            //compute nodes. Each node is either clean or infected by the virus.
            //To prevent overloading the nodes (which would render them useless to the virus) or detection 
            //by system administrators, exactly one virus carrier moves through the network, infecting or 
            //cleaning nodes as it moves.The virus carrier is always located on a single node in the network
            //(the current node) and keeps track of the direction it is facing.
            //To avoid detection, the virus carrier works in bursts; in each burst, it wakes up, does some 
            //work, and goes back to sleep. The following steps are all executed in order one time each burst:
            // - If the current node is infected, it turns to its right. Otherwise, it turns to its left. 
            //   (Turning is done in -place; the current node does not change.)
            // - If the current node is clean, it becomes infected. Otherwise, it becomes cleaned. (This is 
            //   done after the node is considered for the purposes of changing direction.)
            // - The virus carrier moves forward one node in the direction it is facing.
            //Diagnostics have also provided a map of the node infection status (your puzzle input). Clean nodes 
            //are shown as .; infected nodes are shown as #. This map only shows the center of the grid; there 
            //are many more nodes beyond those shown, but none of them are currently infected.
            //The virus carrier begins in the middle of the map facing up.
            //For example, suppose you are given a map like this:
            // ..#
            // #..
            // ...
            //Then, the middle of the infinite grid looks like this, with the virus carrier's position marked with [ ]:
            // . . . . . . . . .
            // . . . . . . . . .
            // . . . . . . . . .
            // . . . . . # . . .
            // . . . #[.]. . . .
            // . . . . . . . . .
            // . . . . . . . . .
            // . . . . . . . . .
            //The virus carrier is on a clean node, so it turns left, infects the node, and moves left:
            // . . . . . . . . .
            // . . . . . . . . .
            // . . . . . . . . .
            // . . . . . # . . .
            // . . .[#]# . . . .
            // . . . . . . . . .
            // . . . . . . . . .
            // . . . . . . . . .
            //The virus carrier is on an infected node, so it turns right, cleans the node, and moves up:
            // . . . . . . . . .
            // . . . . . . . . .
            // . . . . . . . . .
            // . . .[.]. # . . .
            // . . . . # . . . .
            // . . . . . . . . .
            // . . . . . . . . .
            // . . . . . . . . .
            //After a total of 70 bursts of activity, the grid looks like this, with the virus carrier facing up:
            // . . . . . # # . .
            // . . . . # . . # .
            // . . . # . . . . #
            // . . # . #[.]. . #
            // . . # . # . . # .
            // . . . . . # # . .
            // . . . . . . . . .
            // . . . . . . . . .
            //By this time, 41 bursts of activity caused an infection(though most of those nodes have since been 
            //cleaned). After a total of 10000 bursts of activity, 5587 bursts will have caused an infection.
            //Given your actual map, after 10000 bursts of activity, how many bursts cause a node to become infected? 
            //(Do not count nodes that begin infected.)

            IList<Point> infectedNodes = new List<Point>();
            Point currentPosition = new Point(0, 0);
            Direction direction = Direction.UP;
            int bursts = 10000;
            int infected = 0;
            int cleaned = 0;
            
            using (StringReader reader = new StringReader(Input))
            {
                string line;
                int lineNumber = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    for (var i = 0; i < line.Length; i++)
                    {
                        if (line[i] == '#')
                        {
                            infectedNodes.Add(new Point(i, lineNumber));
                        }
                    }
                    currentPosition = new Point(line.Length / 2, lineNumber / 2);
                    lineNumber++;
                }
            }

            for (var i = 0; i < bursts; i++)
            {
                if (infectedNodes.Any(p => p.X == currentPosition.X && p.Y == currentPosition.Y))
                {
                    //infected -> turn right
                    if (direction == Direction.UP) direction = Direction.RIGHT;
                    else if (direction == Direction.RIGHT) direction = Direction.DOWN;
                    else if (direction == Direction.DOWN) direction = Direction.LEFT;
                    else direction = Direction.UP;
                    infectedNodes.Remove(infectedNodes.First(p => p.X == currentPosition.X && p.Y == currentPosition.Y));
                    cleaned++;
                    Debug.WriteLine($"{i}: cleaned [{currentPosition.X},{currentPosition.Y}]");
                }
                else
                {
                    //clean -> turn left
                    if (direction == Direction.UP) direction = Direction.LEFT;
                    else if (direction == Direction.LEFT) direction = Direction.DOWN;
                    else if (direction == Direction.DOWN) direction = Direction.RIGHT;
                    else direction = Direction.UP;
                    infectedNodes.Add(new Point(currentPosition.X, currentPosition.Y));
                    infected++;
                    Debug.WriteLine($"{i}: infected [{currentPosition.X},{currentPosition.Y}]");
                }
                if (direction == Direction.UP) currentPosition.Y -= 1;
                else if (direction == Direction.DOWN) currentPosition.Y += 1;
                else if (direction == Direction.LEFT) currentPosition.X -= 1;
                else currentPosition.X += 1;
            }

            Debug.WriteLine($"Infected {infected} nodes");
            Debug.WriteLine($"Cleaned {cleaned} nodes");
        }

        public static void Part2()
        {
            //As you go to remove the virus from the infected nodes, it evolves to resist your attempt.
            //Now, before it infects a clean node, it will weaken it to disable your defenses. If it 
            //encounters an infected node, it will instead flag the node to be cleaned in the future. So:
            // - Clean nodes become weakened.
            // - Weakened nodes become infected.
            // - Infected nodes become flagged.
            // - Flagged nodes become clean.
            //Every node is always in exactly one of the above states.
            //The virus carrier still functions in a similar way, but now uses the following logic during its 
            //bursts of action:
            // - Decide which way to turn based on the current node:
            //    - If it is clean, it turns left.
            //    - If it is weakened, it does not turn, and will continue moving in the same direction.
            //    - If it is infected, it turns right.
            //    - If it is flagged, it reverses direction, and will go back the way it came.
            // - Modify the state of the current node, as described above.
            // - The virus carrier moves forward one node in the direction it is facing.
            //Start with the same map (still using . for clean and # for infected) and still with the virus 
            //carrier starting in the middle and facing up.
            //Of the first 100 bursts, 26 will result in infection. Unfortunately, another feature of this 
            //evolved virus is speed; of the first 10000000 bursts, 2511944 will result in infection.
            //Given your actual map, after 10000000 bursts of activity, how many bursts cause a node to become 
            //infected? (Do not count nodes that begin infected.)

            IList<Point> infectedNodes = new List<Point>();
            IList<Point> flaggedNodes = new List<Point>();
            IList<Point> weakenedNodes = new List<Point>();
            Point currentPosition = new Point(0, 0);
            Direction direction = Direction.UP;
            int bursts = 10000000;
            int infected = 0;
            int flagged = 0;
            int cleaned = 0;
            int weakened = 0;

            using (StringReader reader = new StringReader(Input))
            {
                string line;
                int lineNumber = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    for (var i = 0; i < line.Length; i++)
                    {
                        if (line[i] == '#')
                        {
                            infectedNodes.Add(new Point(i, lineNumber));
                        }
                    }
                    currentPosition = new Point(line.Length / 2, lineNumber / 2);
                    lineNumber++;
                }
            }

            for (var i = 0; i < bursts; i++)
            {
                Console.WriteLine(i);
                if (infectedNodes.Any(p => p.X == currentPosition.X && p.Y == currentPosition.Y))
                {
                    //infected -> turn right
                    if (direction == Direction.UP) direction = Direction.RIGHT;
                    else if (direction == Direction.RIGHT) direction = Direction.DOWN;
                    else if (direction == Direction.DOWN) direction = Direction.LEFT;
                    else direction = Direction.UP;
                    infectedNodes.Remove(infectedNodes.First(p => p.X == currentPosition.X && p.Y == currentPosition.Y));
                    flaggedNodes.Add(new Point(currentPosition.X, currentPosition.Y));
                    flagged++;
                    //Console.WriteLine($"{i}: flagged [{currentPosition.X},{currentPosition.Y}]");
                }
                else if (flaggedNodes.Any(p => p.X == currentPosition.X && p.Y == currentPosition.Y))
                {
                    //flagged -> reverse direction
                    if (direction == Direction.UP) direction = Direction.DOWN;
                    else if (direction == Direction.RIGHT) direction = Direction.LEFT;
                    else if (direction == Direction.DOWN) direction = Direction.UP;
                    else direction = Direction.RIGHT;
                    flaggedNodes.Remove(flaggedNodes.First(p => p.X == currentPosition.X && p.Y == currentPosition.Y));
                    cleaned++;
                    //Console.WriteLine($"{i}: cleaned [{currentPosition.X},{currentPosition.Y}]");
                }
                else if (weakenedNodes.Any(p => p.X == currentPosition.X && p.Y == currentPosition.Y))
                {
                    //weakened -> continue in same direction
                    weakenedNodes.Remove(weakenedNodes.First(p => p.X == currentPosition.X && p.Y == currentPosition.Y));
                    infectedNodes.Add(new Point(currentPosition.X, currentPosition.Y));
                    infected++;
                    //Console.WriteLine($"{i}: infected [{currentPosition.X},{currentPosition.Y}]");
                }
                else
                {
                    //clean -> turn left
                    if (direction == Direction.UP) direction = Direction.LEFT;
                    else if (direction == Direction.LEFT) direction = Direction.DOWN;
                    else if (direction == Direction.DOWN) direction = Direction.RIGHT;
                    else direction = Direction.UP;
                    weakenedNodes.Add(new Point(currentPosition.X, currentPosition.Y));
                    weakened++;
                    //Console.WriteLine($"{i}: weakened [{currentPosition.X},{currentPosition.Y}]");
                }
                if (direction == Direction.UP) currentPosition.Y -= 1;
                else if (direction == Direction.DOWN) currentPosition.Y += 1;
                else if (direction == Direction.LEFT) currentPosition.X -= 1;
                else currentPosition.X += 1;
            }

            Debug.WriteLine($"Weakened {weakened} nodes");
            Debug.WriteLine($"Infected {infected} nodes");
            Debug.WriteLine($"Flagged {flagged} nodes");
            Debug.WriteLine($"Cleaned {cleaned} nodes");
        }

        private class Point
        {
            public int X { get; set; }
            public int Y { get; set; }

            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        private enum Direction
        {
            UP,
            DOWN,
            LEFT,
            RIGHT
        }

        private static string TestInput = @"..#
#..
...";

        private static string Input = @"#..#...#.#.#..#.#...##.##
.....##.#....#.#......#.#
..#.###.#.#######.###.#.#
.......#.##.###.###..##.#
#....#...#.###....##.....
#.....##.#########..#.#.#
.####.##..#...###.##...#.
#....#..#.###.##.#..##.#.
#####.#.#..#.##..#...####
##.#.#..#.#....###.######
.##.#...#...##.#.##..####
...#..##.#.....#.#..####.
#.##.###..#######.#..#.#.
##....##....##.#..#.##..#
##.#.#.#.##...##.....#...
.#####..#.#....#.#######.
####....###.###.#.#..#..#
.###...#.###..#..#.#####.
#.###..#.#######.#.#####.
.##.#.###.##.##.#.#...#..
######.###.#.#.##.####..#
##..####.##..##.#...##...
...##.##...#..#..##.####.
#.....##.##.#..##.##....#
#.#..####.....#....#.###.";
    }
}
