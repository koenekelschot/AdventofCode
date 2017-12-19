using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace AoC2017
{
    class Day18
    {
        public static void Part1()
        {
            //It seems like the assembly is meant to operate on a set of registers that are each named with a single letter 
            //and that can each hold a single integer. You suppose each register should start with a value of 0.
            //There aren't that many instructions, so it shouldn't be hard to figure out what they do.Here's what you 
            //determine:
            // - snd X plays a sound with a frequency equal to the value of X.
            // - set X Y sets register X to the value of Y.
            // - add X Y increases register X by the value of Y.
            // - mul X Y sets register X to the result of multiplying the value contained in register X by the value of Y.
            // - mod X Y sets register X to the remainder of dividing the value contained in register X by the value of Y
            //   (that is, it sets X to the result of X modulo Y).
            // - rcv X recovers the frequency of the last sound played, but only when the value of X is not zero. 
            //   (If it is zero, the command does nothing.)
            // - jgz X Y jumps with an offset of the value of Y, but only if the value of X is greater than zero. (An 
            //   offset of 2 skips the next instruction, an offset of - 1 jumps to the previous instruction, and so on.)
            //Many of the instructions can take either a register (a single letter) or a number. The value of a register is 
            //the integer it contains; the value of a number is that number.
            //After each jump instruction, the program continues with the instruction to which the jump jumped. After any 
            //other instruction, the program continues with the next instruction. Continuing (or jumping) off either end of 
            //the program terminates it.
            //For example:
            // set a 1
            // add a 2
            // mul a a
            // mod a 5
            // snd a
            // set a 0
            // rcv a
            // jgz a -1
            // set a 1
            // jgz a -2
            // - The first four instructions set a to 1, add 2 to it, square it, and then set it to itself modulo 5, 
            //   resulting in a value of 4. 
            // - Then, a sound with frequency 4 (the value of a) is played.
            // - After that, a is set to 0, causing the subsequent rcv and jgz instructions to both be skipped (rcv because 
            //   a is 0, and jgz because a is not greater than 0).
            // - Finally, a is set to 1, causing the next jgz instruction to activate, jumping back two instructions to 
            //   another jump, which jumps again to the rcv, which ultimately triggers the recover operation.
            //At the time the recover operation is executed, the frequency of the last sound played is 4.
            //What is the value of the recovered frequency (the value of the most recently played sound) the first time a 
            //rcv instruction is executed with a non-zero value?

            IDictionary<string, long> registers = new Dictionary<string, long>();
            IDictionary<string, long> sounds = new Dictionary<string, long>();
            string lastPlayedSound = null;
            IList<string[]> instructions = new List<string[]>();

            string line;
            using (StringReader reader = new StringReader(Input))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    string[] instruction = line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    if (!registers.Keys.Contains(instruction[1]))
                    {
                        registers.Add(instruction[1], 0);
                        sounds.Add(instruction[1], 0);
                    }
                    instructions.Add(instruction);
                }
            }

            for (var i = 0; i < instructions.Count; i++)
            {
                string[] instruction = instructions[i];
                Debug.WriteLine("instruction: " + instruction[0]);
                if (instruction[0] == "snd")
                {
                    sounds[instruction[1]] = registers[instruction[1]];
                    lastPlayedSound = instruction[1];
                }
                else if (instruction[0] == "set")
                {
                    if (long.TryParse(instruction[2], out long value))
                    {
                        registers[instruction[1]] = value;
                    }
                    else
                    {
                        registers[instruction[1]] = registers[instruction[2]];
                    }
                }
                else if (instruction[0] == "add")
                {
                    if (long.TryParse(instruction[2], out long value))
                    {
                        registers[instruction[1]] += value;
                    }
                    else
                    {
                        registers[instruction[1]] += registers[instruction[2]];
                    }
                }
                else if (instruction[0] == "mul")
                {
                    if (long.TryParse(instruction[2], out long value))
                    {
                        registers[instruction[1]] *= value;
                    }
                    else
                    {
                        registers[instruction[1]] *= registers[instruction[2]];
                    }
                }
                else if (instruction[0] == "mod")
                {
                    if (long.TryParse(instruction[2], out long value))
                    {
                        registers[instruction[1]] %= value;
                    }
                    else
                    {
                        registers[instruction[1]] %= registers[instruction[2]];
                    }
                }
                else if (instruction[0] == "rcv")
                {
                    if (!long.TryParse(instruction[1], out long value))
                    {
                        value = registers[instruction[1]];
                    }

                    if (value > 0)
                    {
                        registers[instruction[1]] = sounds[instruction[1]];
                        break;
                    }
                }
                else //jgz
                {
                    if (!long.TryParse(instruction[1], out long value))
                    {
                        value = registers[instruction[1]];
                    }

                    if (value > 0)
                    {
                        if (!int.TryParse(instruction[2], out int jumpValue))
                        {
                            jumpValue = (int)registers[instruction[2]];
                        }

                        i += jumpValue - 1;
                    }
                }
            }

            Debug.WriteLine($"Last played sound: {lastPlayedSound}");
            Debug.WriteLine($"Frequency: " + sounds[lastPlayedSound]);
        }

        public static void Part2()
        {
            //This assembly code isn't about sound at all - it's meant to be run twice at the same time.
            //Each running copy of the program has its own set of registers and follows the code independently - in fact, 
            //the programs don't even necessarily run at the same speed. To coordinate, they use the send (snd) and receive 
            //(rcv) instructions:
            // - snd X sends the value of X to the other program. These values wait in a queue until that program is ready 
            //   to receive them. Each program has its own message queue, so a program can never receive a message it sent.
            // - rcv X receives the next value and stores it in register X. If no values are in the queue, the program waits 
            //   for a value to be sent to it. Programs do not continue to the next instruction until they have received a 
            //   value. Values are received in the order they are sent.
            //Each program also has its own program ID (one 0 and the other 1); the register p should begin with this value.
            //For example:
            // snd 1
            // snd 2
            // snd p
            // rcv a
            // rcv b
            // rcv c
            // rcv d
            //Both programs begin by sending three values to the other. Program 0 sends 1, 2, 0; program 1 sends 1, 2, 1. 
            //Then, each program receives a value (both 1) and stores it in a, receives another value (both 2) and stores it 
            //in b, and then each receives the program ID of the other program (program 0 receives 1; program 1 receives 0) 
            //and stores it in c. Each program now sees a different value in its own copy of register c.
            //Finally, both programs try to rcv a fourth time, but no data is waiting for either of them, and they reach a 
            //deadlock. When this happens, both programs terminate.
            //It should be noted that it would be equally valid for the programs to run at different speeds; for example, 
            //program 0 might have sent all three values and then stopped at the first rcv before program 1 executed even its 
            //first instruction.
            //Once both of your programs have terminated (regardless of what caused them to do so), how many times did program 
            //1 send a value?

            IList<long> queueProgram0 = new List<long>();
            IList<long> queueProgram1 = new List<long>();
            bool program0Waiting = false;
            int program0sendCount = 0;
            bool program1Waiting = false;
            int program1sendCount = 0;
            Thread threadProgram0 = new Thread(() => { Program(0, ref queueProgram0, ref queueProgram1, ref program0Waiting, ref program0sendCount); });
            Thread threadProgram1 = new Thread(() => { Program(1, ref queueProgram1, ref queueProgram0, ref program1Waiting, ref program1sendCount); });
            threadProgram0.Start();
            threadProgram1.Start();

            while (true)
            {
                if (program0Waiting && program1Waiting)
                {
                    Debug.WriteLine("Deadlocked");
                    break;
                }
            }

            threadProgram0.Abort();
            threadProgram1.Abort();

            Debug.WriteLine("Items sent by Program 0: " + program0sendCount);
            Debug.WriteLine("Items sent by Program 1: " + program1sendCount);
        }

        private static void Program(int id, ref IList<long> ownQueue, ref IList<long> otherQueue, ref bool waitIndicator, ref int sendCount)
        {
            IDictionary<string, long> registers = new Dictionary<string, long>
            {
                { "p", id }
            };
            IList<string[]> instructions = new List<string[]>();

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
                Debug.WriteLine("instruction ["+ id +"]: " + instruction[0]);
                if (instruction[0] == "snd")
                {
                    if (!long.TryParse(instruction[1], out long value))
                    {
                        value = registers[instruction[1]];
                    }
                    Debug.WriteLine("send ["+ id +"]: " + value);
                    otherQueue.Add(value);
                    sendCount++;
                }
                else if (instruction[0] == "set")
                {
                    if (long.TryParse(instruction[2], out long value))
                    {
                        registers[instruction[1]] = value;
                    }
                    else
                    {
                        registers[instruction[1]] = registers[instruction[2]];
                    }
                }
                else if (instruction[0] == "add")
                {
                    if (long.TryParse(instruction[2], out long value))
                    {
                        registers[instruction[1]] += value;
                    }
                    else
                    {
                        registers[instruction[1]] += registers[instruction[2]];
                    }
                }
                else if (instruction[0] == "mul")
                {
                    if (long.TryParse(instruction[2], out long value))
                    {
                        registers[instruction[1]] *= value;
                    }
                    else
                    {
                        registers[instruction[1]] *= registers[instruction[2]];
                    }
                }
                else if (instruction[0] == "mod")
                {
                    if (long.TryParse(instruction[2], out long value))
                    {
                        registers[instruction[1]] %= value;
                    }
                    else
                    {
                        registers[instruction[1]] %= registers[instruction[2]];
                    }
                }
                else if (instruction[0] == "rcv")
                {
                    while (ownQueue.Count == 0)
                    {
                        waitIndicator = true;
                    }

                    waitIndicator = false;
                    registers[instruction[1]] = ownQueue[0];
                    ownQueue.RemoveAt(0);
                }
                else //jgz
                {
                    if (!long.TryParse(instruction[1], out long value))
                    {
                        value = registers[instruction[1]];
                    }

                    if (value > 0)
                    {
                        if (!int.TryParse(instruction[2], out int jumpValue))
                        {
                            jumpValue = (int)registers[instruction[2]];
                        }

                        i += jumpValue - 1;
                    }
                }
            }
        }

        private static string TestInput = @"set a 1
add a 2
mul a a
mod a 5
snd a
set a 0
rcv a
jgz a -1
set a 1
jgz a -2";

        private static string TestInput2 = @"snd 1
snd 2
snd p
rcv a
rcv b
rcv c
rcv d";

        private static string Input = @"set i 31
set a 1
mul p 17
jgz p p
mul a 2
add i -1
jgz i -2
add a -1
set i 127
set p 826
mul p 8505
mod p a
mul p 129749
add p 12345
mod p a
set b p
mod b 10000
snd b
add i -1
jgz i -9
jgz a 3
rcv b
jgz b -1
set f 0
set i 126
rcv a
rcv b
set p a
mul p -1
add p b
jgz p 4
snd a
set a b
jgz 1 3
snd b
set f 1
add i -1
jgz i -11
snd a
jgz f -16
jgz a -19";
    }
}
