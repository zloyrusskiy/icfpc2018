using System;
using System.IO;
using System.Threading.Tasks;
using TraceOptimizer.Geometry;
using TraceOptimizer.Voxels;

namespace TraceOptimizer.Domain
{
    public class Model3DFromStdinProvider : IModel3DProvider
    {
        public Model3D GetModel()
        {
            var resolution = int.Parse(Console.ReadLine());
            var matrix = Matrix.Empty(resolution);

            int y = 0, z = resolution - 1;
            var line = Console.ReadLine();
            while (!string.IsNullOrEmpty(line))
            {
                if (z == -1)
                {
                    z = resolution - 1; y += 1;
                }

                for (var x = 0; x < resolution; x++)
                {
                    if (line[x] == '1')
                    {
                        var point = new Point3D(x, y, z);
                        matrix[point] = VoxelStatus.Full;
                    }
                }

                z -= 1;
                line = Console.ReadLine();
            }

            return new Model3D(matrix);
        }
    }
}