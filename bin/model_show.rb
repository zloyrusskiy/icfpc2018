#!/usr/bin/env ruby
$LOAD_PATH.unshift File.expand_path('../../lib', __FILE__)

require 'model_file'

model_path = ARGV.first
view_type = ARGV[1]
abort "USAGE: #{$0} <filepath>" unless model_path

model = ModelFile.parse_file(model_path, view_type)
model.print