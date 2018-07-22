using System;
using System.Collections.Generic;
using TraceOptimizer.Geometry;
using TraceOptimizer.Utils;

namespace TraceOptimizer.Voxels
{
    public class Matrix
    {
        private readonly BitSet _bitSet;
        private readonly int _resolution;

        private readonly int _resolutionSquared;

        private Matrix(int resolution)
        {
            if (resolution <= 0)
                throw new ArgumentOutOfRangeException(
                    $"Unsupported resolution: '{resolution}'");

            _resolution = resolution;
            _resolutionSquared = resolution * resolution;
            _bitSet = new BitSet(resolution);
        }

        public VoxelStatus this[Point3D point]
        {
            get
            {
                var bitPosition = GetBitPosition(point);

                return _bitSet.Get(bitPosition) ?
                    VoxelStatus.Full : VoxelStatus.Empty;
            }
            set
            {
                var bitPosition = GetBitPosition(point);

                _bitSet.Set(bitPosition, value == VoxelStatus.Empty ? false : true);
            }
        }

        public int Resolution => _resolution;

        public IEnumerable<Point3D> FullVoxels()
        {
            for (var x = 0; x < Resolution; x++)
                for (var y = 0; y < Resolution; y++)
                    for (var z = 0; z < Resolution; z++)
                    {
                        var point = new Point3D(x, y, z);
                        if (this[point] == VoxelStatus.Full)
                        {
                            yield return point;
                        }
                    }
        }

        public IEnumerable<Point3D> FullVoxelsOnYLevel(int y)
        {
            for (var x = 0; x < Resolution; x++)
                for (var z = 0; z < Resolution; z++)
                {
                    var point = new Point3D(x, y, z);
                    if (this[point] == VoxelStatus.Full)
                    {
                        yield return point;
                    }
                }
        }

        private int GetBitPosition(Point3D point) =>
            point.X * _resolutionSquared + point.Y * _resolution + point.Z;

        public static Matrix Empty(int resolution) =>
            new Matrix(resolution);
    }
}