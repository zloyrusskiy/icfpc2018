module Command
	class Fission < Base
		def initialize near_diff, seeds_qty
			@nd = near_diff
			@seeds_qty = seeds_qty
		end

		def to_s
			"Fission #{@nd} @seeds_qty"
		end

		def to_binary
			[((@nd.to_binary << 3) ^ 0b101), @seeds_qty]
		end

		def valid? state
			@nd.instance_of?(PointDiff::Near) and @nd.valid? and @seeds_qty < bot.seeds_qty 
		end
	end
end