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

            BotProgram program;
            var action = args[0];
            switch (action)
            {
                case "construct":

                    var modelToConstruct = modelProvider.GetModel();
                    var constructor = new ConstructOptimizerV1();
                    program = constructor.Optimize(modelToConstruct);
                    break;

                case "construct_v2":
                    var modelToConstructV2 = modelProvider.GetModel();
                    var constructorV2 = new ConstructOptimizerV2();
                    program = constructorV2.Optimize(modelToConstructV2);
                    break;

                case "deconstruct":
                    var modelToDeconstruct = modelProvider.GetModel();
                    var deconstructor = new DeconstructOptimizerV1();
                    program = deconstructor.Optimize(modelToDeconstruct);
                    break;

                case "reconstruct":
                    var fromModel = modelProvider.GetModel();
                    var toModel = modelProvider.GetModel();
                    var reconstructor = new ReconstructOptimizerV1();
                    program = reconstructor.Optimize(fromModel, toModel);
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
