module Command
	class Fill < Base
		def initialize near_diff
			@nd = near_diff
		end

		def to_s
			"Fill #{@nd}"
		end

		def to_binary
			(@nd.to_binary << 3) ^ 0b011
		end

		def valid? state
			@nd.instance_of?(PointDiff::Near) and @nd.valid?
		end
	end
end