require 'utils'
require 'bot'
require 'point_2d'
require 'point'

describe Utils do 
  it "Utils#draw_layer simple" do
  	bot_position = Point.new(1,0,1)
  	bot = Bot.new(bot_position)
  	matrix = [
  		[0,0,0],
  		[0,1,0],
  		[0,1,1]
  	]
  	left_bottom = Point2D.new(1,1)
  	right_top = Point2D.new(2,2)

  	commands = Utils.draw_layer(bot, matrix, left_bottom, right_top)

  	expect(commands.map(&:to_s)).to eq ["Fill <0,-1,0>", "SMove <0,0,1>", "Fill <0,-1,0>", "SMove <1,0,0>", "Fill <0,-1,0>", "SMove <0,0,-1>"]
  end
end