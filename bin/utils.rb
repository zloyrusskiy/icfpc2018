$LOAD_PATH.unshift File.expand_path('../../lib', __FILE__)

require 'model_file'
require 'utils'
require 'command'
require 'bot'
require 'point'

point = Point.new(36, 5, 3)
bot = Bot.new(Point.new(16, 10, 0))
movement_commands = Utils.go_to_point_commands(bot, point)
Command.print_commands(movement_commands)

model_path = "D:\\University\\gitRepo\\icfp\\problemsL\\LA001_tgt.mdl"

model = ModelFile.parse_file(model_path)
(0..model.dimensions - 1).each do |y|
  layer = model.points[y]
  rect_bounds_detection = Utils.rect_bounds_detection(layer)
  # puts rect_bounds_detection
end

