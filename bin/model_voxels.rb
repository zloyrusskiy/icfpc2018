$LOAD_PATH.unshift File.expand_path('../../lib', __FILE__)

require 'model_file'

model_path = "D:\\University\\gitRepo\\icfp\\problemsL\\LA001_tgt.mdl"

model = ModelFile.parse_file(model_path)
filled_voxels = model.get_filled_voxels
puts filled_voxels