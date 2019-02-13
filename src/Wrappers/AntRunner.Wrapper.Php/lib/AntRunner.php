<?php namespace AntRunner_Interface;

class Ant
{
	public $Name;
	public $Action;
	
	public function Initialize($mapWidth, $mapHeight, $antColor, $startX, $startY) {}
	
	public function Tick(GameState $state) {}
}

abstract class AntAction
{
    const Wait = 0;
    const MoveRight = 1;
	const MoveDown = 2;
	const MoveLeft = 3;
	const MoveUp = 4;
	const EchoRight = 5;
	const EchoDown = 6;
	const EchoLeft = 7;
	const EchoUp = 8;
	const ShieldOn = 9;
	const ShieldOff = 10;
	const DropBomb = 11;
	const ShootRight = 12;
	const ShootDown = 13;
	const ShootLeft = 14;
	const ShootUp = 15;
}

abstract class DamageValues
{
    const Collision = 5;
    const Impact = 10;
	const Shot = 20;
	const Bomb = 30;
}

abstract class GameEvent
{
    const Nothing = 0;
    const CollisionDamage = 1;
	const ImpactDamageRight = 2;
	const ImpactDamageDown = 4;
	const ImpactDamageLeft = 8;
	const ImpactDamageUp = 16;
	const ShotDamageRight = 32;
	const ShotDamageDown = 64;
	const ShotDamageLeft = 128;
	const ShotDamageUp = 256;
	const BombDamage = 512;
	const PickUpBomb = 1024;
	const PickUpShield = 2048;
	const PickUpHealth = 4096;
	const PickUpFlag = 8192;
	const Dead = 16384;
	const GameOver = 32768;
}

abstract class Item
{
    const Empty = 0;
    const SteelWall = 1;
	const BrickWall = 2;
	const Bomb = 3;
	const PowerUpBomb = 4;
	const PowerUpHealth = 5;
	const PowerUpShield = 6;
	const RedAnt = 7;
	const BlueAnt = 8;
	const GreenAnt = 9;
	const OrangeAnt = 10;
	const PinkAnt = 11;
	const YellowAnt = 12;
	const GrayAnt = 13;
	const WhiteAnt = 14;
	const RedHome = 15;
	const BlueHome = 16;
	const GreenHome = 17;
	const OrangeHome = 18;
	const PinkHome = 19;
	const YellowHome = 20;
	const GrayHome = 21;
	const WhiteHome = 22;
	const Flag = 23;
}

abstract class ItemBonusValues
{
    const Health = 25;
    const Shield = 25;
	const Bomb = 4;
}

abstract class ItemColor
{
    const None = 0;
    const Red = 1;
	const Blue = 2;
	const Green = 3;
	const Orange = 4;
	const Pink = 5;
	const Yellow = 6;
	const Gray = 7;
	const White = 8;
}

class EchoResponse
{
	public $Distance;
	public $Item;

	function __construct($distance, $item)
	{
		$this->Distance = $distance;
		$this->Item = $item;
	}
}

class GameState
{
	public $TickNumber;
	public $HasFlag;
	public $FlagX;
	public $FlagY;
	public $AntWithFlag;
	public $Response;
	public $Event;
		
	function __construct($tickNumber, $event = GameEvent::Nothing, $response = null, $flagX = -1, $flagY = -1, $antWithFlag = ItemColor::None)
	{
		$this->TickNumber = $tickNumber;
		$this->HasFlag = $flagX != -1;
		$this->FlagX = $flagX;
		$this->FlagY = $flagY;
		$this->AntWithFlag = $antWithFlag;
		$this->Response = $response;
		$this->Event = $event;
	}
}