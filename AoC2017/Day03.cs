using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AoC2017
{
    class Day03
    {
        public static void Part1()
        {
            //Each square on the grid is allocated in a spiral pattern starting at a location marked 1 and then counting up while spiraling outward. 
            //For example, the first few squares are allocated like this:
            // 17  16  15  14  13
            // 18   5   4   3  12
            // 19   6   1   2  11
            // 20   7   8   9  10
            // 21  22  23---> ...
            //While this is very space - efficient(no squares are skipped), requested data must be carried back to square 1
            //(the location of the only access port for this memory system) by programs that can only move up, down, left, or right. 
            //They always take the shortest path: the Manhattan Distance between the location of the data and square 1.
            //For example:
            // - Data from square 1 is carried 0 steps, since it's at the access port.
            // - Data from square 12 is carried 3 steps, such as: down, left, left.
            // - Data from square 23 is carried only 2 steps: up twice.
            // - Data from square 1024 must be carried 31 steps.
            //the taxicab distance between (p1,p2) and (q1,q2) is |p1−q1|+|p2−q2|.

            var input = 347991d;
            var dimension = Math.Ceiling(Math.Sqrt(input));
            Debug.WriteLine($"Size: {dimension}x{dimension}");
            var endOffset = Math.Ceiling((dimension - (input / dimension)) * dimension);
            var posCenterX = Math.Ceiling(dimension / 2);
            var posCenterY = Math.Ceiling(dimension / 2);
            var posX = 1d;
            var posY = 1d;
            if (dimension % 2 == 1)
            {
                //einde rechtsonder
                posY = dimension;
                posX = dimension;
                if (endOffset + 1 < dimension)
                {
                    posX -= dimension - endOffset;
                }
                else
                {
                    posX = 1;
                    posY -= endOffset + 1 - dimension;
                }
            }
            else
            {
                //einde linksboven
                posCenterY += 1;
                if (endOffset + 1 > dimension)
                {
                    posX = dimension;
                    posY += endOffset + 1 - dimension;
                }
                else
                {
                    posX += endOffset;
                }
            }
            Debug.WriteLine($"Center at ({posCenterX},{posCenterY})");
            Debug.WriteLine($"Pos at ({posX},{posY})");
            var distX = Math.Abs(posCenterX - posX);
            var distY = Math.Abs(posCenterY - posY);
            Debug.WriteLine($"Distance: {distX + distY}");
        }

        public static void Part2()
        {
            //In the same allocation order as shown above, they store the sum of the values in all adjacent squares, including diagonals.
            //So, the first few squares' values are chosen as follows:
            // - Square 1 starts with the value 1.
            // - Square 2 has only one adjacent filled square (with value 1), so it also stores 1.
            // - Square 3 has both of the above squares as neighbors and stores the sum of their values, 2.
            // - Square 4 has all three of the aforementioned squares as neighbors and stores the sum of their values, 4.
            // - Square 5 only has the first and fourth squares as neighbors, so it gets the value 5.
            //Once a square is written, its value does not change.Therefore, the first few squares would receive the following values:
            // 147  142  133  122   59
            // 304    5    4    2   57
            // 330   10    1    1   54
            // 351   11   23   25   26
            // 362  747  806--->   ...
            //What is the first value written that is larger than your puzzle input ?

            var input = 347991;
            var squares = new List<int> { 1, 1, 2, 4, 5, 10, 11, 23, 25, 26, 54, 57, 59, 122, 133, 142 };

            //algoritme klopt alleen als de binnenste ring (1, 1, 2, 4) niet nodig is, daarom de eerste 16 getallen vullen
            while (squares.Last() < input)
            {
                var i = squares.Count;
                var adjecent = new List<int> { i - 1 };
                if (IsCornerPosition(i))
                {
                    //het is een hoek: alleen een vorige waarde en de diagonaal
                    var innerCornerIndex = GetInnerCornerIndex(i);
                    adjecent.Add(innerCornerIndex);
                }
                else
                {
                    if (IsCornerPosition(i - 1))
                    {
                        //direct na een hoek, dus i - 2 is diagonaal
                        adjecent.Add(i - 2);
                    }

                    var lastRoot = Math.Floor(Math.Sqrt(i));
                    var lastCorner = (int)Math.Pow(lastRoot, 2);
                    var offsetFromCorner = i - lastCorner;
                    var adj = (int)Math.Pow(lastRoot - 2, 2) + offsetFromCorner - 1;
                    var direction = GetDirectionForIndex(i);
                    if (direction.Equals("left") || direction.Equals("right")) {
                        adj -= 2; //extra offset nodig
                    }
                    adjecent.Add(adj);
                    if (GetDirectionForIndex(adj - 1).Equals(direction) || IsCornerPosition(adj - 1))
                    {
                        adjecent.Add(adj - 1);
                    }
                    if (GetDirectionForIndex(adj + 1).Equals(direction) || IsCornerPosition(adj + 1))
                    {
                        adjecent.Add(adj + 1);
                    }
                }
                Debug.WriteLine($"Pos {i}: {string.Join(", ", adjecent)}");
                var sum = 0;
                foreach(var adj in adjecent)
                {
                    sum += squares[adj];
                }
                squares.Add(sum);
                Debug.WriteLine($"Pos {i}: {sum}");
            }
        }

        private static bool IsCornerPosition(int index)
        {
            return Math.Sqrt(index) % 1 == 0 || Math.Sqrt(index - Math.Floor(Math.Sqrt(index))) % 1 == 0;
        }

        private static int GetInnerCornerIndex(int index)
        {
            if (Math.Sqrt(index) % 1 == 0)
            {
                //het is een hoek: alleen een vorige waarde en de diagonaal
                var root = Math.Floor(Math.Sqrt(index));
                return (int)Math.Pow(root - 2, 2);
            }
            else
            {
                //Math.Sqrt(i - Math.Floor(Math.Sqrt(i))) % 1 == 0
                var root = Math.Ceiling(Math.Sqrt(index));
                var innerCornerIndex = (int)Math.Pow(root - 2, 2);
                innerCornerIndex -= (int)Math.Sqrt(innerCornerIndex);
                return innerCornerIndex;
            }
        }

        private static string GetDirectionForIndex(int index)
        {
            if (index > Math.Pow(Math.Floor(Math.Sqrt(index)), 2) && 
                index < Math.Pow(Math.Floor(Math.Sqrt(index)), 2) + Math.Floor(Math.Sqrt(index)))
            {
                if (Math.Floor(Math.Sqrt(index)) % 2 == 1)
                {
                    return "up";
                }
                else
                {
                    return "down";
                }
            }
            if (index > Math.Pow(Math.Floor(Math.Sqrt(index)), 2) + Math.Floor(Math.Sqrt(index)) && 
                index < Math.Pow(Math.Ceiling(Math.Sqrt(index)), 2))
            {
                if (Math.Floor(Math.Sqrt(index)) % 2 == 1)
                {
                    return "left";
                }
                else
                {
                    return "right";
                }
            }
            return "corner";
        }
    }
}
