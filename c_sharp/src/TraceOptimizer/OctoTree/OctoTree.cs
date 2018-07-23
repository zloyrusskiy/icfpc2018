using System.Collections.Generic;
using TraceOptimizer.Geometry;

namespace TraceOptimizer.OctoTree
{
    public class OctoTree<T>
    {
        private OctoTreeNode<T> _root;

        private OctoTree(Cuboid bound)
        {
            _root = new OctoTreeNode<T>(bound, 1);
        }

        public static OctoTree<T> Create(Cuboid bound)
        {
            return new OctoTree<T>(bound);
        }

        public static OctoTree<T> Create(int resolution)
        {
            return Create(Cuboid.FromPoints(
                Point3D.Origin(),
                new Point3D(resolution - 1, resolution - 1, resolution - 1)
            ));
        }

        public bool AddNode(Point3D point, T value)
        {
            return _root.AddNode(point, value);
        }

        public IEnumerable<Cuboid> TraverseFullCuboids()
        {
            return _root.TraverseFullCuboids();
        }
    }
}