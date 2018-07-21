require 'point_2d'

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

  def self.get_go_to_point_commands bot, point
    []
  end

  def self.draw_layer bot, matrix, left_top_point, right_bottom_point
    []
  end
end