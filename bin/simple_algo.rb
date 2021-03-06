#!/usr/bin/env ruby
$LOAD_PATH.unshift File.expand_path('../../lib', __FILE__)

require 'algorithm'
require 'model_file'
require 'command'

model_path = ARGV.first
abort "USAGE: #{$0} <filepath>" unless model_path

model = ModelFile.parse_file(model_path)
commands = Algorithm::Simple.new.solve(model)

Command.print_commands(commands)