using TraceOptimizer.Domain;

namespace TraceOptimizer.Optimization
{
    public interface IDeconstructOptimizer
    {
        BotProgram Optimize(Model3D modelToBuild);
    }
}