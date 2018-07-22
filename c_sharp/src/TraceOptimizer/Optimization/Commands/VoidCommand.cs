using System;
using System.Linq;
using TraceOptimizer.Domain;
using TraceOptimizer.Geometry;
using TraceOptimizer.Voxels;

namespace TraceOptimizer.Optimization.Commands
{
    public class VoidCommand : BotCommand
    {
        public VoidCommand(Vector3D nd)
        {
            if (!nd.IsNear)
                throw new ArgumentOutOfRangeException($"Void command supports only near coordinate differences.");

            Nd = nd;
        }

        public Vector3D Nd { get; }

        public override SceneState Apply(SceneState sceneState)
        {
            var bot = sceneState.Bots.First();
            var vectorToFill = Vector3D.FromPoint(bot.Current) + Nd;
            var pointToFill = vectorToFill.ToPoint();

            if (sceneState.Matrix[pointToFill] == VoxelStatus.Full)
            {
                sceneState.Matrix[pointToFill] = VoxelStatus.Empty;
                return sceneState.ChangeEnergy(-12);
            }
            else
            {
                return sceneState.ChangeEnergy(3);
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