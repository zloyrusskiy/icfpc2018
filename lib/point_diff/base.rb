module PointDiff
  class Base
    attr_reader :dx, :dy, :dz
    LONG_DISTANCE = 15
    SHORT_DISTANCE = 5

    def initialize(dx, dy, dz)
      @dx = dx
      @dy = dy
      @dz = dz
    end

    def self.calc_diff(point1, point2)
      PointDiff::Base.new(
          point1.x - point2.x,
          point1.y - point2.y,
          point1.z - point2.z
      )
    end

    def self.from_s(str)
      coords = str[1..-1].split(',').map(&:to_i)
      new(*coords)
    end

    def to_s
      "<#{@dx},#{@dy},#{@dz}>"
    end

    def mlen
      [@dx.abs, @dy.abs, @dz.abs].sum
    end

    def clen
      [@dx.abs, @dy.abs, @dz.abs].max
    end

    def sld?
      mlen <= SHORT_DISTANCE
    end

    def lld?
      mlen <= LONG_DISTANCE
    end

    def near?
      mlen > 0 && mlen <= 2 && clen == 1
    end
  end
end