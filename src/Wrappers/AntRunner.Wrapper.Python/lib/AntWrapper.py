import sys, inspect, json
import AntRunner.Interface

if len(sys.argv) < 2 or not sys.argv[1]:
	raise Exception("Must supply Ant module name")

if len(sys.argv) > 2 and sys.argv[2].startswith("debug:"):
	port = int(sys.argv[2][6:])
	if port > 1000:
		import ptvsd
		ptvsd.enable_attach(address=('127.0.0.1', port), redirect_output=False)
		ptvsd.wait_for_attach()

LoadingAnt = __import__(sys.argv[1])

def MakeAnt(mod):
    for name, obj in inspect.getmembers(mod):
        if inspect.isclass(obj) and issubclass(obj, AntRunner.Interface.Ant) and obj is not AntRunner.Interface.Ant:
            return obj()

def DecodeGameState(s):
	d = json.loads(s)
	if d["Response"] is not None:
		d["Response"]=AntRunner.Interface.EchoResponse(**d["Response"])
	return  AntRunner.Interface.GameState(**d)


Ant = MakeAnt(LoadingAnt)


for line in sys.stdin:
	cmd = line[:1]
	if cmd == "T":
		s = DecodeGameState(line[1:])
		Ant.Tick(s)
		a = Ant.Action
		sys.stdout.write(f"{a}\n")
		Ant.Action = 0
	elif cmd == "I":
		i = json.loads(line[1:])
		Ant.Initialize(i[0],i[1],i[2],i[3],i[4])
	elif cmd == "N":
		sys.stdout.write(f"{Ant.Name}\n")
	elif cmd == "P":
		sys.stdout.write("Ping\n")

	sys.stdout.flush()
