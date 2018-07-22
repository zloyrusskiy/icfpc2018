#!/usr/bin/env ruby
require 'bunny'
require 'json'
require 'base64'

MODEL_DIR = File.expand_path('../data/models', __dir__)

def model_filename(model_index, prefix, type)
  File.join(MODEL_DIR, 'F%s%03d_%s.mdl' % [prefix, model_index, type])
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
q  = ch.queue('tasks', auto_delete: false, durable: true)
x  = ch.default_exchange

model_index = 1

loop do
  puts "model ##{model_index}"

  sent_something = false

  if File.exist? model_filename(model_index, 'A', 'tgt')
    model_data = File.binread(model_filename(model_index, 'A', 'tgt'))
    msg = {
      type: 'A',
      ind: model_index,
      tgt: Base64.encode64(model_data)
    }
    x.publish(JSON.generate(msg), routing_key: q.name, persistent: true, content_type: 'text/json')
    sent_something = true
  end

  if File.exist? model_filename(model_index, 'D', 'src')
    model_data = File.binread(model_filename(model_index, 'D', 'src'))
    msg = {
      type: 'D',
      ind: model_index,
      src: Base64.encode64(model_data)
    }
    x.publish(JSON.generate(msg), routing_key: q.name, persistent: true, content_type: 'text/json')
    sent_something = true
  end

  if File.exist? model_filename(model_index, 'R', 'src')
    src = File.binread(model_filename(model_index, 'R', 'src'))
    tgt = File.binread(model_filename(model_index, 'R', 'tgt'))
    msg = {
      type: 'R',
      ind: model_index,
      tgt: Base64.encode64(tgt),
      src: Base64.encode64(src)
    }
    x.publish(JSON.generate(msg), routing_key: q.name, persistent: true, content_type: 'text/json')
    sent_something = true
  end

  model_index += 1
  break if sent_something == false
end
