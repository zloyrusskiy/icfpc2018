namespace TraceOptimizer.Optimization.Commands
{
    public abstract class AbstractVisitor<T>
    {
        public virtual T Visit(FillCommand command)
        {
            return default(T);
        }

        public virtual T Visit(FlipCommand command)
        {
            return default(T);
        }

        public virtual T Visit(HaltCommand command)
        {
            return default(T);
        }

        public virtual T Visit(LMoveCommand command)
        {
            return default(T);
        }

        public virtual T Visit(StraightMoveCommand command)
        {
            return default(T);
        }

        public virtual T Visit(WaitCommand command)
        {
            return default(T);
        }

        public virtual T Visit(VoidCommand command)
        {
            return default(T);
        }
    }
}