var net = require("net");
var AntRunner_Interface = require("AntRunner");

var s = net.Server(function (socket) {
    socket.on("data", function (buffer) {
        try {
            var cmd = buffer.toString();

            switch (cmd.charAt(0)) {
                case "T":
                    var state = JSON.parse(cmd.substring(1));
                    AntRunner_Interface.Ant.Tick(state);
                    var a = AntRunner_Interface.Ant.Action.toString();
                    socket.write(a);
                    AntRunner_Interface.Ant.Action = 0;
                    break;
                case "I":
                    var init = JSON.parse(cmd.substring(1));
                    AntRunner_Interface.Ant.Initialize(init[0], init[1], init[2], init[3], init[4], init[5]);
                    break;
                case "N":
                    socket.write(AntRunner_Interface.Ant.Name);
                    break;
                case "P":
                    socket.write("Ping");
                    break;
                case "X":
                    process.exit();
                    break;
                default:
                    console.log("Invalid command");
                    break;
            }
        }
        catch (e) {
            console.log(e);
        }
    });

    socket.on("end", function () {
        console.log("Disconnected");
    });
});

require(process.argv[3]);
s.listen(process.argv[2]);