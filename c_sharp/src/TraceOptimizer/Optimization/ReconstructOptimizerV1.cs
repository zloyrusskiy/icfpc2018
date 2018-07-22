using System;
using System.Collections.Generic;
using System.Linq;
using TraceOptimizer.Domain;
using TraceOptimizer.Geometry;
using TraceOptimizer.Optimization.Commands;
using TraceOptimizer.Optimization.Paths;
using TraceOptimizer.Voxels;

namespace TraceOptimizer.Optimization
{
    public class ReconstructOptimizerV1 : IReconstructOptimizer
    {
        private BotProgram _program;

        private Scene _scene;

        private Model3D _modelToBuild;

        private int _currentYLevel;

        private int _voxelsToFill;

        public BotProgram Optimize(Model3D modelFrom, Model3D modelTo)
        {
            _program = new BotProgram();
            _scene = Scene.FromModel(modelFrom);
            _modelToBuild = modelTo;
            _currentYLevel = modelFrom.Resolution - 1;

            OptimizeInternal();

            return _program;
        }

        private void OptimizeInternal()
        {
            // TODO: We need to remove Flip commands at all
            ApplyCommand(new FlipCommand());

            DeconstructModel();

            ConstructModel();

            // TODO: We need to remove Flip commands at all
            ApplyCommand(new FlipCommand());

            MoveBotToPoint(Point3D.Origin());

            HaltProgram();
        }

        private void DeconstructModel()
        {
            _voxelsToFill = _scene.SceneState.Matrix.FullVoxels().Count();

            while (MissingVoxelsExist())
            {
                // stopWatch.Restart();

                (Point3D voxelPoint, Point3D pointToMove) = FindNextVoxelPointToDeconstruct();
                // Console.WriteLine($"Finding next voxel to fill took: {stopWatch.ElapsedMilliseconds}ms");

                MoveBotToPoint(pointToMove);
                // Console.WriteLine($"Finding path to voxel to fill took: {stopWatch.ElapsedMilliseconds}ms");

                VoidVoxelPoint(voxelPoint);
                // Console.WriteLine($"Filling the voxel took: {stopWatch.ElapsedMilliseconds}ms");

                // Console.WriteLine($"One bot step took: {stopWatch.ElapsedMilliseconds}ms");
            }
        }

        private void ConstructModel()
        {
            _voxelsToFill = _modelToBuild.NumberOfFullVoxels();

            while (MissingVoxelsExist())
            {
                // stopWatch.Restart();

                (Point3D voxelPoint, Point3D pointToMove) = FindNextVoxelPointToConstruct();
                // Console.WriteLine($"Finding next voxel to fill took: {stopWatch.ElapsedMilliseconds}ms");

                MoveBotToPoint(pointToMove);
                // Console.WriteLine($"Finding path to voxel to fill took: {stopWatch.ElapsedMilliseconds}ms");

                FillVoxelPoint(voxelPoint);
                // Console.WriteLine($"Filling the voxel took: {stopWatch.ElapsedMilliseconds}ms");

                // Console.WriteLine($"One bot step took: {stopWatch.ElapsedMilliseconds}ms");
            }
        }

        private void VoidVoxelPoint(Point3D voxelPoint)
        {
            var bot = _scene.GetFirstBot();
            var botVector = Vector3D.FromPoint(bot.Current);
            var voxelVector = Vector3D.FromPoint(voxelPoint);
            _voxelsToFill -= 1;
            ApplyCommand(new VoidCommand(voxelVector - botVector));
        }

        private void FillVoxelPoint(Point3D voxelPoint)
        {
            var bot = _scene.GetFirstBot();
            var botVector = Vector3D.FromPoint(bot.Current);
            var voxelVector = Vector3D.FromPoint(voxelPoint);
            _voxelsToFill -= 1;
            ApplyCommand(new FillCommand(voxelVector - botVector));
        }

        private bool MissingVoxelsExist()
        {
            return _voxelsToFill > 0;
        }

        private (Point3D, Point3D) FindNextVoxelPointToDeconstruct()
        {
            var points = FindNotVoidedVoxelsOnYLayer().ToList();
            while (points.Count == 0)
            {
                _currentYLevel -= 1;
                points = FindNotVoidedVoxelsOnYLayer().ToList();
            }

            return FindClosestPointNearNotFilledVoxels(points);
        }

