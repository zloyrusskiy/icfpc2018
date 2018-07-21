module Command
	class Flip < Base
		def to_s
			'Flip'
		end

		def to_binary
			0b11111101
		end

		def valid? state
			true
		end
	end
end