module PointDiff
	class Long < Base
		def valid?
		end

		def a
			case
			when @x != 0 then 0b01
			when @y != 0 then 0b10
			when @z != 0 then 0b11
			else
				raise 'strange long diff a value'
			end
		end

		def i
			case
			when @x != 0 then @x + 15
			when @y != 0 then @y + 15
			when @z != 0 then @z + 15
			else
				raise 'strange long diff i value'
			end
		end
	end
end