        // Return point NEAR voxel to fill
        private (Point3D, Point3D) FindNextVoxelPointToConstruct()
        {
            var points = FindNotFilledVoxelsOnYLayer().ToList();
            if (points.Count == 0)
            {
                _currentYLevel += 1;
                points = FindNotFilledVoxelsOnYLayer().ToList();
            }

            return FindClosestPointNearNotFilledVoxels(points);
        }

        private void MoveBotToPoint(Point3D voxelPoint)
        {
            var bot = _scene.GetFirstBot();

            var path = PathFinder.FindPathBetweenWithOnlyStraightMoves(
                bot.Current,
                voxelPoint,
                _scene);

            foreach (var command in path)
            {
                ApplyCommand(command);
            }
        }

        private void HaltProgram()
        {
            ApplyCommand(new HaltCommand());
        }

        private void ApplyCommand(BotCommand command)
        {
            _scene.ApplyCommand(command);
            _program.AddCommand(command);
        }

        private IEnumerable<Point3D> FindNotFilledVoxelsOnYLayer()
        {
            foreach (var point in _modelToBuild.Matrix.FullVoxelsOnYLevel(_currentYLevel))
            {
                if (_scene.SceneState.Matrix[point] == VoxelStatus.Empty)
                {
                    yield return point;
                }
            }
        }

        private IEnumerable<Point3D> FindNotVoidedVoxelsOnYLayer()
        {
            return _scene.SceneState.Matrix.FullVoxelsOnYLevel(_currentYLevel);
        }

        private sealed class PriorityQueueComparer : IComparer<Vector3D>
        {
            private readonly Vector3D _destination;

            public PriorityQueueComparer(Vector3D destination)
            {
                _destination = destination;
            }
            public int Compare(Vector3D x, Vector3D y)
            {
                var from = (_destination - x).ManhattanLength;
                var to = (_destination - y).ManhattanLength;

                return from - to;
            }
        }
        private (Point3D, Point3D) FindClosestPointNearNotFilledVoxels(List<Point3D> points)
        {
            var bot = _scene.GetFirstBot();
            var pointsSet = new C5.IntervalHeap<Vector3D>(
                new PriorityQueueComparer(Vector3D.FromPoint(bot.Current))
            );
            var map = new Dictionary<Point3D, Point3D>();
            var boundBox = _scene.PadBoundBox;

            foreach (var pointToFill in points)
            {
                foreach (var candidatePoint in EnumeratePointsToFillFrom(pointToFill))
                {
                    if (!boundBox.Contains(candidatePoint)) continue;
                    if (_scene.SceneState.Matrix[candidatePoint] == VoxelStatus.Full) continue;

                    map[candidatePoint] = pointToFill;
                    pointsSet.Add(Vector3D.FromPoint(candidatePoint));
                }
            }

            var pointToMove = pointsSet.DeleteMin().ToPoint();

            return (map[pointToMove], pointToMove);
        }

        private static int[] offsets = new[] { -1, 0, 1 };
        private static int[] dy_offsets = new[] { 0, 1 };
        private IEnumerable<Point3D> EnumeratePointsToFillFrom(Point3D point)
        {
            foreach (var dx in offsets)
                foreach (var dy in dy_offsets)
                    foreach (var dz in offsets)
                    {
                        if ((dx == 0 && dy == 0 && dz == 0) ||
                            (dx == -1 && dy == -1 && dz == -1) ||
                            (dx == 1 && dy == 1 && dz == 1) ||
                            (dx == 1 && dy == 1 && dz == -1) ||
                            (dx == 1 && dy == -1 && dz == 1) ||
                            (dx == -1 && dy == 1 && dz == 1) ||
                            (dx == -1 && dy == -1 && dz == 1) ||
                            (dx == 1 && dy == -1 && dz == -1) ||
                            (dx == -1 && dy == 1 && dz == -1)) continue;

                        yield return new Point3D(point.X + dx, point.Y + dy, point.Z + dz);
                    }
        }
    }
}