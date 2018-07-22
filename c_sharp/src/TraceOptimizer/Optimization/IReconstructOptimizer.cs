using TraceOptimizer.Domain;

namespace TraceOptimizer.Optimization
{
    public interface IReconstructOptimizer
    {
        BotProgram Optimize(Model3D modelFrom, Model3D modelTo);
    }
}