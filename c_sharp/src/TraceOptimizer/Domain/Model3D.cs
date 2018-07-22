using System.Linq;
using TraceOptimizer.Geometry;
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
    }
}