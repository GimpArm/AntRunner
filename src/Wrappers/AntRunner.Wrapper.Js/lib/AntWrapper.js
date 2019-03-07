var net = require("net");
var eol = require('os').EOL;
var loaded = require(process.argv[3]);
var currentAnt = new loaded.default();

var s = net.Server(function (socket) {
    socket.setEncoding("utf8");
    var buffer = "";
    socket.on("data", function (b) {
        try {
            var message = b.toString();
            if (message === "") return;
            var pos = message.indexOf(eol);
            if (pos === -1) {
                buffer += message;
                return;
            }
            buffer += message.substring(0, pos);
            var cmd = buffer;
            buffer = message.substring(pos + eol.length);

            switch (cmd.charAt(0)) {
                case "T":
                    var state = JSON.parse(cmd.substring(1));
                    currentAnt.Tick(state);
                    var a = currentAnt.Action.toString();
                    socket.write(a);
                    currentAnt.Action = 0;
                    break;
                case "I":
                    var init = JSON.parse(cmd.substring(1));
                    currentAnt.Initialize(init[0], init[1], init[2], init[3], init[4], init[5]);
                    break;
                case "N":
                    socket.write(currentAnt.Name);
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
console.log(process.argv[2]);
s.listen(process.argv[2]);