module Command
	class FusionS < Base
		def initialize near_diff
			@nd = near_diff
		end

		def to_s
			"FusionS #{@nd}"
		end

		def to_binary
			(@nd.to_binary << 3) ^ 0b110
		end

		def valid? state
			@nd.instance_of?(PointDiff::Near) and @nd.valid?
		end
	end
end