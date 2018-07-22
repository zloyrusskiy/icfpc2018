using System;

namespace TraceOptimizer.Geometry
{
    public struct Point3D : IEquatable<Point3D>
    {

        public Point3D(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Point3D Origin() => new Point3D();

        public int X { get; }

        public int Y { get; }

        public int Z { get; }

        public bool IsAdjacent(Point3D point)
        {
            var vectorFrom = Vector3D.FromPoint(this);
            var vectorTo = Vector3D.FromPoint(point);

            return (vectorFrom - vectorTo).ManhattanLength == 1;
        }

        public static bool operator ==(Point3D f, Point3D s) =>
            f.Equals(s);

        public static bool operator !=(Point3D f, Point3D s) =>
            !f.Equals(s);

        public override bool Equals(object obj)
            => Equals((Point3D)obj);

        public override int GetHashCode() => 65536 * X + 256 * Y + Z;

        public bool Equals(Point3D other)
        {
            return X == other.X &&
                Y == other.Y &&
                Z == other.Z;
        }

        public override string ToString() =>
            $"Point({X}, {Y}, {Z})";
    }
}