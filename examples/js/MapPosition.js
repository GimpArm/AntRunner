var AntRunner = require("AntRunner");

(function() {
    var mapPosition = function() {
        this.Known = false;
        this.Item = AntRunner.Item.Empty;
    };
    module.exports.Create = function() { return new mapPosition(); };
}());
