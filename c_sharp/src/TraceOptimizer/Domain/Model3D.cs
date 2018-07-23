using System.Linq;
using TraceOptimizer.Geometry;
using TraceOptimizer.OctoTree;
using TraceOptimizer.Voxels;

namespace TraceOptimizer.Domain
{
    public class Model3D
    {
        public Model3D(Matrix matrix)
        {

            Matrix = matrix;
        }

        public Matrix Matrix { get; }

        public int Resolution => Matrix.Resolution;

        public int NumberOfFullVoxels()
        {
            return Matrix.FullVoxels().Count();
        }

        public OctoTree<VoxelStatus> ToOctoTree()
        {
            var tree = OctoTree.OctoTree<VoxelStatus>
                .Create(Cuboid.FromPoints(
                    Point3D.Origin(),
                    new Point3D(Resolution - 1, Resolution - 1, Resolution - 1)));

            // Console.WriteLine($"Resolution: {Resolution}");

            foreach (var fullVoxelPoint in Matrix.FullVoxels())
            {
                // Console.WriteLine($"Trying to add {fullVoxelPoint}");
                tree.AddNode(fullVoxelPoint, VoxelStatus.Empty);
            }

            return tree;
        }
    }
}