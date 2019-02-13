(function () {
    var key = Symbol.for("AntRunner.Interface.Ant");
    
    var Ant = function() {
        this.Name = "No Name Js";
        this.Action = 0;
        this.Initialize = function(mapWidth, mapHeight, color, startX, startY) {};
        this.Tick = function(state) {};
    };

    if (!global[key]) {
        global[key] = new Ant();
    }

    Object.defineProperty(module.exports, "Ant", {
        configurable: true,
        get() {
            return global[key]; 
        }
    });

    module.exports.Create = function(ant) {
        global[key] = Object.assign(Ant, ant);
    };


    module.exports.AntAction = Object.freeze({
        "Wait": 0,
        "MoveRight": 1,
        "MoveDown": 2,
        "MoveLeft": 3,
        "MoveUp": 4,
        "EchoRight": 5,
        "EchoDown": 6,
        "EchoLeft": 7,
        "EchoUp": 8,
        "ShieldOn": 9,
        "ShieldOff": 10,
        "DropBomb": 11,
        "ShootRight": 12,
        "ShootDown": 13,
        "ShootLeft": 14,
        "ShootUp": 15
    });

    module.exports.DamageValues = Object.freeze({
        "Collision": 5,
        "Impact": 10,
        "Shot": 20,
        "Bomb": 30
    });

    module.exports.EchoResponse = {
        "Distance": 0,
        "Item": 0
    };

    module.exports.GameEvent = Object.freeze({
        "Nothing": 0x00000,
        "CollisionDamage": 0x00001,
        "ImpactDamageRight": 0x00002,
        "ImpactDamageDown": 0x00004,
        "ImpactDamageLeft": 0x00008,
        "ImpactDamageUp": 0x00010,
        "ShotDamageRight": 0x00020,
        "ShotDamageDown": 0x00040,
        "ShotDamageLeft": 0x00080,
        "ShotDamageUp": 0x00100,
        "BombDamage": 0x000200,
        "PickUpBomb": 0x00400,
        "PickUpShield": 0x00800,
        "PickUpHealth": 0x01000,
        "PickUpFlag": 0x02000,
        "Dead": 0x04000,
        "GameOver": 0x08000
    });

    module.exports.GameState = {
        "TickNumber": 0,
        "HasFlag": false,
        "FlagX": -1,
        "FlagY": -1,
        "AntWithFlag": 0,
        "Response": null,
        "Event": 0
    };

    module.exports.Item = Object.freeze({
        "Empty": 0,
        "SteelWall": 1,
        "BrickWall": 2,
        "Bomb": 3,
        "PowerUpBomb": 4,
        "PowerUpHealth": 5,
        "PowerUpShield": 6,
        "RedAnt": 7,
        "BlueAnt": 8,
        "GreenAnt": 9,
        "OrangeAnt": 10,
        "PinkAnt": 11,
        "YellowAnt": 12,
        "GrayAnt": 13,
        "WhiteAnt": 14,
        "RedHome": 15,
        "BlueHome": 16,
        "GreenHome": 17,
        "OrangeHome": 18,
        "PinkHome": 19,
        "YellowHome": 20,
        "GrayHome": 21,
        "WhiteHome": 22,
        "Flag": 23
    });

    module.exports.ItemBonusValues = Object.freeze({
        "Health": 25,
        "Shield": 25,
        "Bomb": 4
    });

    module.exports.ItemColor = Object.freeze({
        "None": 0,
        "Red": 1,
        "Blue": 2,
        "Green": 3,
        "Orange": 4,
        "Pink": 5,
        "Yellow": 6,
        "Gray": 7,
        "White": 8
    });
}());