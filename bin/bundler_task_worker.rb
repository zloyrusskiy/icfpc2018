#!/usr/bin/env ruby
require 'bunny'
require 'json'
require 'base64'
require 'socket'

OUTPUT_DIR = File.expand_path('../tmp/output', __dir__)

def trace_filename(model_index, prefix)
  'F%s%03d.nbt' % [prefix, model_index]
end

def process_task model_index, type, src, tgt
  model_name = 'm%s%03d.mdl' % [type, model_index]
  model_name_txt = File.join(OUTPUT_DIR, model_name + '.txt')
  model_path = File.join(OUTPUT_DIR, model_name)
  trace_path = File.join(OUTPUT_DIR, model_name + '.nbt')

  case type
  when 'A'
    File.binwrite(model_path, tgt)
    sv_model = `ruby #{__dir__}/model_show.rb '#{model_path}' simple_view`
    File.binwrite(model_name_txt, sv_model)
  when 'D'
    File.binwrite(model_path, src)
    sv_model = `ruby #{__dir__}/model_show.rb '#{model_path}' simple_view`
    File.binwrite(model_name_txt, sv_model)
  when 'R'
    src_name = File.join(OUTPUT_DIR, 's' + model_name)
    tgt_name = File.join(OUTPUT_DIR, 't' + model_name)
    File.binwrite(src_name, src)
    File.binwrite(tgt_name, tgt)

    sv_model = `ruby #{__dir__}/model_show.rb '#{src_name}' simple_view`
    sv_model += "\n"
    sv_model += `ruby #{__dir__}/model_show.rb '#{tgt_name}' simple_view`
    File.binwrite(model_name_txt, sv_model)
  end

  optimizer_type = case type
    when 'A' then 'construct'
    when 'D' then 'deconstruct'
    when 'R' then 'reconstruct'
  end
  dotnet_optimizer_cmd = "dotnet " + __dir__ + "/../c_sharp/src/TraceOptimizer/bin/Release/netcoreapp2.1/TraceOptimizer.dll #{optimizer_type}"
  ruby_trace_to_text_cmd = "ruby " + __dir__ + "/trace_text_to_binary.rb > #{trace_path}"
  command = "cat #{model_name_txt} | #{dotnet_optimizer_cmd} | #{ruby_trace_to_text_cmd}"

  system(command)

  trace_path
end

conn = Bunny.new(
  host: ENV['RMQ_HOST'] || 'localhost',
  vhost: '/icfpc',
  user: 'icfpc',
  password: '8V02X0XopUKE8RTd'
)
conn.start
at_exit { conn.close }

ch = conn.create_channel
q = ch.queue('tasks', auto_delete: false, durable: true)
q_err = ch.queue('errors', auto_delete: false)
q_res = ch.queue('results', auto_delete: false, durable: true)
x = ch.default_exchange

q.subscribe(manual_ack: true, block: true) do |delivery_info, properties, payload|
  begin
    data = JSON.parse(payload)

    type = data['type']
    model_index = data['ind']

    puts "got #{type} #{model_index}"

    tgt = data['tgt'] && Base64.decode64(data['tgt'])
    src = data['src'] && Base64.decode64(data['src'])

    trace_filepath = process_task(model_index, type, src, tgt)
    trace_data = File.binread(trace_filepath)

    msg = {
      trace_name: trace_filename(model_index, type),
      trace_data: Base64.encode64(trace_data)
    }
    x.publish(JSON.generate(msg), routing_key: q_res.name, content_type: 'text/json')

    ch.acknowledge(delivery_info.delivery_tag, false)
  rescue
    ch.reject(delivery_info.delivery_tag, true)

    msg = {
      hostname: Socket.gethostname,
      data: payload,
      err: $!.inspect
    }
    x.publish(JSON.generate(msg), routing_key: q_err.name, content_type: 'text/json')

    sleep 1
  end
end
