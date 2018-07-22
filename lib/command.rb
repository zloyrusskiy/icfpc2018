require "command/base"
require "command/fill"
require "command/fission"
require "command/flip"
require "command/fusionp"
require "command/fusions"
require "command/halt"
require "command/lmove"
require "command/smove"
require "command/wait"
require "command/void"

module Command
  def self.print_commands(commands)
    puts commands.join("\n")
  end
end
