require 'point_2d'
require 'command'
require 'point_diff'

module Utils
  def self.rect_bounds_detection(layer)
    min_point_2d = nil
    max_point_2d = nil
    layer.each.with_index do |row, z|
      row.each.with_index do |cell, x|
        next unless Voxel.full?(cell)
        min_point_2d = Point2D.new(row.length, row.length) if min_point_2d.nil?
        max_point_2d = Point2D.new(-1, -1) if max_point_2d.nil?
        min_point_2d.x = x if x <= min_point_2d.x
        min_point_2d.z = z if z <= min_point_2d.z
        max_point_2d.x = x if x >= max_point_2d.x
        max_point_2d.z = z if z >= max_point_2d.z
      end
    end
    [min_point_2d, max_point_2d]
  end

  def self.go_to_point_commands bot, point
    commands = []
    diff = PointDiff::Base.calc_diff(point, bot.position)
    long_smoves_dx_count = diff.dx.abs / PointDiff::Base::LONG_DISTANCE
    short_smove_dx = diff.dx.abs % PointDiff::Base::LONG_DISTANCE
    sign = diff.dx <=> 0
    if long_smoves_dx_count != 0
      commands.concat(
          (1..long_smoves_dx_count).map do |_|
            Command::SMove.new(
                PointDiff::Long.new(PointDiff::Base::LONG_DISTANCE * sign, 0, 0)
            )
          end
      )
    end
    if short_smove_dx != 0
      commands.push(
          Command::SMove.new(
              PointDiff::Long.new(short_smove_dx * sign, 0, 0)
          )
      )
    end
    long_smoves_dy_count = diff.dy.abs / PointDiff::Base::LONG_DISTANCE
    short_smove_dy = diff.dy.abs % PointDiff::Base::LONG_DISTANCE
    sign = diff.dy <=> 0
    if long_smoves_dy_count != 0
      commands.concat(
          (1..long_smoves_dy_count).map do |_|
            Command::SMove.new(
                PointDiff::Long.new(0, PointDiff::Base::LONG_DISTANCE * sign, 0)
            )
          end
      )
    end
    if short_smove_dy != 0
      commands.push(
          Command::SMove.new(
              PointDiff::Long.new(0, short_smove_dy * sign, 0)
          )
      )
    end
    long_smoves_dz_count = diff.dz.abs / PointDiff::Base::LONG_DISTANCE
    short_smove_dz = diff.dz.abs % PointDiff::Base::LONG_DISTANCE
    sign = diff.dz <=> 0
    if long_smoves_dz_count != 0
      commands.concat(
          (1..long_smoves_dz_count).map do |_|
            Command::SMove.new(
                PointDiff::Long.new(0, 0, PointDiff::Base::LONG_DISTANCE * sign)
            )
          end
      )
    end
    if short_smove_dz != 0
      commands.push(
          Command::SMove.new(
              PointDiff::Long.new(0, 0, short_smove_dz * sign)
          )
      )
    end
    commands
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