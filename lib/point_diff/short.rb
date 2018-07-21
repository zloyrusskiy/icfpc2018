module PointDiff
  class Short < Base
    def valid?
      sld?
    end

    def a
      case
      when @dx != 0 then
        0b01
      when @dy != 0 then
        0b10
      when @dz != 0 then
        0b11
      else
        raise 'strange short diff a value'
      end
    end

    def i
      case
      when @dx != 0 then
        @dx + LONG_DISTANCE
      when @dy != 0 then
        @dy + LONG_DISTANCE
      when @dz != 0 then
        @dz + LONG_DISTANCE
      else
        raise 'strange short diff i value'
      end
    end
  end
end