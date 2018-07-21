require 'voxel'

Model = Struct.new(:dimensions, :points) do
  def print view_type = nil
    if view_type && (view_type == 'simple_view')
      puts dimensions.to_s
      points.each.with_index do |matrix, _ind|
        matrix_header = ''
        matrix_output = ''

        matrix.each do |row|
          matrix_output += row.join + "\n"
        end

        if matrix_output.include? '1'
          puts matrix_header + '' + matrix_output + ''
        end
      end
    else
      points.each.with_index do |matrix, ind|
        matrix_header = "  -- ##{ind} --"
        matrix_output = ''

        matrix.each do |row|
          matrix_output += row.join + "\n"
        end

        if matrix_output.include? '1'
          puts matrix_header + "\n" + matrix_output + "\n"
        end
      end

      puts "  -- dimensions: #{dimensions} --"
    end
  end

  def get_voxel(point)
    Voxel.new(point.x, point.y, point.z, Voxel.full?(points[point.x][point.y][point.z]))
  end

  def get_filled_voxels
    voxels = []
    points.each.with_index do |matrix, matrix_index|
      matrix.each.with_index do |row, row_index|
        voxels.concat(
            row
                .map
                .with_index
                .select {|cell, _cell_index| Voxel.full?(cell)}
                .map do |_cell, cell_index|
              Voxel.new(matrix_index, row_index, cell_index, true)
            end
        )
      end
    end
    voxels
  end
end

module ModelFile
  def self.parse_file(filepath)
    model_data = File.binread(filepath)
    dimensions, fill_bits = model_data.unpack('Cb*')
    total_fill_bits = dimensions ** 3

    excess_bits_qty = fill_bits.size - total_fill_bits

    points = fill_bits
                 .chars
                 .drop(excess_bits_qty)
                 .map(&:to_i)
                 .each_slice(dimensions)
                 .each_slice(dimensions)
                 .to_a
                 .transpose
                 .map(&:transpose)
                 # .map(&:reverse) # z вверху

    Model.new(dimensions, points)
  end
end
