using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TraceOptimizer.Geometry;
using TraceOptimizer.Optimization.Commands;
using TraceOptimizer.Voxels;

namespace TraceOptimizer.Domain
{
    public sealed class Scene
    {
        private Scene(int resolution)
        {
            SceneState = SceneState.New(resolution);
        }

        public SceneState SceneState { get; private set; }

        public NanoBot GetFirstBot()
        {
            return SceneState.Bots.First();
        }

        public Cuboid PadBoundBox =>
            Cuboid.FromPoints(
                Point3D.Origin(),
                new Point3D(
                    SceneState.Matrix.Resolution - 1,
                    SceneState.Matrix.Resolution - 1,
                    SceneState.Matrix.Resolution - 1));

        public Scene ApplyCommand(BotCommand command)
        {
            SceneState = command.Apply(SceneState);
            return this;
        }

        public Cuboid[] GetObstacles()
        {
            return SceneState.Matrix.FullVoxels()
                .Select(p => Cuboid.FromPoints(p, p))
                .ToArray();
        }

        public int FilledVoxelsCount()
        {
            return SceneState.Matrix.FullVoxels().Count();
        }

        public static Scene New(int resolution)
        {
            return new Scene(resolution);
        }
    }
}