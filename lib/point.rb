require 'distance'

class Point
	attr_reader :x, :y, :z

	def initialize x, y, z
		@x = x
		@y = y
		@z = z
	end

	def near_diff point
		Distance::NearDiff.new(self, point)
	end

	def short_diff point
		Distance::ShortDiff.new(self, point)
	end

	def long_diff point
		Distance::LongDiff.new(self, point)
	end
end