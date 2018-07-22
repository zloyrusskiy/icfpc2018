using System;
using System.Linq;
using TraceOptimizer.Geometry;
using TraceOptimizer.Optimization.Commands;
using TraceOptimizer.Optimization.Paths;
using Xunit;

namespace TraceOptimizer.Tests
{
    public class FindPathTests
    {
        [Fact]
        public void FindsStraightLineIfItExists()
        {
            var boundingBox = Cuboid.FromPoints(
                Point3D.Origin(),
                new Point3D(10, 10, 10));
            var source = new Point3D(5, 5, 5);
            var dest = new Point3D(5, 8, 5);

            var path = PathFinder.FindPathBetween(
                source,
                dest,
                boundingBox);
            var pathCommand = path[0] as StraightMoveCommand;

            Assert.Equal(1, path.Count);
            Assert.IsType<StraightMoveCommand>(path[0]);
            Assert.Equal<Vector3D>(new Vector3D(0, 3, 0), pathCommand.LongDiff);
        }

        [Fact]
        public void FindsTwoStraightLinesIfTheyExists()
        {
            var boundingBox = Cuboid.FromPoints(
                Point3D.Origin(),
                new Point3D(10, 10, 10));
            var source = new Point3D(5, 5, 5);
            var dest = new Point3D(5, 1, 6);

            var path = PathFinder.FindPathBetween(
                source,
                dest,
                boundingBox);
            var pathCommand1 = path[0] as StraightMoveCommand;
            var pathCommand2 = path[1] as StraightMoveCommand;

            Assert.Equal(2, path.Count);
            Assert.IsType<StraightMoveCommand>(path[0]);
            Assert.IsType<StraightMoveCommand>(path[1]);
            Assert.Equal<Vector3D>(new Vector3D(0, -4, 0), pathCommand1.LongDiff);
            Assert.Equal<Vector3D>(new Vector3D(0, 0, 1), pathCommand2.LongDiff);
        }

        [Fact]
        public void FindsThreeStraightLinesIfTheyExists()
        {
            var boundingBox = Cuboid.FromPoints(
                Point3D.Origin(),
                new Point3D(10, 10, 10));
            var source = new Point3D(5, 5, 5);
            var dest = new Point3D(8, 1, 6);

            var path = PathFinder.FindPathBetween(
                source,
                dest,
                boundingBox);
            var pathCommand1 = path[0] as StraightMoveCommand;
            var pathCommand2 = path[1] as StraightMoveCommand;
            var pathCommand3 = path[2] as StraightMoveCommand;

            Assert.Equal(3, path.Count);
            Assert.IsType<StraightMoveCommand>(path[0]);
            Assert.IsType<StraightMoveCommand>(path[1]);
            Assert.IsType<StraightMoveCommand>(path[2]);
            Assert.Equal<Vector3D>(new Vector3D(0, -4, 0), pathCommand1.LongDiff);
            Assert.Equal<Vector3D>(new Vector3D(3, 0, 0), pathCommand2.LongDiff);
            Assert.Equal<Vector3D>(new Vector3D(0, 0, 1), pathCommand3.LongDiff);
        }

        [Fact]
        public void FindsThreeStraightLinesIfThereIsObstacleInFrontOfMove()
        {
            var boundingBox = Cuboid.FromPoints(
                Point3D.Origin(),
                new Point3D(10, 10, 10));
            var source = new Point3D(5, 5, 5);
            var dest = new Point3D(5, 5, 7);
            var obstacle = Cuboid.FromPoints(
                new Point3D(5, 5, 6),
                new Point3D(5, 5, 6)
            );

            var path = PathFinder.FindPathBetween(
                source,
                dest,
                boundingBox,
                new[] { obstacle });
            var pathCommand1 = path[0] as StraightMoveCommand;
            var pathCommand2 = path[1] as StraightMoveCommand;
            var pathCommand3 = path[2] as StraightMoveCommand;

            Assert.Equal(3, path.Count);
            Assert.IsType<StraightMoveCommand>(path[0]);
            Assert.IsType<StraightMoveCommand>(path[1]);
            Assert.IsType<StraightMoveCommand>(path[2]);
            Assert.Equal<Vector3D>(new Vector3D(1, 0, 0), pathCommand1.LongDiff);
            Assert.Equal<Vector3D>(new Vector3D(0, 0, 2), pathCommand2.LongDiff);
            Assert.Equal<Vector3D>(new Vector3D(-1, 0, 0), pathCommand3.LongDiff);
        }

