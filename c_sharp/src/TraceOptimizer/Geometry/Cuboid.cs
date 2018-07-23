using System;
using System.Collections.Generic;

namespace TraceOptimizer.Geometry
{
    public struct Cuboid : IEquatable<Cuboid>
    {
        private Cuboid(Point3D c1, Point3D c2)
        {
            MinPoint = c1;
            MaxPoint = c2;
        }

        public Point3D MinPoint { get; }

        public Point3D MaxPoint { get; }

        public int Size =>
            (MaxPoint.X - MinPoint.X + 1) *
            (MaxPoint.Y - MinPoint.Y + 1) *
            (MaxPoint.Z - MinPoint.Z + 1);

        public Dimension Dimension
        {
            get
            {
                var dimValue = (MinPoint.X == MaxPoint.X ? 0 : 1) +
                    (MinPoint.Y == MaxPoint.Y ? 0 : 1) +
                    (MinPoint.Z == MaxPoint.Z ? 0 : 1);

                switch (dimValue)
                {
                    case 0:
                        return Dimension.Point;
                    case 1:
                        return Dimension.Line;
                    case 2:
                        return Dimension.Plane;
                    case 3:
                        return Dimension.Box;
                    default:
                        throw new InvalidOperationException(
                            $"Unsupported dimension value: '{dimValue}'.");
                }
            }
        }

        public IEnumerable<Point3D> Points()
        {
            for (var x = MinPoint.X; x <= MaxPoint.X; x++)
                for (var y = MinPoint.Y; y <= MaxPoint.Y; y++)
                    for (var z = MinPoint.Z; z <= MaxPoint.Z; z++)
                    {
                        yield return new Point3D(x, y, z);
                    }
        }

        public IEnumerable<Point3D> PointsFromBottomToTopSnake()
        {
            var vector = Vector3D.FromPoint(MinPoint);

            for (var y = MinPoint.Y; y <= MaxPoint.Y; y++)
                for (var x = MinPoint.X; x <= MaxPoint.X; x++)
                    for (var z = MinPoint.Z; z <= MaxPoint.Z; z++)
                    {
                        yield return new Point3D(x, y, z);
                    }
        }

        public static Cuboid FromPoints(Point3D p1, Point3D p2)
        {
            var minPoint = new Point3D(
                Math.Min(p1.X, p2.X),
                Math.Min(p1.Y, p2.Y),
                Math.Min(p1.Z, p2.Z));

            var maxPoint = new Point3D(
                Math.Max(p1.X, p2.X),
                Math.Max(p1.Y, p2.Y),
                Math.Max(p1.Z, p2.Z));

            return new Cuboid(minPoint, maxPoint);
        }

        public bool Contains(Point3D point) =>
            (MinPoint.X <= point.X && point.X <= MaxPoint.X) &&
            (MinPoint.Y <= point.Y && point.Y <= MaxPoint.Y) &&
            (MinPoint.Z <= point.Z && point.Z <= MaxPoint.Z);

        public override int GetHashCode() =>
            MinPoint.GetHashCode() ^ MaxPoint.GetHashCode();

        public override bool Equals(object obj) =>
            Equals((Cuboid)obj);

        public bool Equals(Cuboid other) =>
            MinPoint == other.MinPoint &&
            MaxPoint == other.MaxPoint;
    }
}