var AntRunner = require("antrunner");
var MapPosition = require("./MapPosition.js");

var AntAction = AntRunner.AntAction;
var GameEvent = AntRunner.GameEvent;
var Item = AntRunner.Item;
var EchoResponse = AntRunner.EchoResponse;
var GameState = AntRunner.GameState;
var ItemColor = AntRunner.ItemColor;
var DamageValues = AntRunner.DamageValues
var ItemBonusValues = AntRunner.ItemBonusValues

module.exports.default = (function () {
	function ExampleAnt() {
		this.Name =  "NodeJs Ant";
		this.Action=  AntAction.Wait;

		this.MapWidth =  0;
		this.MapHeight = 0;

		this.CurrentX = 0;
		this.CurrentY = 0;

		this.MyColor = ItemColor.None;

		this.Map = [];

		this.CurrentMode = 0;
		this.LastAction = AntAction.Wait;
		this.SearchPrimary = 0;
		this.SearchSecondary = 0;
	}

	ExampleAnt.prototype.Initialize =	function(mapWidth, mapHeight, color, startX, startY) {
		this.MapWidth = mapWidth;
		this.MapHeight = mapHeight;
		this.MyColor = color;
		this.CurrentX = startX;
		this.CurrentY = startY;

		this.CurrentMode = 0;
		this.LastAction = AntAction.Wait;

		//Initialize Map
		this.Map = [];
		for (var y = 0; y<mapHeight; ++y) {
			this.Map[y] = [];
			for (var x = 0; x<mapWidth; ++x) {
				this.Map[y][x] = new MapPosition();
			}
		}

		this.Map[this.CurrentY][this.CurrentX].Known = true;
		this.SetSearchDirection();
	};
		
	ExampleAnt.prototype.Tick = function(state) {
		this.ProcessEcho(state.Response);
		this.ProcessGameEvent(state.Event);

		switch (this.CurrentMode) {
			//Map mode
			case 0:
				this.MapMode();
				break;
		}    
	};

	ExampleAnt.prototype.SetAction = function(a) {
		//Set the action for the current turn.
		this.Action = a;
		//Set the last action so we know the direction on the next turn for successful moving and echo response.
		this.LastAction = a;
	};

	ExampleAnt.prototype.ProcessEcho = function(response) {
		//There was no Echo last turn
		if (response == null) return;

		//Set map tiles
		switch (this.LastAction) {
			case AntAction.EchoRight:
				for (var i = 1; i < response.Distance; ++i) {
					this.Map[this.CurrentY][this.CurrentX + i].Known = true;
				}

				if (this.CurrentX + response.Distance < this.MapWidth) {
					this.Map[this.CurrentY][this.CurrentX + response.Distance].Known = true;
					this.Map[this.CurrentY][this.CurrentX + response.Distance].Item = response.Item;
				}
				break;
			case AntAction.EchoDown:
				for (var i = 1; i < response.Distance; ++i) {
					this.Map[this.CurrentY + i][this.CurrentX].Known = true;
				}

				if (this.CurrentY + response.Distance < this.MapHeight) {
					this.Map[this.CurrentY + response.Distance][this.CurrentX].Known = true;
					this.Map[this.CurrentY + response.Distance][this.CurrentX].Item = response.Item;
				}
				break;
			case AntAction.EchoLeft:
				for (var i = 1; i < response.Distance; ++i) {
					this.Map[this.CurrentY][this.CurrentX - i].Known = true;
				}
				if (this.CurrentX - response.Distance >= 0) {
					this.Map[this.CurrentY][this.CurrentX - response.Distance].Known = true;
					this.Map[this.CurrentY][this.CurrentX - response.Distance].Item = response.Item;
				}
				break;
			case AntAction.EchoUp:
				for (var i = 1; i < response.Distance; ++i) {
					this.Map[this.CurrentY - i][this.CurrentX].Known = true;
				}
				if (this.CurrentY - response.Distance >= 0) {
					this.Map[this.CurrentY - response.Distance][this.CurrentX].Known = true;
					this.Map[this.CurrentY - response.Distance][this.CurrentX].Item = response.Item;
				}
				break;
			default:
				//Should never come here
				return;
		}
	};
	

	ExampleAnt.prototype.ProcessGameEvent = function(e) {
		if (e & GameEvent.CollisionDamage) {
			//we ran into something
			return;
		}

		//Move was successful, update map position
		switch (this.LastAction) {
			case AntAction.MoveRight:
				this.CurrentX += 1;
				break;
			case AntAction.MoveDown:
				this.CurrentY += 1;
				break;
			case AntAction.MoveLeft:
				this.CurrentX -= 1;
				break;
			case AntAction.MoveUp:
				this.CurrentY -= 1;
				break;
		}
	};

	ExampleAnt.prototype.MapMode = function() {
		//Check left
		if (this.CurrentX > 0 && !this.Map[this.CurrentY][this.CurrentX - 1].Known) {
			this.SetAction(AntAction.EchoLeft);
			return;
		}

		//Check Right
		if (this.CurrentX < this.MapWidth - 1 && !this.Map[this.CurrentY][this.CurrentX + 1].Known) {
			this.SetAction(AntAction.EchoRight);
			return;
		}

		//Check Up
		if (this.CurrentY > 0 && !this.Map[this.CurrentY - 1][this.CurrentX].Known) {
			this.SetAction(AntAction.EchoUp);
			return;
		}

		//Check Down
		if (this.CurrentY < this.MapHeight - 1 && !this.Map[this.CurrentY + 1][this.CurrentX].Known) {
			this.SetAction(AntAction.EchoDown);
			return;
		}

		//All tiles next to us are known, move a direction.
		if (this.CanMove(this.SearchPrimary)) {
			this.SetAction(this.SearchPrimary);
			return;
		}

		if (this.CanMove(this.SearchSecondary)) {
			this.SetAction(this.SearchSecondary);
			return;
		}

		//Can't move in that direction any more, try a different direction.
		if (this.LastAction != this.SearchPrimary && this.CanMove(this.OppositeDirection(this.SearchPrimary))) {
			this.SearchPrimary = this.OppositeDirection(this.SearchPrimary);
			this.SetAction(this.SearchPrimary);
			return;
		}

		if (this.CanMove(this.OppositeDirection(this.SearchSecondary))) {
			this.SearchSecondary = this.OppositeDirection(this.SearchSecondary);
			this.SetAction(this.SearchSecondary);
			return;
		}

		//Can't find a direction, reset current search directions
		this.SetSearchDirection();
	};

	ExampleAnt.prototype.SetSearchDirection = function() {
		if (this.CurrentX == 0 || this.MapWidth / this.CurrentX < 0.9) {
			//search right
			this.SearchPrimary = AntAction.MoveRight;
		} else {
			//search left
			this.SearchPrimary = AntAction.MoveLeft;
		}

		if (this.CurrentY == 0 || this.MapHeight / this.CurrentY < 0.9) {
			//search down
			this.SearchSecondary = AntAction.MoveDown;
		} else {
			//search up
			this.SearchSecondary = AntAction.MoveUp;
		}
	};

	ExampleAnt.prototype.CanMove = function(a) {
		switch (a)
		{
			case AntAction.MoveRight:
				if (this.CurrentX + 1 == this.MapWidth) return false;
				nextSpace = this.Map[this.CurrentY][this.CurrentX + 1];
				break;
			case AntAction.MoveDown:
				if (this.CurrentY + 1 == this.MapHeight) return false;
				nextSpace = this.Map[this.CurrentY + 1][this.CurrentX];
				break;
			case AntAction.MoveLeft:
				if (this.CurrentX - 1 == -1) return false;
				nextSpace = this.Map[this.CurrentY][this.CurrentX - 1];
				break;
			case AntAction.MoveUp:
				if (this.CurrentY - 1 == -1) return false;
				nextSpace = this.Map[this.CurrentY - 1][this.CurrentX];
				break;
			default:
				return true;
		}

		return nextSpace.Known && !this.Blocked(nextSpace.Item);
	};

	ExampleAnt.prototype.Blocked = function(i)
	{
		switch (i)
		{
			case Item.SteelWall:
			case Item.BrickWall:
			case Item.Bomb:
				return true;
			case Item.RedHome:
				return this.MyColor != ItemColor.Red;
			case Item.BlueHome:
				return this.MyColor != ItemColor.Blue;
			case Item.GreenHome:
				return this.MyColor != ItemColor.Green;
			case Item.OrangeHome:
				return this.MyColor != ItemColor.Orange;
			case Item.PinkHome:
				return this.MyColor != ItemColor.Pink;
			case Item.YellowHome:
				return this.MyColor != ItemColor.Yellow;
			case Item.GrayHome:
				return this.MyColor != ItemColor.Gray;
			case Item.WhiteHome:
				return this.MyColor != ItemColor.White;
			default:
				return false;
		}
	};

	ExampleAnt.prototype.OppositeDirection = function(a)
	{
		switch (a)
		{
			case AntAction.MoveRight:
				return AntAction.MoveLeft;
			case AntAction.MoveDown:
				return AntAction.MoveUp;
			case AntAction.MoveLeft:
				return AntAction.MoveRight;
			case AntAction.MoveUp:
				return AntAction.MoveDown;
			case AntAction.EchoRight:
				return AntAction.EchoLeft;
			case AntAction.EchoDown:
				return AntAction.EchoUp;
			case AntAction.EchoLeft:
				return AntAction.EchoRight;
			case AntAction.EchoUp:
				return AntAction.EchoDown;
			case AntAction.ShootRight:
				return AntAction.ShootLeft;
			case AntAction.ShootDown:
				return AntAction.ShootUp;
			case AntAction.ShootLeft:
				return AntAction.ShootRight;
			case AntAction.ShootUp:
				return AntAction.ShootDown;
			default:
				return AntAction.Wait;
		}
	};

	return ExampleAnt;
}());