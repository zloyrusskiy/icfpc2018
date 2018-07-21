class State
	def initialize model
		@commands = []
		@model = model
	end

	def << commands_or_command
		if commands_or_command is_a? Command::Base
			add_commands([commands_or_command])
		elsif commands_or_command instance_of? Array
			add_commands commands_or_command
		else
			raise 'Unknown type for adding commands'
		end
	end

	def commands
		@commands.dup
	end

	private

	def add_commands commands
		if commands.all? { |c| c.valid?(self) }
			@commands.concat(commands)
		else
			raise 'Commands is invalid'
		end
	end
end