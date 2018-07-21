module Command
	class LMove < Base
		def initialize short_diff1, short_diff2
			@sld1 = short_diff1
			@sld2 = short_diff2
		end

		def to_s
			"LMove #{@sld1} #{@sld2}"
		end

		def to_binary
			[(@sld2.a << 6) ^ (@sld1.a << 4) ^ 0b1100, (sld2.i << 4) ^ (sld1.i << 4)]
		end

		def valid? state
			@sld1.instance_of?(PointDiff::Short) and
			@sld2.instance_of?(PointDiff::Short) and
			@sld1.valid? and
			@sld2.valid?
		end
	end
end