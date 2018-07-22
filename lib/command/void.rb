module Command
  class Void < Base
    def initialize(near_diff)
      @nd = near_diff
    end

    def to_s
      "Void #{@nd}"
    end

    def to_binary
      (@nd.to_binary << 3) ^ 0b010
    end

    def valid?(state)
      @nd.instance_of?(PointDiff::Near) and @nd.valid?
    end
  end
end
