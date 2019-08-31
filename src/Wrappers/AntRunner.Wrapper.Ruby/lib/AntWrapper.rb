if !File.file?(ARGV[0])
	raise("Ant file '#{ARGV[0]}' does not exist!")
end
require("json")
require_relative("AntRunner.rb")
require(ARGV[0])

STDOUT.sync = true

__CurrentAnt__ = AntRunner::Interface::Ant.Descendants().first.new()
__Running__ = true

while __Running__ && STDIN.gets
	begin
		line = $_.strip!
		if line.nil? || line.length == 0
			next
		end
		cmd = line[0]
		case cmd
		when "T"
			__CurrentAnt__.Tick(AntRunner::Interface::GameState.from_json(line[1..-1]))
			STDOUT.write("ok\n")
		when "A"
			a = __CurrentAnt__.Action
			STDOUT.write("#{a}\n")
			__CurrentAnt__.Action = AntRunner::Interface::AntAction::Wait
		when "I"
			i = JSON.parse(line[1..-1])
			__CurrentAnt__.Initialize(i[0], i[1], i[2], i[3], i[4])
		when "N"
			STDOUT.write("#{__CurrentAnt__.Name}\n")
		when "P"
			STDOUT.write("Ping\n")
		when "X"
			__Running__ = false
		else
			STDOUT.write("Invalid command\n")
		end
	rescue
		#Do Nothing
	end
end