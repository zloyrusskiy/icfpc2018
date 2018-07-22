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

            var optimizer = new OptimizerV1();
            var program = optimizer.Optimize(model);

            var serializer = new BotProgramToStdoutSerializer();
            serializer.Serialize(program);
        }
    }
}
