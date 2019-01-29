# Ant Runner
A .Net programming exercise game. Create an AI for your cyborg ant to battle up to 8 cyborg ants in a game of capture the flag. Ants are randomly placed on the map knowing only the map size, their starting position, and their own color. They must use echo location to find their way around the map being careful not to run into walls, other ants, or step on bombs. Your ant isn't defense less, it's armed with a high powered laser cannon, a defensive energy shield, and can pick up bombs to deploy traps for other ants. Each ant gets 250ms of processing time to decide what to do next or else they miss their turn.

![Preview](https://github.com/GimpArm/AntRunner/raw/master/AntRunner-Preview.gif)

## Getting Started
Download the [latest release](releases/download/1.0.0.0/AntRunner-Latest.zip) and you will find everything you need to start with an [ExampleAnt project](bin/AntRunner-1.0.0.0/ExampleAnt).

### Building
Create a .Net assembly (Standard 2.0 preferred), include AntRunner.Interface.dll as a reference and create a class which inherts from AntRunner.Interface.Ant. Build your project and load the resulting DLL into AntRunner.exe.

### Ant Class (AntRunner.Interface.Ant)
This is the class which all ants must inherit from. It must have an empty public constructor and implement all abstract methods and properties.
```
public class AwesomeAnt : Ant
{
  //Your code here
}
```

### Ant Properties
#### Name
Read only string property which must be overriden. Returns a `string` for the ant's name. It is called once when loading.
```
public override string Name => "Awesome Ant";
```

#### FlagResource
Read only string property which is optional to override. If overriden then it should return a `string` which contains the full name of an embedded image resource that will be used as the ant's flag icon.
```
public override string FlagResource => "AwesomeAnt.Flag.png";
```

### Flag
Read only Stream property which is optional to override. If overriden then it should return a `Stream` which contains the binary data of an image that will be used as the ant's flag icon. If overriden then `FlagResource` is ignored.

### Ant Fields
#### Action
AntAction enum field which is the action to be preformed after the end of the current tick cycle. The game manager will reset this to `AntAction.Wait` after it is read.

### Ant Methods
#### Initialize(int mapWidth, int mapHeight, ItemColor antColor, int startX, int startY)
Void method which is called once before the start of each game to initialize the ant. It must be overridden and is provided with the dimentions of the current map, the ant's color, and ant's starting x and y.
```
public override void Initialize(int mapWidth, int mapHeight, ItemColor antColor, int startX, int startY)
{
    _mapWidth = mapWidth;
    _mapHeight = mapHeight;
    _myColor = antColor;
    _currentX = startX;
    _currentY = startY;
}
```

#### Tick(GameState state)
Void method which is called every 250ms initiating the ant's game turn (tick). If the ant's process is busy then the turn is skipped. It must be overridden and is provided with a GameState object with information about actions from the previous tick. It should set the `Action` field each time it is run to control the ant.
```
public override void Tick(GameState state)
{
    //Do Stuff
    Action = AntAction.MoveRight;
}
```

## Game Enums 
### AntAction
*AntRunner.Interface.AntAction*

Actions that the ant can perform on its turn. Should be set to the `Action` field on each `Tick()`.

#### Wait
Ant does nothing.

#### MoveRight
Ant attempts to move one space to the **right**.

#### MoveDown
Ant attempts to move one space **below**.

#### MoveLeft
Ant attempts to move one space to the **left**.

#### MoveUp
Ant attempts to move one space **above**.

#### EchoRight
Ant performs an echo to the **right**. The response will come in the `GameState` of the next `Tick()` call. See `EchoResponse`

#### EchoDown
Ant performs an echo **below**. The response will come in the `GameState` of the next `Tick()` call. See `EchoResponse`

#### EchoLeft
Ant performs an echo to the **left**. The response will come in the `GameState` of the next `Tick()` call. See `EchoResponse`

#### EchoUp
Ant performs an echo **above**. The response will come in the `GameState` of the next `Tick()` call in. See `EchoResponse`

#### ShieldOn
Ant turns on its shield if it has any. For every 4 ticks the shield is on, it will lose 1 point so it is not wise to always leave the shield on.

#### ShieldOff
Ant turns off its shield.

#### DropBomb
Ant drops a bomb at the current position if it has any.

#### ShootRight
Ant shoot its laser in a straight line to the **right**.

#### ShootDown
Ant shoot its laser in a straight line **below**.

#### ShootLeft
Ant shoot its laser in a straight line to the **left**.

#### ShootUp
Ant shoot its laser in a straight line **above**.


### Item
*AntRunner.Interface.Item*

Items that are on the map which will be returned as part of an `EchoReponse`.

#### Empty
Nothing at all, aka empty space.

#### SteelWall
Unbreakable steel wall.

#### BrickWall
Brick wall which may be destroyed with a laser shot. There is the possibility that a power-up is hidden in the wall when destroyed.

#### Bomb
Bomb that will damage the ant if stepped on. Can be shot with the laser to destroy. Damage amount is defined by the constant `DamageValues.Bomb`.

#### PowerUpBomb
Bomb Power-up, adds bombs to the ant's inventory. Amount of bombs given is defined by the constant `ItemBonusValues.Bomb`.

#### PowerUpHealth
Health Power-up, adds more health level to the ant, maximum 100. Amount of health given is defined by the constant `ItemBonusValues.Health`.

#### PowerUpShield
Shield Power-up, adds more shield level to the ant, maximum 100. Amount of shield given is defined by the constant `ItemBonusValues.Shield`.

#### RedAnt, BlueAnt, GreenAnt, OrangeAnt, PinkAnt, YellowAnt, GrayAnt, WhiteAnt
The ant which currently occupies the space.

#### RedHome, BlueHome, GreenHome, OrangeHome, PinkHome, YellowHome, GrayHome, WhiteHome
The home for the corresponding ant. The flag must be brought to you ant's home to win the game.

#### Flag
The flag, pick this up and bring it to the correct color home. Don't pick it up too early because as soon as you do, all other ants are informed of your position at all times!


### ItemColor
*AntRunner.Interface.ItemColor*

The 8 available colors for ants and their homes and a `None` value so signify no color.

### GameEvent
*AntRunner.Interface.GameEvent*

Enum flags of possible events that can occur as a result of all ants' GameAction. Multiple events may occur at once so it is best to use the HasFlag() method to check.

#### Nothing
Nothing has occurred.

#### CollisionDamage
Ant ran into an object when it attempted to move and incurred damage. The move was unsuccessful and the ant remains at its current location. Damage amount is defined by the constant `DamageValues.Collision`.

#### ImpactDamageRight
Another ant ran into or rammed the ant from the **right** and incurred damage. Damage amount is defined by the constant `DamageValues.Impact`.

#### ImpactDamageDown
Another ant ran into or rammed the ant from **below** and incurred damage. Damage amount is defined by the constant `DamageValues.Impact`.

#### ImpactDamageLeft
Another ant ran into or rammed the ant from the **left** and incurred damage. Damage amount is defined by the constant `DamageValues.Impact`.

#### ImpactDamageUp
Another ant ran into or rammed the ant from **above** and incurred damage. Damage amount is defined by the constant `DamageValues.Impact`.

#### ShotDamageRight
Ant was shot by a laser from the **right** and incurred damage. Damage amount is defined by the constant `DamageValues.Shot`.

#### ShotDamageDown
Ant was shot by a laser from **below** and incurred damage. Damage amount is defined by the constant `DamageValues.Shot`.

#### ShotDamageLeft
Ant was shot by a laser from the **left** and incurred damage. Damage amount is defined by the constant `DamageValues.Shot`.

#### ShotDamageUp
Ant was shot by a laser from the **above** and incurred damage. Damage amount is defined by the constant `DamageValues.Shot`.

#### BombDamage
Ant walked over a bomb and incurred damage. Damage amount is defined by the constant `DamageValues.Bomb`.

#### PickUpBomb
Ant picked up a Bomb Power-up. Amount of bombs given is defined by the constant `ItemBonusValues.Bomb`.

#### PickUpHealth
Ant picked up a Health Power-up. Amount of health given is defined by the constant `ItemBonusValues.Health`.

#### PickUpShield
Ant picked up a Shield Power-up. Amount of shield given is defined by the constant `ItemBonusValues.Shield`.

#### PickUpFlag
Ant picked up the Flag. Run to the correct color home!

#### Dead
Ant has died and will no longer be getting Tick() calls.

#### GameOver
The game is over, either an ant successfully retrieved the flag or all ants have died.


## Game Objects
### EchoResponse
*AntRunner.Interface.EchoResponse*

Response object when an Echo GameAction was made the previous tick.

#### Distance
Integer property containing the value of how many squares away the echo bounces from. All spaces between the ant and the `Item` are `Empty`.

#### Item
Item property containing the `Item` enum of the object located at the specified `Distance`.

### GameState
An object containing information about what ocurred as a results of all ants' actions of the previous `Tick()`.

#### TickNumber
Long property of how many ticks have gone by since the start of the game. Each tick is increased by 1 from the previous. If you see a gap then that means your ant process was busy and did not complete it's processing in 250ms.

#### HasFlag
Boolean property whether an ant has picked up the flag. If this is true and your ant does not have the flag then you should try to kill that ant before it reaches its home.

#### FlagX
Integer property of the X coordinate of the ant that is carrying the flag. Will be -1 is no ant has the flag.

#### FlagY
Integer property of the Y coordinate of the ant that is carrying the flag. Will be -1 is no ant has the flag.

#### AntWithFlag
ItemColor property representing the color of the ant that is cyrrings the flag. Will be None if no ant has the flag.

#### Response
EchoResponse property of the previous tick GameAction. If previous tick was not an echo action then this is null.

#### Event
GameEvent property of the combined events which affected your ant. Multiple events can occur at on.e time, like ShotLeft and Bomb, so it is best to use the HasFlag() enum method.


## Game Constants
### DamageValues
*AntRunner.Interface.DamageValues*

Amount of damage which is applied to the ant's health and shield when they incur damange. If the sheild is on then the amount is subtracted from that first then all remaining is subtracted from the health. If the shields are not on then all damage is subtracted from the health.

#### Collission
Damage applied when you ant runs into an object. **5**

#### Impact
Damage applied when your ant is run into or rammed by another ant. **10**

#### Shot
Damage applied when you ant is shot with a laser. **20**

#### Bomb
Damage applied when an ant steps on a bomb. **30**


### ItemBonusValues
*AntRunner.Interface.ItemBonusValues*

Values of how much is given when a power-up is picked up.

### Health
Amount of health that is restored when a health power-up is acquired. **25**

### Shield
Amount of shield that is restored when a shield power-up is acquired. **25**

### Bomb
Amount of bombs added to an ant's inventory when a bomb power-up is acquired. **4**


## Maps
Maps can be created by making a simple bitmap (bmp) image file and placing it in the Maps folder. Each pixel is a point on the map. All colors which are not defined are ignored.

### SteelWall
Black rgb(0,0,0)
Do not create empty spaces completely surrounded by SteelWall. It is possible for an ant/home/flag to randomly be placed in this space.

### BrickWall
Red rgb(255,0,0)
There should always be some included because the only way to obtain power-ups is by shooting BrickWalls.

### AntHome 
Blue rgb(0,0,255)
If there aren't enough home locations on the map for the ants loaded then homes will be randomly placed. Colors are randomly assigned to each home location. There can be more possible home locations than 8 but there will only be homes allocated for as many ants loaded.

### Flag
Green rgb(0,255,0)
If there is no flag location set then the flag will randomly be placed. There can be multiple possible flag locations and a location will be chosen at random.

## Debugging
Execute the AntRunner.exe with the argument `debug` then attach to the process with the Visual Studio debugger. The debug argument waits for each ant to finish before continuing so you have time to debug. You can also press **F5** while AntRunner.exe is running to toggle debug mode on and off.
```
AntRunner.exe debug
```

