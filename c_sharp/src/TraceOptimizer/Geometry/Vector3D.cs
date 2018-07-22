using System;

namespace TraceOptimizer.Geometry
{
    public struct Vector3D : IEquatable<Vector3D>
    {
        public Vector3D(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public int X { get; }

        public int Y { get; }

        public int Z { get; }

        public Point3D ToPoint() =>
            new Point3D(X, Y, Z);

        public static Vector3D FromPoint(Point3D point) =>
            new Vector3D(point.X, point.Y, point.Z);

        public bool IsLinear =>
            (X != 0 && Y == 0 && Z == 0) ||
            (X == 0 && Y != 0 && Z == 0) ||
            (X == 0 && Y == 0 && Z != 0);

        public bool IsShortLinear =>
            IsLinear && ManhattanLength <= 5;

        public bool IsLongLinear =>
            IsLinear && ManhattanLength <= 15;

        public bool IsNear =>
            ManhattanLength > 0 &&
            ManhattanLength <= 2 &&
            ChebyshevLength == 1;

        public bool IsFar =>
            ChebyshevLength > 0 &&
            ChebyshevLength <= 30;

        public int ManhattanLength =>
            Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);

        public int ChebyshevLength =>
            Math.Max(Math.Abs(X), Math.Max(Math.Abs(Y), Math.Abs(Z)));

        public static Vector3D operator +(Vector3D f, Vector3D s) =>
            new Vector3D(f.X + s.X, f.Y + s.Y, f.Z + s.Z);

        public static Vector3D operator -(Vector3D f, Vector3D s) =>
            new Vector3D(f.X - s.X, f.Y - s.Y, f.Z - s.Z);

        public static Vector3D operator *(Vector3D v, int num) =>
            new Vector3D(v.X * num, v.Y * num, v.Z * num);

        public override int GetHashCode() =>
            65536 * X + 256 * Y + Z;

        public override bool Equals(object obj) =>
            Equals((Vector3D)obj);

        public bool Equals(Vector3D other) =>
            X == other.X && Y == other.Y && Z == other.Z;

        public override string ToString() =>
            $"Point({X}, {Y}, {Z})";
    }
}