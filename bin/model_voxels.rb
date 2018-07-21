$LOAD_PATH.unshift File.expand_path('../../lib', __FILE__)

require 'model_file'

model_path = "D:\\University\\gitRepo\\icfp\\problemsL\\LA001_tgt.mdl"

model = ModelFile.parse_file(model_path)
puts model.filled_voxels