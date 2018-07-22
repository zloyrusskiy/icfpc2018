using System;
using System.Linq;
using TraceOptimizer.Domain;
using TraceOptimizer.Geometry;
using TraceOptimizer.Voxels;

namespace TraceOptimizer.Optimization.Commands
{
    public sealed class FillCommand : BotCommand
    {
        public FillCommand(Vector3D nd)
        {
            if (!nd.IsNear)
                throw new ArgumentOutOfRangeException($"Fill command supports only near coordinate differences.");

            Nd = nd;
        }

        public Vector3D Nd { get; }

        public override SceneState Apply(SceneState sceneState)
        {
            var bot = sceneState.Bots.First();
            var vectorToFill = Vector3D.FromPoint(bot.Current) + Nd;
            var pointToFill = vectorToFill.ToPoint();

            if (sceneState.Matrix[pointToFill] == VoxelStatus.Empty)
            {
                sceneState.Matrix[pointToFill] = VoxelStatus.Full;
                return sceneState.ChangeEnergy(12);
            }
            else
            {
                return sceneState.ChangeEnergy(6);
            }
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