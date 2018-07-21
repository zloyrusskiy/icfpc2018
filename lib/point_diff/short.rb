module PointDiff
	class Short < Base
		def valid?
		end

		def a
			case
			when @x != 0 then 0b01
			when @y != 0 then 0b10
			when @z != 0 then 0b11
			else
				raise 'strange short diff a value'
			end
		end

		def i
			case
			when @x != 0 then @x + 5
			when @y != 0 then @y + 5
			when @z != 0 then @z + 5
			else
				raise 'strange short diff i value'
			end
		end
	end
end