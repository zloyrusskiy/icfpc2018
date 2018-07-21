module PointDiff
	class Near < Base
		def valid?
		end

		def to_binary
			(@x + 1) * 9 + (@y + 1) * 3 + (@z + 1)
		end
	end
end