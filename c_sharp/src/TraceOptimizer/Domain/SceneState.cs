using System.Collections.Generic;
using TraceOptimizer.Voxels;

namespace TraceOptimizer.Domain
{
    public class SceneState
    {
        private sealed class NanoBotComparer : IComparer<NanoBot>
        {
            public int Compare(NanoBot x, NanoBot y)
            {
                return x.Bid - y.Bid;
            }
        }

        public int Energy { get; set; }

        public HarmonicsMode HarmonicsMode { get; set; }

        public Matrix Matrix { get; set; }

        public ICollection<NanoBot> Bots { get; set; }

        public SceneState ChangeEnergy(int amount)
        {
            Energy += amount;
            return this;
        }

        public SceneState FlipHarmonicsMode()
        {
            HarmonicsMode = HarmonicsMode == HarmonicsMode.Grounded ?
                HarmonicsMode.Floating : HarmonicsMode.Grounded;

            return this;
        }

        public SceneState ClearNanoBots()
        {
            Bots = new HashSet<NanoBot>();
            return this;
        }

        public static SceneState New(int resolution)
        {
            var sceneState = new SceneState
            {
                Energy = 0,
                HarmonicsMode = HarmonicsMode.Grounded,
                Matrix = Matrix.Empty(resolution),
            };

            sceneState.Bots = new SortedSet<NanoBot>(
                new[] { NanoBot.Initial() },
                new NanoBotComparer());

            return sceneState;
        }

        public static SceneState FromMatrix(Matrix matrix)
        {
            var sceneState = new SceneState
            {
                Energy = 0,
                HarmonicsMode = HarmonicsMode.Grounded,
                Matrix = matrix,
            };

            sceneState.Bots = new SortedSet<NanoBot>(
                new[] { NanoBot.Initial() },
                new NanoBotComparer());

            return sceneState;
        }
    }
}