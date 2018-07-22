using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TraceOptimizer.Domain;
using TraceOptimizer.Geometry;
using TraceOptimizer.Optimization.Commands;
using TraceOptimizer.Optimization.Paths;
using TraceOptimizer.Voxels;

namespace TraceOptimizer.Optimization
{
    public class ConstructOptimizerV1 : IConstructOptimizer
    {
        private Scene _scene;

        private BotProgram _program;

        private Model3D _model;

        private int _voxelsToFill;

        private int _currentYLevel;

        public BotProgram Optimize(Model3D modelToBuild)
        {
            _scene = Scene.New(modelToBuild.Resolution);
            _program = new BotProgram();
            _voxelsToFill = modelToBuild.NumberOfFullVoxels();
            _currentYLevel = 0;
            _model = modelToBuild;

            OptimizeInternal();

            return _program;
        }

        private void OptimizeInternal()
        {
            // TODO: We need to remove Flip commands at all
            ApplyCommand(new FlipCommand());

            // var stopWatch = Stopwatch.StartNew();

            while (MissingVoxelsExist())
            {
                // stopWatch.Restart();

                (Point3D voxelPoint, Point3D pointToMove) = FindNextVoxelPoint();
                // Console.WriteLine($"Finding next voxel to fill took: {stopWatch.ElapsedMilliseconds}ms");

                MoveBotToPoint(pointToMove);
                // Console.WriteLine($"Finding path to voxel to fill took: {stopWatch.ElapsedMilliseconds}ms");

                FillVoxelPoint(voxelPoint);
                // Console.WriteLine($"Filling the voxel took: {stopWatch.ElapsedMilliseconds}ms");

                // Console.WriteLine($"One bot step took: {stopWatch.ElapsedMilliseconds}ms");
            }

            // TODO: We need to remove Flip commands at all
            ApplyCommand(new FlipCommand());

            MoveBotToPoint(Point3D.Origin());

            HaltProgram();
        }

        private void HaltProgram()
        {
            ApplyCommand(new HaltCommand());
        }

        private void FillVoxelPoint(Point3D voxelPoint)
        {
            var bot = _scene.GetFirstBot();
            var botVector = Vector3D.FromPoint(bot.Current);
            var voxelVector = Vector3D.FromPoint(voxelPoint);
            _voxelsToFill -= 1;
            // Console.WriteLine($"{botVector}: {voxelPoint}");
            ApplyCommand(new FillCommand(voxelVector - botVector));
        }

        private void MoveBotToPoint(Point3D voxelPoint)
        {
            var bot = _scene.GetFirstBot();

            // var obstacles = _scene.GetObstacles();
            // var path = PathFinder.FindPathBetween(
            //     bot.Current,
            //     voxelPoint,
            //     _scene.PadBoundBox,
            //     obstacles);

            var path = PathFinder.FindPathBetweenWithOnlyStraightMoves(
                bot.Current,
                voxelPoint,
                _scene);

            foreach (var command in path)
            {
                ApplyCommand(command);
            }
        }

        private void ApplyCommand(BotCommand command)
        {
            _scene.ApplyCommand(command);
            _program.AddCommand(command);
        }

        private bool MissingVoxelsExist()
        {
            return _voxelsToFill > 0;
        }

        // Return point NEAR voxel to fill
        private (Point3D, Point3D) FindNextVoxelPoint()
        {
            var points = FindNotFilledVoxelsOnYLayer().ToList();
            if (points.Count == 0)
            {
                _currentYLevel += 1;
                points = FindNotFilledVoxelsOnYLayer().ToList();
            }
            // Console.WriteLine($"On Y-level '{_currentYLevel}' there are {points.Count} points to fill");

            return FindClosestPointNearNotFilledVoxels(points);
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

        private IEnumerable<Point3D> FindNotFilledVoxelsOnYLayer()
        {
            // Console.WriteLine(_currentYLevel);
            // Console.WriteLine(_model.Matrix.FullVoxels()
            // .Where(p => p.Y == _currentYLevel).Count());

            foreach (var point in _model.Matrix.FullVoxelsOnYLevel(_currentYLevel))
            {
                if (_scene.SceneState.Matrix[point] == VoxelStatus.Empty)
                {
                    yield return point;
                }
            }
        }
    }
}