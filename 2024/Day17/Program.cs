using System.ComponentModel.Design;
using System.Numerics;
using System.Text.RegularExpressions;

using Microsoft.Win32;

namespace Day17
{
    class Program
    {
        class Registers
        {
            public int IP;

            public long A;
            public long B;
            public long C;

            public List<long> Out = [];

            public Registers ()
            {

            }

            public Registers (Registers other)
                => (A, B, C) = (other.A, other.B, other.C);

            public string OutString => string.Join (",", Out);
        }

        delegate void Instruction (Registers registers, long operand);

        static class Instructions
        {
            public static Instruction Get (long instruction) => instruction switch {
                0 => adv,
                1 => bxl,
                2 => bst,
                3 => jnz,
                4 => bxc,
                5 => output,
                6 => bdv,
                7 => cdv,
                _ => throw new NotSupportedException ()
            };

            static long ComboOperand (Registers registers, long operand) => operand switch {
                0 => operand,
                1 => operand,
                2 => operand,
                3 => operand,
                4 => registers.A,
                5 => registers.B,
                6 => registers.C,
                _ => throw new InvalidOperationException (),
            };

            static long IntPow (long operand, long pow)
            {
                long val = 1;
                while (pow-- > 0)
                    val *= operand;
                return val;
            }
            // 0
            static void adv (Registers registers, long operand)
                => registers.A = registers.A / IntPow (2, ComboOperand (registers, operand));
            // 1
            static void bxl (Registers registers, long operand)
                => registers.B ^= operand;
            // 2
            static void bst (Registers registers, long operand)
                => registers.B = ComboOperand (registers, operand) & 7;
            // 3
            static void jnz (Registers registers, long operand)
                => registers.IP = registers.A == 0 ? registers.IP : (int) operand;
            // 4
            static void bxc (Registers registers, long operand)
                => registers.B ^= registers.C;
            // 5
            static void output (Registers registers, long operand)
                => registers.Out.Add (ComboOperand (registers, operand) & 7);
            // 6
            static void bdv (Registers registers, long operand)
                => registers.B = registers.A / IntPow (2, ComboOperand (registers, operand));
            // 7
            static void cdv (Registers registers, long operand)
                => registers.C = registers.A / IntPow (2, ComboOperand (registers, operand));
        }

        static void Main (string[] args)
        {
            var lines = File.ReadAllLines ("input.txt");
            var registers = new Registers {
                A = int.Parse (lines.Single (t => t.StartsWith ("Register A")).Split (": ")[1]),
                B = int.Parse (lines.Single (t => t.StartsWith ("Register B")).Split (": ")[1]),
                C = int.Parse (lines.Single (t => t.StartsWith ("Register C")).Split (": ")[1])
            };

            List<long> program = lines
                .Single (t => t.StartsWith ("Program"))
                .Split (": ")[1]
                .Split (",")
                .Select (long.Parse)
                .ToList ();


            var q1Registers = new Registers (registers);
            Execute (program, q1Registers);
            Console.WriteLine ($"Q1: {q1Registers.OutString}");

            // part 2
            //
            // The output always takes the last 3 bits.
            // What can I do with that? Nothing? Something?
            //
            // Ok - the internet says i can backtrack to recreate
            // the input and bitshift my way to success. Let's see if
            // i understood that correctly...
            //

            Console.WriteLine ($"Q2: {Discover (program, 0, program.Count - 1)}");
        }

        static long Discover (List<long> program, long seed, int programIndex)
        {
            for (long i = seed; i < seed + 8; i++) {
                var r = new Registers { A = i };
                Execute (program, r);
                if (r.Out.SequenceEqual (program.Skip (programIndex))) {
                    if (programIndex == 0)
                        return i;

                    // maybe?
                    var result = Discover (program, i << 3, programIndex - 1);
                    if (result != -1)
                        return result;
                }
            }
            return -1;
        }

        static void Execute (List<long> program, Registers registers)
        {
            while (registers.IP < program.Count) {
                var instruction = Instructions.Get (program[registers.IP++]);
                var operand = program[registers.IP++];
                instruction (registers, operand);
            }
        }
    }
}
