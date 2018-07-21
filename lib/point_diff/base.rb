module PointDiff
	class Base
		attr_reader :x, :y, :z

		def initialize x,y,z
			@x = x
			@y = y
			@z = z
		end

		def self.from_s str
			coords = str[1..-1].split(',').map(&:to_i)
			self.new(*coords)
		end

		def to_s
			"<#{@x},#{@y},#{@z}>"
		end
	end
end