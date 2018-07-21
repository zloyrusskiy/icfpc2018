#!/usr/bin/env ruby
$LOAD_PATH.unshift File.expand_path('../../lib', __FILE__)

require 'point_diff'
require 'command'

commands = []
ARGF.readlines.each.with_index do |line, index|
	cmd, *args = line.strip.split(' ')

	next if cmd == '' or cmd.nil?

	commands << case cmd.downcase
	when 'fill'
		nd = PointDiff::Near.from_s args.first
		Command::Fill.new(nd)
	when 'fission'
		nd = PointDiff::Near.from_s args.first
		qty = args.last.to_i
		Command::Fission.new(nd, qty)
	when 'flip'
		Command::Flip.new
	when 'fusionp'
		nd = PointDiff::Near.from_s args.first
		Command::FusionP.new(nd)
	when 'fusions'
		nd = PointDiff::Near.from_s args.first
		Command::FusionS.new(nd)
	when 'halt'
		Command::Halt.new
	when 'lmove'
		sd1 = PointDiff::Short.from_s args[0]
		sd2 = PointDiff::Short.from_s args[1]
		Command::Lmove.new(sd1, sd2)
	when 'smove'
		ld = PointDiff::Long.from_s args.first
		Command::SMove.new(ld)
	when 'wait'
		Command::Wait.new
	else
		raise 'Unknown command %s at %d line' % [cmd, index + 1]
	end
end

binary_commands = commands.map(&:to_binary).flatten
bin_output = binary_commands.pack('C*')
$stdout.write(bin_output)