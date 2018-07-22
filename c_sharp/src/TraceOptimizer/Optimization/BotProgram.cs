using System.Collections.Generic;
using System.Linq;
using TraceOptimizer.Optimization.Commands;

namespace TraceOptimizer.Optimization
{
    public class BotProgram
    {
        private readonly List<BotCommand> _commands;

        public BotProgram(IEnumerable<BotCommand> commands)
        {
            _commands = commands.ToList();
        }

        public BotProgram()
            : this(new List<BotCommand>())
        {
        }

        public void AddCommand(BotCommand command)
        {
            _commands.Add(command);
        }

        public IEnumerable<BotCommand> Commands => _commands;
    }
}