using System;
using System.Collections.Generic;
using System.Linq;
using TraceOptimizer.Geometry;

namespace TraceOptimizer.OctoTree
{
    public class OctoTreeNode<T>
    {
        private bool _innerNode;

        private Cuboid _bounds;

        private List<OctoTreeLeaf<T>> _leafs;

        private OctoTreeNode<T>[] _children;

        private int _capacity;

        public OctoTreeNode(Cuboid bounds, int capacity)
        {
            _bounds = bounds;
            _innerNode = false;
            _capacity = capacity;
            _leafs = new List<OctoTreeLeaf<T>>();
        }

        public bool AddNode(Point3D point, T value)
        {
            if (!_bounds.Contains(point)) return false;

            if (!_innerNode && _leafs.Count + 1 > _capacity)
            {
                DivideSelf();
            }

            if (_innerNode)
            {
                var added = false;

                foreach (var child in _children)
                {
                    if (child.AddNode(point, value))
                    {
                        added = true;
                        break;
                    }
                }

                if (!added)
                    throw new InvalidOperationException($"Cannot add point {point} to tree in bound {_bounds}");
            }
            else
            {
                _leafs.Add(new OctoTreeLeaf<T>(point, value));
            }

            return true;
        }

        private bool DivideSelf()
        {
            var minPoint = _bounds.MinPoint;
            var maxPoint = _bounds.MaxPoint;

            var _size_x = (maxPoint.X - minPoint.X + 1) / 2;
            var _size_y = (maxPoint.Y - minPoint.Y + 1) / 2;
            var _size_z = (maxPoint.Z - minPoint.Z + 1) / 2;

            _children = new OctoTreeNode<T>[8];

            _children[0] = new OctoTreeNode<T>(
                Cuboid.FromPoints(
                    new Point3D(minPoint.X, minPoint.Y, minPoint.Z),
                    new Point3D(maxPoint.X - _size_x, maxPoint.Y - _size_y, maxPoint.Z - _size_z)
                ),
                _capacity);

            _children[1] = new OctoTreeNode<T>(
                Cuboid.FromPoints(
                    new Point3D(minPoint.X, minPoint.Y, minPoint.Z + _size_z),
                    new Point3D(maxPoint.X - _size_x, maxPoint.Y - _size_y, maxPoint.Z)
                ),
            _capacity);

            _children[2] = new OctoTreeNode<T>(
                Cuboid.FromPoints(
                    new Point3D(minPoint.X + _size_x, minPoint.Y, minPoint.Z + _size_z),
                    new Point3D(maxPoint.X, maxPoint.Y - _size_y, maxPoint.Z)
                ),
            _capacity);

            _children[3] = new OctoTreeNode<T>(
                Cuboid.FromPoints(
                    new Point3D(minPoint.X + _size_x, minPoint.Y, minPoint.Z),
                    new Point3D(maxPoint.X, maxPoint.Y - _size_y, maxPoint.Z - _size_z)
                ),
            _capacity);

            _children[4] = new OctoTreeNode<T>(
                Cuboid.FromPoints(
                    new Point3D(minPoint.X + _size_x, minPoint.Y + _size_y, minPoint.Z),
                    new Point3D(maxPoint.X, maxPoint.Y, maxPoint.Z - _size_z)
                ),
            _capacity);

            _children[5] = new OctoTreeNode<T>(
                Cuboid.FromPoints(
                    new Point3D(minPoint.X + _size_x, minPoint.Y + _size_y, minPoint.Z + _size_z),
                    new Point3D(maxPoint.X, maxPoint.Y, maxPoint.Z)
                ),
            _capacity);

            _children[6] = new OctoTreeNode<T>(
                Cuboid.FromPoints(
                    new Point3D(minPoint.X, minPoint.Y + _size_y, minPoint.Z + _size_z),
                    new Point3D(maxPoint.X - _size_x, maxPoint.Y, maxPoint.Z)
                ),
            _capacity);

            _children[7] = new OctoTreeNode<T>(
                Cuboid.FromPoints(
                    new Point3D(minPoint.X, minPoint.Y + _size_y, minPoint.Z),
                    new Point3D(maxPoint.X - _size_x, maxPoint.Y, maxPoint.Z - _size_z)
                ),
            _capacity);

            foreach (var leaf in _leafs)
            {
                foreach (var child in _children)
                {
                    if (child.AddNode(leaf.Point, leaf.Value))
                    {
                        break;
                    }
                }
            }

            _innerNode = true;
            _leafs = null;

            return true;
        }

        public int Count()
        {
            if (_innerNode)
            {
                return _children.Select(c => c.Count()).Sum();
            }
            else
            {
                return _leafs.Count;
            }
        }

        public IEnumerable<Cuboid> TraverseFullCuboids()
        {
            if (_innerNode)
            {
                if (Count() == _bounds.Size)
                {
                    yield return _bounds;
                }
                else
                {
                    foreach (var child in _children)
                    {
                        foreach (var cuboid in child.TraverseFullCuboids())
                        {
                            yield return cuboid;
                        }
                    }
                }
            }
            else
            {
                if (_leafs.Count > 0)
                {
                    foreach (var leaf in _leafs)
                    {
                        yield return Cuboid.FromPoints(
                            leaf.Point,
                            leaf.Point
                        );
                    }
                }
            }
        }
    }
}