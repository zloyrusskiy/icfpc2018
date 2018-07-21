module Command
	class Halt < Base
		def to_s
			'Halt'
		end

		def to_binary
			0b11111111
		end

		def valid? state
			true
		end
	end
end