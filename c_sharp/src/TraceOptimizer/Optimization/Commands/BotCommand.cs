using System;
using TraceOptimizer.Domain;

namespace TraceOptimizer.Optimization.Commands
{
    public abstract class BotCommand
    {
        public abstract SceneState Apply(SceneState sceneState);

        public abstract void Visit(AbstractVisitor visitor);


        public abstract T Visit<T>(AbstractVisitor<T> visitor);
    }
}