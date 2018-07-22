using System;
using System.Linq;
using TraceOptimizer.Domain;
using TraceOptimizer.Geometry;

namespace TraceOptimizer.Optimization.Commands
{
    public sealed class StraightMoveCommand : BotCommand
    {
        public StraightMoveCommand(Vector3D longDiff)
        {
            if (!longDiff.IsLongLinear)
                throw new ArgumentOutOfRangeException($"Straight move command supports only long linear coordinate differences.");
            LongDiff = longDiff;
        }

        public Vector3D LongDiff { get; }

        public override SceneState Apply(SceneState sceneState)
        {
            var bot = sceneState.Bots.First();
            var vectorToMove = Vector3D.FromPoint(bot.Current) + LongDiff;
            var pointToMove = vectorToMove.ToPoint();

            bot.MoveTo(pointToMove);

            return sceneState.ChangeEnergy(2 * LongDiff.ManhattanLength);
        }

        public override void Visit(AbstractVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override T Visit<T>(AbstractVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}