namespace TraceOptimizer.Optimization.Commands
{
    public abstract class AbstractVisitor
    {
        public abstract void Visit(FillCommand command);

        public abstract void Visit(FlipCommand command);

        public abstract void Visit(HaltCommand command);

        public abstract void Visit(LMoveCommand command);

        public abstract void Visit(StraightMoveCommand command);

        public abstract void Visit(WaitCommand command);

        public abstract void Visit(VoidCommand command);
    }
}