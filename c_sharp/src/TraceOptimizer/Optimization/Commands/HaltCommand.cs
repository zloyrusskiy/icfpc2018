using TraceOptimizer.Domain;

namespace TraceOptimizer.Optimization.Commands
{
    public sealed class HaltCommand : BotCommand
    {
        public override SceneState Apply(SceneState sceneState)
        {
            return sceneState.ClearNanoBots();
        }

        public override void Visit(AbstractVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override T Visit<T>(AbstractVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}