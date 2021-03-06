# ruby .\model_calculation.rb "D:/University/gitRepo/icfp/problemsF" "D:/University/gitRepo/icfp/tracesF"
# 2 args: first - path to folder with models, second - path to folder with calculated traces

require "etc"
require "parallel"

threads = []
models_path = ARGV.first
models = Dir.glob(models_path + "/**/FA*.mdl").select { |e| File.file?(e) }
traces_path = ARGV[1]
puts "We have #{models.length}, so let's do it!"
# models = models[0..models.length / 30]

Parallel.each(models, in_threads: Etc.nprocessors) do |model|
  model_name = model.split("/").last
  model_show_cmd = "ruby " + __dir__ + "/model_show.rb '#{model}' simple_view"
  optimizer_type = model_name.start_with?("FA") ? "construct" : (model_name.start_with?("FD") ? "deconstruct" : "")
  dotnet_optimizer_cmd = "dotnet " + __dir__ + "/../c_sharp/src/TraceOptimizer/bin/Release/netcoreapp2.1/TraceOptimizer.dll #{optimizer_type}"
  ruby_trace_to_text_cmd = "ruby " + __dir__ + "/trace_text_to_binary.rb > #{traces_path}/#{model_name.gsub("_tgt", "").gsub("_src", "").gsub(".mdl", ".nbt")}"
  command = "#{model_show_cmd} | #{dotnet_optimizer_cmd} | #{ruby_trace_to_text_cmd}"
  # command = "#{model_show_cmd} | #{dotnet_optimizer_cmd} > 1.txt "
  start_time = Time.now
  puts "#{model} (#{Parallel.worker_number}) (#{start_time}) STARTED.\n"
  system command
  end_time = Time.now
  puts "#{model} (#{Parallel.worker_number}) (#{end_time}) ENDED! Execution time: #{end_time - start_time}\n"
end
