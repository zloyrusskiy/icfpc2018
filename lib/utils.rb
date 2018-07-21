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

	def self.draw_layer bot, matrix, left_bottom_2d_point, right_top_2d_point
		commands = []

		need_cells_qty = (right_top_2d_point.x - left_bottom_2d_point.x + 1) * (right_top_2d_point.z - left_bottom_2d_point.z + 1)
		direction_delta = 1
		1.upto(need_cells_qty) do
			if matrix[bot.position.z][bot.position.x] == 1
				commands << Command::Fill.new(PointDiff::Near.new(0, -1, 0))
			end

			if (
				(direction_delta == 1 and bot.position.z < right_top_2d_point.z) or
				(direction_delta == -1 and bot.position.z > left_bottom_2d_point.z)
				)
				commands << Command::SMove.new(PointDiff::Long.new(0, 0, direction_delta))
				bot.position.z += direction_delta
			else
				commands << Command::SMove.new(PointDiff::Long.new(1, 0, 0))
				bot.position.x += 1
				direction_delta = -direction_delta
			end
		end

		commands.pop

		commands
	end
end