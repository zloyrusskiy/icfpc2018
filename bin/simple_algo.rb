#!/usr/bin/env ruby
$LOAD_PATH.unshift File.expand_path('../../lib', __FILE__)

require 'model_file'
require 'command'

model_path = ARGV.first
abort "USAGE: #{$0} <filepath>" unless model_path

model = ModelFile.parse_file(model_path, view_type)
commands = Algorithm::Simple.solve(model)

Command.print_commands(commands)