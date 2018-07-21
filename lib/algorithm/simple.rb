require 'utils'
require 'command'
require 'point'

module Algorithm
	class Simple
		def initialize
			@commands = []
		end

		def solve model
			bot = Bot.new

			@commands += prepare_state()

			model.points.each do |layer|
				left_top_point, right_bottom_point = Utils.rect_bounds_detection(layer)
				move_commands = Utils.get_go_to_point_commands bot, left_top_point
				@commands += move_commands
				@commands += Utils.draw_layer(bot, layer, left_top_point, right_bottom_point)
			end

			@commands += move_to_origin(bot)

			@commands
		end

		private

		def prepare_state
			[
				Command::Flip.new
			]
		end

		def move_to_origin bot
			move_commands = Utils.get_go_to_point_commands bot, Point.new(0,0,0)
			@commands += move_commands

			@commands << Command::Flip.new
			@commands << Command::Halt.new
		end
	end
end