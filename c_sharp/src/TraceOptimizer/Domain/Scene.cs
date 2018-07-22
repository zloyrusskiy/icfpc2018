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
        private Scene()
        {

        }
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

        public Cuboid[] GetObstaclesAgainstStraightMove(Point3D source)
        {
            var vector = Vector3D.FromPoint(source);
            var padBoundBox = PadBoundBox;
            var obstacles = new List<Cuboid>();

            foreach (var dx in Enumerable.Range(1, 15))
            {
                var newVector = vector + Vector3D.FromPoint(new Point3D(dx, 0, 0));
                var newPoint = newVector.ToPoint();

                if (padBoundBox.Contains(newPoint) &&
                    this.SceneState.Matrix[newPoint] == VoxelStatus.Full)
                {
                    obstacles.Add(Cuboid.FromPoints(newPoint, newPoint));
                    break;
                }
            }

            foreach (var dx in Enumerable.Range(1, 15))
            {
                var newVector = vector + Vector3D.FromPoint(new Point3D(-dx, 0, 0));
                var newPoint = newVector.ToPoint();

                if (padBoundBox.Contains(newPoint) &&
                    this.SceneState.Matrix[newPoint] == VoxelStatus.Full)
                {
                    obstacles.Add(Cuboid.FromPoints(newPoint, newPoint));
                    break;
                }
            }

            foreach (var dy in Enumerable.Range(1, 15))
            {
                var newVector = vector + Vector3D.FromPoint(new Point3D(0, dy, 0));
                var newPoint = newVector.ToPoint();

                if (padBoundBox.Contains(newPoint) &&
                    this.SceneState.Matrix[newPoint] == VoxelStatus.Full)
                {
                    obstacles.Add(Cuboid.FromPoints(newPoint, newPoint));
                    break;
                }
            }

            foreach (var dy in Enumerable.Range(1, 15))
            {
                var newVector = vector + Vector3D.FromPoint(new Point3D(0, -dy, 0));
                var newPoint = newVector.ToPoint();

                if (padBoundBox.Contains(newPoint) &&
                    this.SceneState.Matrix[newPoint] == VoxelStatus.Full)
                {
                    obstacles.Add(Cuboid.FromPoints(newPoint, newPoint));
                    break;
                }
            }

            foreach (var dz in Enumerable.Range(1, 15))
            {
                var newVector = vector + Vector3D.FromPoint(new Point3D(0, 0, dz));
                var newPoint = newVector.ToPoint();

                if (padBoundBox.Contains(newPoint) &&
                    this.SceneState.Matrix[newPoint] == VoxelStatus.Full)
                {
                    obstacles.Add(Cuboid.FromPoints(newPoint, newPoint));
                    break;
                }
            }

            foreach (var dz in Enumerable.Range(1, 15))
            {
                var newVector = vector + Vector3D.FromPoint(new Point3D(0, 0, -dz));
                var newPoint = newVector.ToPoint();

                if (padBoundBox.Contains(newPoint) &&
                    this.SceneState.Matrix[newPoint] == VoxelStatus.Full)
                {
                    obstacles.Add(Cuboid.FromPoints(newPoint, newPoint));
                    break;
                }
            }

            return obstacles.ToArray();
        }

        public int FilledVoxelsCount()
        {
            return SceneState.Matrix.FullVoxels().Count();
        }

        public static Scene New(int resolution)
        {
            return new Scene(resolution);
        }

        public static Scene FromModel(Model3D model)
        {
            return new Scene
            {
                SceneState = SceneState.FromMatrix(model.Matrix)
            };
        }
    }
}