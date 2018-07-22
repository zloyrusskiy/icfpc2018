using System;
using TraceOptimizer.Optimization.Commands;

namespace TraceOptimizer.Optimization
{
    public class BotProgramToStdoutSerializer : IBotProgramSerializer
    {
        private sealed class ConsoleWriteLineVisitor : AbstractVisitor
        {
            public override void Visit(FillCommand command)
            {
                var p = command.Nd;
                Console.WriteLine($"Fill <{p.X},{p.Y},{p.Z}>");
            }

            public override void Visit(FlipCommand command)
            {
                Console.WriteLine("Flip");
            }

            public override void Visit(HaltCommand command)
            {
                Console.WriteLine("Halt");
            }

            public override void Visit(LMoveCommand command)
            {
                var p = command.Sld1;
                var q = command.Sld2;
                Console.WriteLine($"LMove <{p.X},{p.Y},{p.Z}> <{q.X},{q.Y},{q.Z}>");
            }

            public override void Visit(StraightMoveCommand command)
            {
                var p = command.LongDiff;
                Console.WriteLine($"SMove <{p.X},{p.Y},{p.Z}>");
            }

            public override void Visit(WaitCommand command)
            {
                Console.WriteLine("Wait");
            }

            public override void Visit(VoidCommand command)
            {
                var p = command.Nd;
                Console.WriteLine($"Void <{p.X},{p.Y},{p.Z}>");
            }
        }

        public void Serialize(BotProgram program)
        {
            var serializer = new ConsoleWriteLineVisitor();

            foreach (var command in program.Commands)
            {
                command.Visit(serializer);
            }
        }
    }
}