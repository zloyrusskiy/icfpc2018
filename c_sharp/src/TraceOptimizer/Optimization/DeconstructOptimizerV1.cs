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
    public class DeconstructOptimizerV1 : IDeconstructOptimizer
    {
        private Scene _scene;

        private BotProgram _program;

        private Model3D _model;

        private int _voxelsToVoid;

        private int _currentYLevel;

        public BotProgram Optimize(Model3D modelToBuild)
        {
            _scene = Scene.FromModel(modelToBuild);
            _program = new BotProgram();
            _voxelsToVoid = modelToBuild.NumberOfFullVoxels();
            _currentYLevel = modelToBuild.Resolution - 1;
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

                VoidVoxelPoint(voxelPoint);
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

        private void VoidVoxelPoint(Point3D voxelPoint)
        {
            var bot = _scene.GetFirstBot();
            var botVector = Vector3D.FromPoint(bot.Current);
            var voxelVector = Vector3D.FromPoint(voxelPoint);
            _voxelsToVoid -= 1;
            // Console.WriteLine($"{botVector}: {voxelPoint}");
            ApplyCommand(new VoidCommand(voxelVector - botVector));
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
            return _voxelsToVoid > 0;
        }

        // Return point NEAR voxel to fill
        private (Point3D, Point3D) FindNextVoxelPoint()
        {
            var points = FindNotVoidedVoxelsOnYLayer().ToList();
            while (points.Count == 0)
            {
                _currentYLevel -= 1;
                points = FindNotVoidedVoxelsOnYLayer().ToList();
            }
            // Console.WriteLine($"On Y-level '{_currentYLevel}' there are {points.Count} points to fill");

            return FindClosestPointNearNotVoidedVoxels(points);
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
        private (Point3D, Point3D) FindClosestPointNearNotVoidedVoxels(List<Point3D> points)
        {
            var bot = _scene.GetFirstBot();
            var pointsSet = new C5.IntervalHeap<Vector3D>(
                new PriorityQueueComparer(Vector3D.FromPoint(bot.Current))
            );
            var map = new Dictionary<Point3D, Point3D>();
            var boundBox = _scene.PadBoundBox;

            foreach (var pointToFill in points)
            {
                foreach (var candidatePoint in EnumeratePointsToVoidFrom(pointToFill))
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
        private IEnumerable<Point3D> EnumeratePointsToVoidFrom(Point3D point)
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

        private IEnumerable<Point3D> FindNotVoidedVoxelsOnYLayer()
        {
            // Console.WriteLine(_currentYLevel);
            // Console.WriteLine(_model.Matrix.FullVoxels()
            // .Where(p => p.Y == _currentYLevel).Count());

            return _scene.SceneState.Matrix.FullVoxelsOnYLevel(_currentYLevel);
        }
    }
}