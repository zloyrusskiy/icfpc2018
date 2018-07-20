Model = Struct.new(:dimensions, :points) do
	def print
		points.each.with_index do |matrix, ind|
			matrix_header = "  -- ##{ind} --"
			matrix_output = ''

			matrix.each do |row|
				matrix_output += row.join() + "\n"
			end

			if matrix_output.include? '1'
				puts matrix_header + "\n" + matrix_output + "\n"
			end
		end

		puts "  -- dimensions: #{dimensions} --"
	end
end

module ModelFile
	def self.parse_file filepath
		model_data = File.binread(filepath)
		dimensions, fill_bits = model_data.unpack('Cb*')
		total_fill_bits = dimensions ** 3

		excess_bits_qty = fill_bits.size - total_fill_bits

		points = fill_bits
			.chars
			.drop(excess_bits_qty)
			.each_slice(dimensions)
			.each_slice(dimensions)
			.to_a
			.transpose
			.map(&:transpose)
			.map(&:reverse)

		Model.new(dimensions, points)
	end
end