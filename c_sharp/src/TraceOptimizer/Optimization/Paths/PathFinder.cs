using TraceOptimizer.Domain;
using TraceOptimizer.Geometry;
using C5;
using System.Collections.Generic;
using System.Linq;
using TraceOptimizer.Optimization.Commands;
using System;

namespace TraceOptimizer.Optimization.Paths
{
    public static class PathFinder
    {
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

        private struct PathTempInfo
        {
            public Vector3D Parent { get; set; }

            public MoveInfo Move { get; set; }
        }

        public struct MoveInfo
        {
            public MoveType MoveType { get; set; }

            public Vector3D First { get; set; }

            public Vector3D Second { get; set; }
        }

        private static int DistanceCost(Vector3D from, Vector3D to)
        {
            return (from - to).ManhattanLength;
        }

        public static List<BotCommand> FindPathBetweenWithOnlyStraightMoves(
            Point3D source,
            Point3D destination,
            Scene scene)
        {
            var boundingBox = scene.PadBoundBox;
            var destinationVector = Vector3D.FromPoint(destination);
            var sourceVector = Vector3D.FromPoint(source);
            var priorityQueue = new IntervalHeap<Vector3D>(
                new PriorityQueueComparer(destinationVector));
            var visited = new System.Collections.Generic.HashSet<Vector3D>();
            var g_of_x = new Dictionary<Vector3D, int>();
            var parent = new Dictionary<Vector3D, PathTempInfo>();
            // var f_of_x = new Dictionary<Vector3D, int>();

            priorityQueue.Add(sourceVector);
            g_of_x[sourceVector] = 0;
            parent[sourceVector] = new PathTempInfo();
            // f_of_x[sourceVector] = DistanceCost(sourceVector, destinationVector);

            while (priorityQueue.Count != 0)
            {
                var nextPoint = priorityQueue.DeleteMin();

                if (nextPoint.ToPoint() == destination)
                {
                    // We have found the path!
                    return GeneratePathFrom(parent, sourceVector, destinationVector);
                }

                var obstacles = scene.GetObstaclesAgainstStraightMove(nextPoint.ToPoint());

                visited.Add(nextPoint);

                // Console.WriteLine($"Enumerating paths from {nextPoint}");
                foreach (var reachableStruct in EnumerateMoves(nextPoint))
                {
                    // Console.WriteLine($"Checking to {reachableStruct.First} with type {reachableStruct.MoveType}");
                    var reachable = reachableStruct.First + nextPoint;
                    var reachablePoint = reachable.ToPoint();
                    var pathObstacle = Cuboid.FromPoints(nextPoint.ToPoint(), reachablePoint);

                    if (!boundingBox.Contains(reachablePoint)) continue;

                    if (StraightPathBoundBoxIntersectsObstacles(pathObstacle, obstacles)) continue;

                    var tentativeScore = g_of_x[nextPoint] +
                        DistanceCost(nextPoint, reachable);

                    if (visited.Contains(reachable) &&
                        tentativeScore >= g_of_x.GetValueOrDefault(reachable, 0))
                    {
                        continue;
                    }

                    if (!visited.Contains(reachable) ||
                        tentativeScore < g_of_x.GetValueOrDefault(reachable, 0))
                    {

                        parent[reachable] = new PathTempInfo
                        {
                            Parent = nextPoint,
                            Move = reachableStruct
                        };
                        g_of_x[reachable] = tentativeScore;
                        //f_of_x[reachable] = tentativeScore + DistanceCost(reachable, destinationVector);
                        if (!priorityQueue.Contains(reachable))
                        {
                            priorityQueue.Add(reachable);
                        }
                    }
                }
            }

            return null;
        }

        private static bool StraightPathBoundBoxIntersectsObstacles(Cuboid pathObstacle, Cuboid[] obstacles)
        {
            foreach (var obstacle in obstacles)
            {
                if (pathObstacle.Contains(obstacle.MinPoint)) return true;
            }
            return false;
        }

