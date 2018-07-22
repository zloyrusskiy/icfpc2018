using System;
using System.Linq;
using TraceOptimizer.Domain;
using TraceOptimizer.Geometry;

namespace TraceOptimizer.Optimization.Commands
{
    public class LMoveCommand : BotCommand
    {
        public LMoveCommand(Vector3D sld1, Vector3D sld2)
        {
            if (!sld1.IsShortLinear || !sld2.IsShortLinear)
                throw new ArgumentOutOfRangeException($"L move command supports only short linear coordinate differences.");

            Sld1 = sld1;
            Sld2 = sld2;
        }

        public Vector3D Sld1 { get; }

        public Vector3D Sld2 { get; }

        public override SceneState Apply(SceneState sceneState)
        {
            var bot = sceneState.Bots.First();
            var vectorToMove = Vector3D.FromPoint(bot.Current) +
                Sld1 + Sld2;
            var pointToMove = vectorToMove.ToPoint();

            bot.MoveTo(pointToMove);

            return sceneState.ChangeEnergy(2 * (Sld1.ManhattanLength + 2 + Sld2.ManhattanLength));
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