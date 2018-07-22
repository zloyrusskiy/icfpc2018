using TraceOptimizer.Domain;
using System.Collections.Generic;
using TraceOptimizer.Optimization.Commands;

namespace TraceOptimizer.Optimization
{
    public interface IMovementOptimizer
    {
        List<BotCommand> OptimizeSMoves(List<BotCommand> movementsToOptimize);
    }
}