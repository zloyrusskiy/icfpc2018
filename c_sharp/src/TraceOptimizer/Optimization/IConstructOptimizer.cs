using TraceOptimizer.Domain;

namespace TraceOptimizer.Optimization
{
    public interface IConstructOptimizer
    {
        BotProgram Optimize(Model3D modelToBuild);
    }
}