        [Fact]
        public void FindsThreeStraightLinesIfThereIsALineObstacleInFrontOfMove()
        {
            var boundingBox = Cuboid.FromPoints(
                Point3D.Origin(),
                new Point3D(10, 10, 10));
            var source = new Point3D(5, 5, 5);
            var dest = new Point3D(5, 5, 7);
            var obstacle = Cuboid.FromPoints(
                new Point3D(3, 5, 6),
                new Point3D(7, 5, 6)
            );

            var path = PathFinder.FindPathBetween(
                source,
                dest,
                boundingBox,
                new[] { obstacle });
            var pathCommand1 = path[0] as StraightMoveCommand;
            var pathCommand2 = path[1] as StraightMoveCommand;
            var pathCommand3 = path[2] as StraightMoveCommand;

            Assert.Equal(3, path.Count);
            Assert.IsType<StraightMoveCommand>(path[0]);
            Assert.IsType<StraightMoveCommand>(path[1]);
            Assert.IsType<StraightMoveCommand>(path[2]);
            Assert.Equal<Vector3D>(new Vector3D(0, 1, 0), pathCommand1.LongDiff);
            Assert.Equal<Vector3D>(new Vector3D(0, 0, 2), pathCommand2.LongDiff);
            Assert.Equal<Vector3D>(new Vector3D(0, -1, 0), pathCommand3.LongDiff);
        }

        public void FindsThreeStraightLinesIfThereIsALineOfPointObstaclesInFrontOfMove()
        {
            var boundingBox = Cuboid.FromPoints(
                Point3D.Origin(),
                new Point3D(10, 10, 10));
            var source = new Point3D(5, 5, 5);
            var dest = new Point3D(5, 5, 7);
            var obstacles = new[] {
                Cuboid.FromPoints(
                    new Point3D(3, 5, 6),
                    new Point3D(3, 5, 6)
                ),
                Cuboid.FromPoints(
                    new Point3D(4, 5, 6),
                    new Point3D(4, 5, 6)
                ),
                Cuboid.FromPoints(
                    new Point3D(5, 5, 6),
                    new Point3D(5, 5, 6)
                ),
                Cuboid.FromPoints(
                    new Point3D(6, 5, 6),
                    new Point3D(6, 5, 6)
                ),
                Cuboid.FromPoints(
                    new Point3D(7, 5, 6),
                    new Point3D(7, 5, 6)
                ),
            };

            var path = PathFinder.FindPathBetween(
                source,
                dest,
                boundingBox,
                obstacles);
            var pathCommand1 = path[0] as StraightMoveCommand;
            var pathCommand2 = path[1] as StraightMoveCommand;
            var pathCommand3 = path[2] as StraightMoveCommand;

            Assert.Equal(3, path.Count);
            Assert.IsType<StraightMoveCommand>(path[0]);
            Assert.IsType<StraightMoveCommand>(path[1]);
            Assert.IsType<StraightMoveCommand>(path[2]);
            Assert.Equal<Vector3D>(new Vector3D(0, 1, 0), pathCommand1.LongDiff);
            Assert.Equal<Vector3D>(new Vector3D(0, 0, 2), pathCommand2.LongDiff);
            Assert.Equal<Vector3D>(new Vector3D(0, -1, 0), pathCommand3.LongDiff);
        }
    }
}
