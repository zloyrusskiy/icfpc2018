using TraceOptimizer.Domain;

namespace TraceOptimizer.Optimization
{
    public interface IOptimizer
    {
        BotProgram Optimize(Model3D modelToBuild);
    }
}