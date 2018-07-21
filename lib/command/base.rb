module Command
	class Base
		def to_s
			raise NotImplementedError
		end

		def to_binary
			raise NotImplementedError
		end

		def valid? state
			raise NotImplementedError
		end
	end
end