using System.Numerics;
using System.Text.RegularExpressions;

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

            public Registers()
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
                4 => registers.A,
                5 => registers.B,
                6 => registers.C,
                7 => throw new InvalidOperationException (),
                _ => operand,
            };

            static long IntPow (long operand, long pow)
            {
                long val = 1;
                while (pow-- > 0)
                    val *= operand;
                return val;
            }

            static void adv (Registers registers, long operand)
                => registers.A = registers.A / IntPow (2, ComboOperand (registers, operand));

            static void bdv (Registers registers, long operand)
                => registers.B = registers.A / IntPow (2, ComboOperand (registers, operand));

            static void cdv (Registers registers, long operand)
                => registers.C = registers.A / IntPow (2, ComboOperand (registers, operand));

            static void bxl (Registers registers, long operand)
                => registers.B ^= operand;

            static void bst (Registers registers, long operand)
                => registers.B = ComboOperand (registers, operand) & 7;

            static void jnz (Registers registers, long operand)
                => registers.IP = registers.A == 0 ? registers.IP : (int) operand;

            static void bxc (Registers registers, long operand)
                => registers.B ^= registers.C;

            static void output (Registers registers, long operand)
                => registers.Out.Add (ComboOperand (registers, operand) & 7);
        }

        static void Main (string[] args)
        {
            var lines = File.ReadAllLines ("input.txt");
            var registers = new Registers {
                A = int.Parse (lines.Single (t => t.StartsWith ("Register A")).Split (": ")[1]),
                B = int.Parse (lines.Single (t => t.StartsWith ("Register B")).Split (": ")[1]),
                C = int.Parse (lines.Single (t => t.StartsWith ("Register C")).Split (": ")[1])
            };

            ReadOnlyMemory<long> program = lines
                .Single (t => t.StartsWith ("Program"))
                .Split (": ")[1]
                .Split (",")
                .Select (long.Parse)
                .ToArray ();


            var q1Registers = new Registers (registers);
            Execute (program, q1Registers);
            Console.WriteLine ($"Q1: {q1Registers.OutString}");
        }

        static void Execute (ReadOnlyMemory<long> program, Registers registers)
        {
            while (registers.IP < program.Length) {
                var instruction = Instructions.Get (program.Span[registers.IP++]);
                var operand = program.Span[registers.IP++];
                instruction (registers, operand);
            }
        }
    }
}
