using System;
using System.Collections.Generic;
using TraceOptimizer.Geometry;
using TraceOptimizer.Optimization.Commands;

namespace TraceOptimizer.Optimization
{
    public class MovementOptimizerV1 : IMovementOptimizer
    {
        public List<BotCommand> OptimizeSMoves(List<BotCommand> movementsToOptimize)
        {
            var newCommands = new List<BotCommand>();

            for (var i = 0; i < movementsToOptimize.Count; i++)
            {
                var current = movementsToOptimize[i];
                if (i + 1 < movementsToOptimize.Count)
                {
                    var next = movementsToOptimize[i + 1];
                    if (current is StraightMoveCommand && next is StraightMoveCommand)
                    {
                        var currentStraight = (StraightMoveCommand)current;
                        var nextStraight = (StraightMoveCommand)next;
                        if (MovementOptimizerV1.canUniteToLMove(currentStraight.LongDiff, nextStraight.LongDiff) || MovementOptimizerV1.canUniteToSMove(currentStraight.LongDiff, nextStraight.LongDiff))
                        {
                            if (MovementOptimizerV1.canUniteToLMove(currentStraight.LongDiff, nextStraight.LongDiff))
                            {
                                newCommands.Add(
                                    new LMoveCommand(currentStraight.LongDiff, nextStraight.LongDiff)
                                );
                            }
                            else if (MovementOptimizerV1.canUniteToSMove(currentStraight.LongDiff, nextStraight.LongDiff))
                            {
                                newCommands.Add(
                                    new StraightMoveCommand(currentStraight.LongDiff + nextStraight.LongDiff)
                                );
                            }
                            i++;
                            continue;
                        }

                    }
                }
                newCommands.Add(current);
            }

            return newCommands;
        }

        public static bool canUniteToLMove(Vector3D d1, Vector3D d2) =>
            d1.IsShortLinear &&
            d2.IsShortLinear &&
            d1.IsLinearlyDifferent(d2);

        public static bool canUniteToSMove(Vector3D d1, Vector3D d2) =>
            d1.IsLinearlySame(d2) &&
            (d1 + d2).IsLongLinear;
    }
}