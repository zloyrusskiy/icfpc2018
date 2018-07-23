using TraceOptimizer.Geometry;

namespace TraceOptimizer.OctoTree
{
    public class OctoTreeLeaf<T>
    {
        public OctoTreeLeaf(Point3D point, T value)
        {
            Point = point;
            Value = value;
        }

        public Point3D Point { get; }

        public T Value { get; }
    }
}