        public static List<BotCommand> FindPathBetween(
            Point3D source,
            Point3D destination,
            Cuboid boundingBox,
            Cuboid[] obstacles = null)
        {
            obstacles = obstacles ?? new Cuboid[0];
            var destinationVector = Vector3D.FromPoint(destination);
            var sourceVector = Vector3D.FromPoint(source);
            var priorityQueue = new IntervalHeap<Vector3D>(
                new PriorityQueueComparer(destinationVector));
            var visited = new System.Collections.Generic.HashSet<Vector3D>();
            var g_of_x = new Dictionary<Vector3D, int>();
            var parent = new Dictionary<Vector3D, PathTempInfo>();
            // var f_of_x = new Dictionary<Vector3D, int>();

            priorityQueue.Add(sourceVector);
            g_of_x[sourceVector] = 0;
            parent[sourceVector] = new PathTempInfo();
            // f_of_x[sourceVector] = DistanceCost(sourceVector, destinationVector);

            while (priorityQueue.Count != 0)
            {
                var nextPoint = priorityQueue.DeleteMin();

                if (nextPoint.ToPoint() == destination)
                {
                    // We have found the path!
                    return GeneratePathFrom(parent, sourceVector, destinationVector);
                }

                visited.Add(nextPoint);

                // Console.WriteLine($"Enumerating paths from {nextPoint}");
                foreach (var reachableStruct in EnumerateMoves(nextPoint))
                {
                    // Console.WriteLine($"Checking to {reachableStruct.First} with type {reachableStruct.MoveType}");
                    var reachable = reachableStruct.First + nextPoint;
                    var reachablePoint = reachable.ToPoint();
                    var pathObstacle = Cuboid.FromPoints(nextPoint.ToPoint(), reachablePoint);

                    if (!boundingBox.Contains(reachablePoint)) continue;

                    if (PathBoundBoxIntersectsObstacles(pathObstacle, obstacles)) continue;

                    var tentativeScore = g_of_x[nextPoint] +
                        DistanceCost(nextPoint, reachable);

                    if (visited.Contains(reachable) &&
                        tentativeScore >= g_of_x.GetValueOrDefault(reachable, 0))
                    {
                        continue;
                    }

                    if (!visited.Contains(reachable) ||
                        tentativeScore < g_of_x.GetValueOrDefault(reachable, 0))
                    {

                        parent[reachable] = new PathTempInfo
                        {
                            Parent = nextPoint,
                            Move = reachableStruct
                        };
                        g_of_x[reachable] = tentativeScore;
                        //f_of_x[reachable] = tentativeScore + DistanceCost(reachable, destinationVector);
                        if (!priorityQueue.Contains(reachable))
                        {
                            priorityQueue.Add(reachable);
                        }
                    }
                }
            }

            return null;
        }

        private static bool PathBoundBoxIntersectsObstacles(
            Cuboid pathObstacle,
            Cuboid[] obstacles)
        {
            foreach (var point in pathObstacle.Points())
            {
                if (obstacles.Any(o => o.Contains(point))) return true;
            }
            return false;
        }

        private static List<BotCommand> GeneratePathFrom(
            Dictionary<Vector3D, PathTempInfo> parent,
            Vector3D source, Vector3D destination)
        {
            var current = parent[destination];
            var commands = new List<BotCommand>();

            // Console.WriteLine("Building path...");
            while (current.Move.MoveType != MoveType.Loop)
            {
                switch (current.Move.MoveType)
                {
                    case MoveType.Straight:
                        commands.Add(new StraightMoveCommand(current.Move.First));
                        break;
                        // case MoveType.LMove:
                        //     commands.Add(new LMoveCommand(current.Move.Second, current.Move.First));
                        //     break;
                }
                current = parent[current.Parent];
            }

            commands.Reverse();
            return commands;
        }

        private static readonly Vector3D[] offsets = new[] {
            new Vector3D(0, 0, 1),
            new Vector3D(0, 0, -1),
            new Vector3D(0, 1, 0),
            new Vector3D(0, -1, 0),
            new Vector3D(1, 0, 0),
            new Vector3D(-1, 0, 0),
        };

        public static IEnumerable<MoveInfo> EnumerateMoves(Vector3D source)
        {
            foreach (var point in EnumerateStraightMoves(source))
            {
                yield return point;
            }

            // foreach (var point in EnumerateLMoves(source))
            // {
            //     yield return point;
            // }
        }
        public static IEnumerable<MoveInfo> EnumerateStraightMoves(Vector3D source)
        {
            return EnumerateMovesLowerOrEqual(source, 15);
        }

        // Works incorrectly!!!
        public static IEnumerable<MoveInfo> EnumerateLMoves(Vector3D source)
        {
            foreach (var first in EnumerateMovesLowerOrEqual(source, 5))
            {
                var pos = source + first.First;
                foreach (var second in EnumerateMovesLowerOrEqual(pos, 5))
                {
                    var nextPos = pos + second.First;

                    yield return new MoveInfo
                    {
                        MoveType = MoveType.LMove,
                        First = nextPos,
                        Second = pos,
                    };
                }
            }
        }

        public static IEnumerable<MoveInfo> EnumerateMovesLowerOrEqual(Vector3D source, int maxMult)
        {
            foreach (var vector in offsets)
            {
                foreach (var i in Enumerable.Range(1, maxMult))
                {
                    // var dist = source + vector * i;
                    var dist = vector * i;

                    yield return new MoveInfo
                    {
                        MoveType = MoveType.Straight,
                        First = dist
                    };
                }
            }
        }
    }
}