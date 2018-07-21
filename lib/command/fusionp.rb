module Command
	class FusionP < Base
		def initialize near_diff
			@nd = near_diff
		end

		def to_s
			"FusionP #{@nd}"
		end

		def to_binary
			(@nd.to_binary << 3) ^ 0b111
		end

		def valid? state
			@nd.instance_of?(PointDiff::Near) and @nd.valid?
		end
	end
end