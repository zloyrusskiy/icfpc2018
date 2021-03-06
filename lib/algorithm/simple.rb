require 'utils'
require 'command'
require 'point'
require 'bot'

module Algorithm
	class Simple
		def initialize
			@commands = []
		end

		def solve model
			bot = Bot.new(Point.new(0,0,0))

			@commands += prepare_state()

			model.points.each.with_index do |layer, y_coord|
				left_bottom_2d_point, right_top_2d_point = Utils.rect_bounds_detection(layer)

				left_bottom_3d_point = Point.from_2d_point(left_bottom_2d_point, y_coord)

				move_commands = Utils.go_to_point_commands bot, left_bottom_3d_point

				@commands += move_commands
				@commands += Utils.draw_layer(bot, layer, left_bottom_2d_point, right_top_2d_point)
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
			move_commands = Utils.go_to_point_commands bot, Point.new(0, 0, 0)
			@commands += move_commands

			@commands << Command::Flip.new
			@commands << Command::Halt.new
		end
	end
end