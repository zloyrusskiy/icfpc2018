class Point
  attr_accessor :x, :y, :z

  def initialize(x, y, z)
    @x = x
    @y = y
    @z = z
  end

  def self.from_2d_point(point_2d, y_coord)
    self.new point_2d.x, y_coord, point_2d.z
  end

  def to_s
    "<#{@x},#{@y},#{@z}>"
  end

  def near_diff(point)
    PointDiff::Near.calc_diff(self, point)
  end

  def short_diff(point)
    PointDiff::Short.calc_diff(self, point)
  end

  def long_diff(point)
    PointDiff::Long.calc_diff(self, point)
  end
end