$LOAD_PATH.unshift File.expand_path('../../lib', __FILE__)

require 'model_file'
require 'utils'

model_path = "D:\\University\\gitRepo\\icfp\\problemsL\\LA001_tgt.mdl"

model = ModelFile.parse_file(model_path)
(0..model.dimensions - 1).each do |y|
  layer = model.points[y]
  rect_bounds_detection = Utils.rect_bounds_detection(layer)
  puts rect_bounds_detection
end