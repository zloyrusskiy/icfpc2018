#!/usr/bin/env ruby
require 'bunny'
require 'json'
require 'base64'

OUTPUT_DIR = File.expand_path('../tmp/traces', __dir__)

conn = Bunny.new(
  host: ENV['RMQ_HOST'] || 'localhost',
  vhost: '/icfpc',
  user: 'icfpc',
  password: '8V02X0XopUKE8RTd'
)
conn.start
at_exit { conn.close }

ch = conn.create_channel
ch.prefetch(1)
q  = ch.queue('results', auto_delete: false, durable: true)

q.subscribe(manual_ack: true) do |delivery_info, properties, payload|
  puts "got #{payload}"

  data = JSON.parse(payload)

  filename = File.join(OUTPUT_DIR, data['trace_name'])
  File.unlink(filename) rescue nil

  file_data = Base64.decode64(data['trace_data'])
  File.write(filename, file_data)

  ch.acknowledge(delivery_info.delivery_tag, false)
end
