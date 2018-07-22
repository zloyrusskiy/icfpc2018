using System.Collections.Generic;
using System.Linq;
using TraceOptimizer.Geometry;

namespace TraceOptimizer.Domain
{
    public class NanoBot
    {
        private const int MAX_BID = 20;

        private NanoBot()
        {
        }

        public int Bid { get; private set; }

        public Point3D Current { get; private set; }

        public IEnumerable<int> Seeds { get; private set; }

        public NanoBot MoveTo(Point3D newPoint)
        {
            Current = newPoint;
            return this;
        }

        public static NanoBot Initial()
        {
            return new NanoBot
            {
                Bid = 1,
                Current = Point3D.Origin(),
                Seeds = new SortedSet<int>(Enumerable.Range(2, MAX_BID + 1)),
            };
        }
    }
}