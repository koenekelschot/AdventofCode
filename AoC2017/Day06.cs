using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AoC2017
{
    class Day06
    {
        public static void Part1()
        {
            //There are sixteen memory banks; each memory bank can hold any number of blocks.
            //The goal of the reallocation routine is to balance the blocks between the memory banks.
            //The reallocation routine operates in cycles.In each cycle, it finds the memory bank with 
            //the most blocks (ties won by the lowest - numbered memory bank) and redistributes those 
            //blocks among the banks. To do this, it removes all of the blocks from the selected bank, 
            //then moves to the next (by index) memory bank and inserts one of the blocks. It continues 
            //doing this until it runs out of blocks; if it reaches the last memory bank, it wraps around 
            //to the first one.
            //The debugger would like to know how many redistributions can be done before a blocks-in-banks 
            //configuration is produced that has been seen before.
            //For example, imagine a scenario with only four memory banks:
            // - The banks start with 0, 2, 7, and 0 blocks.The third bank has the most blocks, so it is chosen for redistribution.
            // - The infinite loop is detected after the fifth block redistribution cycle, and so the answer in this example is 5.
            //Given the initial block counts in your puzzle input, how many redistribution cycles must be completed before a configuration is produced that has been seen before?

            bool noLoopDetected = true;
            List<string> configs = new List<string>();
            string lastConfig = Input;

            while (noLoopDetected = !configs.Any(c => c.Equals(lastConfig)))
            {
                configs.Add(lastConfig);
                List<int> extendedConfig = lastConfig.Split(' ').Select(c => int.Parse(c)).ToList();
                var numBanks = extendedConfig.Count();
                var maxBlocks = extendedConfig.Max();
                var bankIndex = extendedConfig.IndexOf(maxBlocks);
                extendedConfig[bankIndex] = 0;
                while (maxBlocks > 0)
                {
                    bankIndex++;
                    if (bankIndex >= numBanks)
                    {
                        bankIndex -= numBanks;
                    }
                    extendedConfig[bankIndex]++;
                    maxBlocks--;
                }

                lastConfig = string.Join(" ", extendedConfig);
                Debug.WriteLine(lastConfig);
            }
            Debug.WriteLine("Steps: " + configs.Count());
        }

        public static void Part2()
        {
            //In the example above, 2 4 1 2 is seen again after four cycles, and so the answer in that example would be 4.
            //How many cycles are in the infinite loop that arises from the configuration in your puzzle input?
            bool noLoopDetected = true;
            List<string> configs = new List<string>();
            string lastConfig = Input;

            while (noLoopDetected = !configs.Any(c => c.Equals(lastConfig)))
            {
                configs.Add(lastConfig);
                List<int> extendedConfig = lastConfig.Split(' ').Select(c => int.Parse(c)).ToList();
                var numBanks = extendedConfig.Count();
                var maxBlocks = extendedConfig.Max();
                var bankIndex = extendedConfig.IndexOf(maxBlocks);
                extendedConfig[bankIndex] = 0;
                while (maxBlocks > 0)
                {
                    bankIndex++;
                    if (bankIndex >= numBanks)
                    {
                        bankIndex -= numBanks;
                    }
                    extendedConfig[bankIndex]++;
                    maxBlocks--;
                }

                lastConfig = string.Join(" ", extendedConfig);
                Debug.WriteLine(lastConfig);
            }
            Debug.WriteLine("Steps: " + configs.Count());
            Debug.WriteLine("Seen before at: " + configs.IndexOf(lastConfig));
            Debug.WriteLine("Result: " + (configs.Count() - configs.IndexOf(lastConfig)));

        }

        //private static string Input = "0 2 7 0";
        private static string Input = "4 10 4 1 8 4 9 14 5 1 14 15 0 15 3 5";
    }
}
