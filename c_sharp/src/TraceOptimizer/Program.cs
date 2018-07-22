using System;
using TraceOptimizer.Domain;
using TraceOptimizer.Optimization;

namespace TraceOptimizer
{
    class Program
    {
        static void Main(string[] args)
        {
            var modelProvider = new Model3DFromStdinProvider();
            var model = modelProvider.GetModel();

            BotProgram program;
            var action = args[0];

            switch (action)
            {
                case "construct":
                    var constructor = new ConstructOptimizerV1();
                    program = constructor.Optimize(model);
                    break;

                case "deconstruct":
                    var deconstructor = new DeconstructOptimizerV1();
                    program = deconstructor.Optimize(model);
                    break;

                default:
                    Console.WriteLine($"Unsupported parameter: {action}");
                    return;
            }

            var serializer = new BotProgramToStdoutSerializer();
            serializer.Serialize(program);
        }
    }
}
