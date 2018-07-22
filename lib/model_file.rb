require "voxel"

Model = Struct.new(:dimensions, :points) do
  def print(view_type = nil)
    if view_type && (view_type == "simple_view")
      puts dimensions.to_s
      points.each do |layer|
        layer_header = ""
        layer_output = ""

        layer.each do |row|
          layer_output += row.join + "\n"
        end

        if layer_output.include? "1"
          puts layer_header + "" + layer_output + ""
        end
      end
    else
      points.each.with_index do |layer, y|
        layer_header = "  -- ##{y} --"
        layer_output = ""

        layer.each do |row|
          layer_output += row.join + "\n"
        end

        if layer_output.include? "1"
          puts layer_header + "\n" + layer_output + "\n"
        end
      end

      puts "  -- dimensions: #{dimensions} --"
    end
  end

  def voxel(point)
    Voxel.new(point.x, point.y, point.z, Voxel.full?(points[point.y][point.z][point.x]))
  end

  def voxels
    vxls = []
    points.each.with_index do |layer, y|
      layer.each.with_index do |row, z|
        vxls.concat(
          row
            .map
            .with_index
            .map do |cell, x|
            Voxel.new(x, y, z, Voxel.full?(cell))
          end
        )
      end
    end
    vxls
  end

  def filled_voxels
    voxels.select { |voxel| voxel.full }
  end
end

module ModelFile
  def self.parse_file(filepath)
    model_data = File.binread(filepath)
    dimensions, fill_bits = model_data.unpack("Cb*")
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
      .map(&:reverse)

    Model.new(dimensions, points)
  end
end
