module PointDiff
  class Near < Base
    def valid?
      near?
    end

    def to_binary
      (@dx + 1) * 9 + (@dy + 1) * 3 + (@dz + 1)
    end
  end
end