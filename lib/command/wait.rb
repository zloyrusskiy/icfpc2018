module Command
	class Wait < Base
		def to_s
			'Wait'
		end

		def to_binary
			0b11111110
		end

		def valid? state
			true
		end
	end
end