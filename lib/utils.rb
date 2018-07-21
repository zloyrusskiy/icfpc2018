require 'point_2d'
require 'command'
require 'point_diff'

module Utils
	def self.rect_bounds_detection matrix
		[Point2D.new(0,0), Point2D.new(0,0)]
	end

	def self.get_go_to_point_commands bot, point
		[]
	end

	def self.draw_layer bot, matrix, left_top_2d_point, right_bottom_2d_point
		commands = []

		direction_delta = 1
		while bot.x <= right_bottom_2d_point.x
			while (
				(direction_delta == 1 and bot.z <= right_bottom_2d_point.z) or
				(direction_delta == -1 and bot.z >= left_top_2d_point.z)
			)
				if matrix[bot.z][bot.x] == '1'
					commands << Command::Fill.new(PointDiff::Near.new(0, -1, 0))
				end

				commands << Command::SMove.new(PointDiff::Long.new(0, 0, direction_delta))
				bot.z += direction_delta
			end

			commands.pop
			bot.z -= direction_delta
			commands << Command::SMove.new(PointDiff::Long.new(1, 0, 0))
			bot.x += 1
			direction_delta = -direction_delta
		end
		commands.pop
		bot.x -= 1

		commands
	end
end