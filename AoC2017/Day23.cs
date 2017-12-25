using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AoC2017
{
    class Day23
    {
        public static void Part1()
        {
            //You decide to head directly to the CPU and fix the printer from there.As you get close, you find 
            //an experimental coprocessor doing so much work that the local programs are afraid it will halt 
            //and catch fire.This would cause serious issues for the rest of the computer, so you head in and 
            //see what you can do.
            //The code it's running seems to be a variant of the kind you saw recently on that tablet. The 
            //general functionality seems very similar, but some of the instructions are different:
            // - set X Y sets register X to the value of Y.
            // - sub X Y decreases register X by the value of Y.
            // - mul X Y sets register X to the result of multiplying the value contained in register X by the 
            //   value of Y.
            // - jnz X Y jumps with an offset of the value of Y, but only if the value of X is not zero. (An 
            //   offset of 2 skips the next instruction, an offset of -1 jumps to the previous instruction, and 
            //   so on.)
            //Only the instructions listed above are used.The eight registers here, named a through h, all start 
            //at 0.
            //The coprocessor is currently set to some kind of debug mode, which allows for testing, but prevents 
            //it from doing any meaningful work.
            //If you run the program (your puzzle input), how many times is the mul instruction invoked?

            IDictionary<string, long> registers = new Dictionary<string, long>();
            IList<string[]> instructions = new List<string[]>();
            long mulInvocations = 0;

            string line;
            using (StringReader reader = new StringReader(Input))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    string[] instruction = line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    if (!registers.Keys.Contains(instruction[1]))
                    {
                        registers.Add(instruction[1], 0);
                    }
                    instructions.Add(instruction);
                }
            }

            for (var i = 0; i < instructions.Count; i++)
            {
                string[] instruction = instructions[i];
                Debug.WriteLine("instruction: " + instruction[0]);
                if (instruction[0] == "set")
                {
                    if (!long.TryParse(instruction[2], out long value))
                    {
                        value = registers[instruction[2]];
                    }
                    registers[instruction[1]] = value;
                }
                else if (instruction[0] == "sub")
                {
                    if (!long.TryParse(instruction[2], out long value))
                    {
                        value = registers[instruction[2]];
                    }
                    registers[instruction[1]] -= value;
                }
                else if (instruction[0] == "mul")
                {
                    if (!long.TryParse(instruction[2], out long value))
                    {
                        value = registers[instruction[2]];
                    }
                    registers[instruction[1]] *= value;
                    mulInvocations++;
                }
                else //jnz
                {
                    if (!long.TryParse(instruction[1], out long value))
                    {
                        value = registers[instruction[1]];
                    }

                    if (value != 0)
                    {
                        if (!int.TryParse(instruction[2], out int jumpValue))
                        {
                            jumpValue = (int)registers[instruction[2]];
                        }

                        i += jumpValue - 1;
                    }
                }
            }

            Debug.WriteLine("Invocations of mul: " + mulInvocations);
        }

        public static void Part2()
        {
            //Now, it's time to fix the problem.
            //The debug mode switch is wired directly to register a.You flip the switch, which makes register a 
            //now start at 1 when the program is executed.
            //Immediately, the coprocessor begins to overheat. Whoever wrote this program obviously didn't choose 
            //a very efficient implementation. You'll need to optimize the program if it has any hope of completing 
            //before Santa needs that printer working.
            //The coprocessor's ultimate goal is to determine the final value left in register h once the program 
            //completes. Technically, if it had that... it wouldn't even need to run the program.
            //After setting register a to 1, if the program were to run to completion, what value would be left in 
            //register h?

            //b = 93
            //c = 93
            //b *= 100 //9300
            //b -= -100000 //109300
            //c = 109300
            //c -= 17000 //126300
            //    f = 1
            //    d = 2
            //        e = 2
            //            g = d * e - b
            //            if g == 0
            //	            f = 0
            //            e -= -1
            //            g = e - b
            //            if g != 0 //loop (g = d * e - b)
            //        d -= -1
            //        g = d - b
            //        if g != 0 //loop (e = 2)
            //    if f == 0
            //        h -= -1
            //    g = b - c
            //    if g != 0
            //        b -= -17
            //        //loop (f = 1)
            //
            //b = 109300;
            //c = 126300;
            //for (b = 109300; b <= c; b += 17) {
            //    f = 1;
            //    for (d = 2; d <= b; d += 1) {
            //        for (e = 2; e <= b; e += 1) {
            //            if (d * e - b == 0) {
            //	            f = 0;
            //            }
            //        }
            //    }
            //    if (f == 0) {
            //        h += 1;
            //    }
            //}
            long b = 109300;
            long c = 126300;
            long h = 0;
            for (b = b; b <= c; b += 17)
            {
                long f = 1;
                //for (long d = 2; d <= b; d++)
                //{
                //    for (long e = 2; e <= b; e++)
                //    {
                //        if (d * e == b) --> controleren of b geen priemgetal is
                //        {
                //            f = 0;
                //        }
                //    }
                //}
                if (!IsPrimeNumber(b))
                {
                    f = 0;
                }
                if (f == 0)
                {
                    h += 1;
                }
            }
            Debug.WriteLine("h: " + h);
        }

        private static bool IsPrimeNumber(long number)
        {
            if (number == 1) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            var boundary = (int)Math.Floor(Math.Sqrt(number));

            for (int i = 3; i <= boundary; i += 2)
            {
                if (number % i == 0) return false;
            }

            return true;
        }

        private static string Input = @"set b 93
set c b
jnz a 2
jnz 1 5
mul b 100
sub b -100000
set c b
sub c -17000
set f 1
set d 2
set e 2
set g d
mul g e
sub g b
jnz g 2
set f 0
sub e -1
set g e
sub g b
jnz g -8
sub d -1
set g d
sub g b
jnz g -13
jnz f 2
sub h -1
set g b
sub g c
jnz g 2
jnz 1 3
sub b -17
jnz 1 -23";
    }
}
