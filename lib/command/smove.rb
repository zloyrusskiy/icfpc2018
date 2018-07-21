module Command
	class SMove < Base
		def initialize long_diff
			@lld = long_diff
		end

		def to_s
			"SMove #{@lld}"
		end

		def to_binary
			[(@lld.a << 4) ^ 0b0100, @lld.i]
		end

		def valid? state
			@sld1.instance_of?(PointDiff::Long) and
			@lld.valid?
		end
	end
end