import json

class Ant:
	Name = "No Name"
	Action = 0

	def Initialize(self, mapWidth, mapHeight, color, startX, startY):
		pass

	def Tick(self, state):
		pass

		
class AntAction:
	Wait = 0
	MoveRight = 1
	MoveDown = 2
	MoveLeft = 3
	MoveUp = 4
	EchoRight = 5
	EchoDown = 6
	EchoLeft = 7
	EchoUp = 8
	ShieldOn = 9
	ShieldOff = 10
	DropBomb = 11
	ShootRight = 12
	ShootDown = 13
	ShootLeft = 14
	ShootUp = 15		

class Item:
	Empty = 0
	SteelWall = 1
	BrickWall = 2
	Bomb = 3
	PowerUpBomb = 4
	PowerUpHealth = 5
	PowerUpShield = 6
	RedAnt = 7
	BlueAnt = 8
	GreenAnt = 9
	OrangeAnt = 10
	PinkAnt = 11
	YellowAnt = 12
	GrayAnt = 13
	WhiteAnt = 14
	RedHome = 15
	BlueHome = 16
	GreenHome = 17
	OrangeHome = 18
	PinkHome = 19
	YellowHome = 20
	GrayHome = 21
	WhiteHome = 22
	Flag = 23		

class GameEvent:
	Nothing = 0
	CollisionDamage = 1
	ImpactDamageRight = 2
	ImpactDamageDown = 4
	ImpactDamageLeft = 8
	ImpactDamageUp = 16
	ShotDamageRight = 32
	ShotDamageDown = 64
	ShotDamageLeft = 128
	ShotDamageUp = 256
	BombDamage = 512
	PickUpBomb = 1024
	PickUpShield = 2048
	PickUpHealth = 4096
	PickUpFlag = 8192
	Dead = 16384
	GameOver = 32768		

class ItemColor:
	None_ = 0
	Red = 1
	Blue = 2
	Green = 3
	Orange = 4
	Pink = 5
	Yellow = 6
	Gray = 7
	White = 8		

class ItemBonusValues:
	Health = 25
	Shield = 25
	Bomb = 4		

class DamageValues:
	Collision = 5
	Impact = 10
	Shot = 20
	Bomb = 30		

class EchoResponse:
	Distance = 0
	Item = 0

	def __init__(self, Distance, Item):
		self.Distance = Distance
		self.Item = Item


class GameState(json.JSONEncoder):
	TickNumber = 0
	HasFlag = False
	FlagX = -1
	FlagY = -1
	AntWithFlag = ItemColor.None_
	Response = None
	Event = 0

	def __init__(self, TickNumber, Event = GameEvent.Nothing, Response = None, FlagX = -1, FlagY = -1, AntWithFlag = ItemColor.None_):
		self.TickNumber = TickNumber
		self.Event = Event
		self.Response = Response
		self.HasFlag = FlagX != -1
		self.FlagX = FlagX
		self.FlagY = FlagY
		self.AntWithFlag = AntWithFlag

	def default(self, obj):
		if isinstance(obj, complex):
			return [obj.real, obj.imag]
		return json.JSONEncoder.default(self, obj)