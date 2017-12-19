using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace AoC2017
{
    class Day13
    {
        public static void Part1()
        {
            //You need to cross a vast firewall. The firewall consists of several layers, each with a security scanner that moves 
            //back and forth across the layer. To succeed, you must not be detected by a scanner.
            //By studying the firewall briefly, you are able to record (in your puzzle input) the depth of each layer and the range 
            //of the scanning area for the scanner within it, written as depth: range.Each layer has a thickness of exactly 1.
            //A layer at depth 0 begins immediately inside the firewall; a layer at depth 1 would start immediately after that.
            //For example, suppose you've recorded the following:
            // 0: 3
            // 1: 2
            // 4: 4
            // 6: 4
            //This means that there is a layer immediately inside the firewall (with range 3), a second layer immediately after that 
            //(with range 2), a third layer which begins at depth 4 (with range 4), and a fourth layer which begins at depth 6 (also 
            //with range 4).
            //Within each layer, a security scanner moves back and forth within its range. Each security scanner starts at the top 
            //and moves down until it reaches the bottom, then moves up until it reaches the top, and repeats. A security scanner 
            //takes one picosecond to move one step.
            //Your plan is to hitch a ride on a packet about to move through the firewall. The packet will travel along the top of 
            //each layer, and it moves at one layer per picosecond. Each picosecond, the packet moves one layer forward (its first 
            //move takes it into layer 0), and then the scanners move one step. If there is a scanner at the top of the layer as your 
            //packet enters it, you are caught. (If a scanner moves into the top of its layer while you are there, you are not caught: 
            //it doesn't have time to notice you before you leave.)
            //In this situation, you are caught in layers 0 and 6, because your packet entered the layer when its scanner was at the 
            //top when you entered it.You are not caught in layer 1, since the scanner moved into the top of the layer once you were 
            //already there.
            //The severity of getting caught on a layer is equal to its depth multiplied by its range. (Ignore layers in which you do 
            //not get caught.) The severity of the whole trip is the sum of these values. In the example above, the trip severity is 
            //0 * 3 + 6 * 4 = 24.
            //Given the details of the firewall you've recorded, if you leave immediately, what is the severity of your whole trip?

            IDictionary<int, int> layers = new Dictionary<int, int>();
            int severity = 0;
            int steps = -1;

            string line;
            using (StringReader reader = new StringReader(Input))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    var split = line.Split(new[] { ": " }, StringSplitOptions.RemoveEmptyEntries);
                    int layer = int.Parse(split[0]);
                    layers.Add(layer, int.Parse(split[1]));
                    if (layer > steps)
                    {
                        steps = layer;
                    }
                }
            }

            for (var i = 0; i <= steps; i++)
            {
                if (layers.Keys.Contains(i))
                {
                    int range = layers[i];
                    if (i % ((range - 1) * 2) == 0)
                    {
                        Debug.WriteLine("HIT");
                        severity += i * range;
                    }
                }
            }
            Debug.WriteLine(severity);
        }

        public static void Part2()
        {
            //Now, you need to pass through the firewall without being caught - easier said than done.
            //You can't control the speed of the packet, but you can delay it any number of picoseconds. For each picosecond you 
            //delay the packet before beginning your trip, all security scanners move one step. You're not in the firewall during 
            //this time; you don't enter layer 0 until you stop delaying the packet.
            //Because all smaller delays would get you caught, the fewest number of picoseconds you would need to delay to get through 
            //safely is 10. What is the fewest number of picoseconds that you need to delay the packet to pass through the firewall 
            //without being caught?

            IDictionary<int, int> layers = new Dictionary<int, int>();
            int steps = -1;
            int delay = 0;
            bool isHit = true;

            string line;
            using (StringReader reader = new StringReader(Input))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    var split = line.Split(new[] { ": " }, StringSplitOptions.RemoveEmptyEntries);
                    int layer = int.Parse(split[0]);
                    layers.Add(layer, int.Parse(split[1]));
                    if (layer > steps)
                    {
                        steps = layer;
                    }
                }
            }

            while (isHit)
            {
                isHit = false;
                foreach (var key in layers.Keys)
                {
                    int range = layers[key];
                    if ((key + delay) % ((range - 1) * 2) == 0)
                    {
                        Console.WriteLine($"HIT with delay {delay} at pos {key}");
                        isHit = true;
                        delay++;
                        break;
                    }
                }
            }

            Debug.WriteLine(delay);
        }

        public static string TestInput = @"0: 3
1: 2
4: 4
6: 4";

        public static string Input = @"0: 3
1: 2
2: 4
4: 4
6: 5
8: 6
10: 6
12: 8
14: 6
16: 6
18: 9
20: 8
22: 8
24: 8
26: 12
28: 8
30: 12
32: 12
34: 12
36: 10
38: 14
40: 12
42: 10
44: 8
46: 12
48: 14
50: 12
52: 14
54: 14
56: 14
58: 12
62: 14
64: 12
66: 12
68: 14
70: 14
72: 14
74: 17
76: 14
78: 18
84: 14
90: 20
92: 14";
    }
}
