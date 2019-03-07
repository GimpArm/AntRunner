var AntRunner = require("AntRunner");

module.exports = (function() {
    var MapPosition = function() {
        this.Known = false;
        this.Item = AntRunner.Item.Empty;
    };
    return MapPosition;
}());
