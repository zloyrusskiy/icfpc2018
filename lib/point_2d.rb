class Point2D
  attr_accessor :x, :z

  def initialize x, z
    @x = x
    @z = z
  end

  def to_s
    "<#{@x},#{@z}>"
  end
end