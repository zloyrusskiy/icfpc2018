using System;
using System.Collections.Generic;
using System.Linq;
using TraceOptimizer.Domain;
using TraceOptimizer.Geometry;
using TraceOptimizer.OctoTree;
using TraceOptimizer.Optimization.Commands;
using TraceOptimizer.Optimization.Paths;
using TraceOptimizer.Utils;
using TraceOptimizer.Voxels;

namespace TraceOptimizer.Optimization
{
    public class ConstructOptimizerV2 : IConstructOptimizer
    {
        private Model3D _modelToBuild;

        private Scene _scene;

        private BotProgram _program;

        private OctoTree<VoxelStatus> _treeToConstruct;

        private int _resolution;
        private int _resolutionSquared;
        private int _voxelsFilled;

        private DSU _dsu;

        public BotProgram Optimize(Model3D modelToBuild)
        {
            _modelToBuild = modelToBuild;
            _treeToConstruct = _modelToBuild.ToOctoTree();
            _scene = Scene.New(_modelToBuild.Resolution);
            _program = new BotProgram();
            _resolution = _modelToBuild.Resolution;
            _resolutionSquared = _resolution * _resolution;
            _dsu = new DSU(_resolutionSquared * _resolution + 1);
            _voxelsFilled = 0;

            // Console.WriteLine("Starting to optimize...");

            OptimizeInternal();

            return _program;
        }

        private void OptimizeInternal()
        {

            foreach (var cuboid in _treeToConstruct.TraverseFullCuboids()
                .OrderBy(c => c.MinPoint.Y)
                .ThenBy(c => c.MinPoint.X)
                .ThenBy(c => c.MinPoint.Z))
            {
                // Console.WriteLine($"Here with {cuboid}");
                FillCuboid(cuboid);
            }

            MoveBotToPoint(Point3D.Origin());
            HaltProgram();
        }

        private void FillCuboid(Cuboid cube)
        {
            foreach (var point in cube.PointsFromBottomToTopSnake())
            {
                var vector = Vector3D.FromPoint(point);
                var vectorAbove = vector + Vector3D.FromPoint(new Point3D(0, 1, 0));

                MoveBotToPoint(vectorAbove.ToPoint());
                FillVoxelPoint(point);
            }
        }

        private void FillVoxelPoint(Point3D voxelPoint)
        {
            var bot = _scene.GetFirstBot();
            var botVector = Vector3D.FromPoint(bot.Current);
            var voxelVector = Vector3D.FromPoint(voxelPoint);
            var dsuIndex = GetPointDSUIndex(voxelPoint);

            if (voxelPoint.Y == 0)
            {
                // foreach (var point in FilledAdjacent(voxelPoint))
                // {
                //     var p = GetPointDSUIndex(point);
                //     _dsu.Union(p, dsuIndex);
                // }
                _dsu.Union(0, dsuIndex);
            }
            else
            {
                var adjacentFilled = FilledAdjacent(voxelPoint).ToList();
                if (adjacentFilled.Count > 0)
                {
                    foreach (var point in adjacentFilled)
                    {
                        var p = GetPointDSUIndex(point);
                        _dsu.Union(p, dsuIndex);
                    }
                }
                else
                {
                    if (_scene.SceneState.HarmonicsMode == HarmonicsMode.Grounded)
                    {
                        ApplyCommand(new FlipCommand());
                    }
                }
            }

            _voxelsFilled += 1;
            ApplyCommand(new FillCommand(voxelVector - botVector));

            if (_voxelsFilled == _dsu.SizeOf(0) - 2)
            {
                if (_scene.SceneState.HarmonicsMode == HarmonicsMode.Floating)
                {
                    ApplyCommand(new FlipCommand());
                }
            }
            else
            {
                if (_scene.SceneState.HarmonicsMode == HarmonicsMode.Grounded)
                {
                    ApplyCommand(new FlipCommand());
                }
            }
        }

        private static IEnumerable<Point3D> AdjacentTo(Point3D point)
        {
            yield return new Point3D(point.X + 1, point.Y, point.Z);
            yield return new Point3D(point.X - 1, point.Y, point.Z);
            yield return new Point3D(point.X, point.Y + 1, point.Z);
            yield return new Point3D(point.X, point.Y - 1, point.Z);
            yield return new Point3D(point.X, point.Y, point.Z + 1);
            yield return new Point3D(point.X, point.Y, point.Z - 1);
        }

        private IEnumerable<Point3D> FilledAdjacent(Point3D point)
        {
            foreach (var p in AdjacentTo(point))
            {
                if (_scene.SceneState.Matrix[p] == VoxelStatus.Full)
                {
                    yield return p;
                }
            }
        }

        private int GetPointDSUIndex(Point3D point)
        {
            return _resolutionSquared * point.X + _resolution * point.Y + point.Z + 1;
        }

        private void MoveBotToPoint(Point3D voxelPoint)
        {
            var bot = _scene.GetFirstBot();

            // Console.WriteLine($"We need to move from {bot.Current} to {voxelPoint}");

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
    }
}