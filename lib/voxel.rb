require 'point'

class Voxel
  attr_reader :x, :y, :z, :full

  def initialize(x, y, z, is_full)
    @x = x
    @y = y
    @z = z
    @full = is_full
  end

  def to_s
    "#{point} #{@full}"
  end

  def self.full?(cell)
    cell.to_s == '1'
  end

  def point
    Point.new(@x, @y, @z)
  end
end