# Ant Runner
A programming exercise game. Create an AI for your cyborg ant to battle up to 8 cyborg ants in a game of capture the flag. Ants are randomly placed on the map knowing only the map size, their starting position, and their own color. They must use echo location to find their way around the map being careful not to run into walls, other ants, or step on bombs. Your ant isn't defense less, it's armed with a high powered laser cannon, a defensive energy shield, and can pick up bombs to deploy traps for other ants. Each ant gets 250ms of processing time to decide what to do next or else they miss their turn.

![Preview](https://github.com/GimpArm/AntRunner/raw/master/AntRunner-Preview.gif)

## Getting Started
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
Download the [latest release](https://github.com/GimpArm/AntRunner/releases/download/1.1.0.0/AntRunner-1.1.0.zip) and you will find everything you need to start with an [ExampleAnt project](bin/AntRunner-1.0.0.0/ExampleAnt). [Read the wiki](https://github.com/GimpArm/AntRunner/wiki) for more information about creating and debugging an ant and an explanation of all objects in the AntRunner.Interface library.

=======
Download the [latest release](https://github.com/GimpArm/AntRunner/releases/download/1.0.0.0/AntRunner-Latest.zip) and you will find everything you need to start with an [ExampleAnt project](bin/AntRunner-1.0.0.0/ExampleAnt). Read the [wiki](wiki/AntRunner.Interface) for more information about the AntRunner.Interface library.
=======
Download the [latest release](https://github.com/GimpArm/AntRunner/releases/download/1.0.0.0/AntRunner-Latest.zip) and you will find everything you need to start with an [ExampleAnt project](bin/AntRunner-1.0.0.0/ExampleAnt). Read the [wiki](https://github.com/GimpArm/AntRunner/wiki) for more information about the AntRunner.Interface library.
>>>>>>> 30fcd6d... Update README.md

### Building
Create a .Net assembly (Standard 2.0 preferred), include AntRunner.Interface.dll as a reference and create a class which inherts from AntRunner.Interface.Ant. Build your project and load the resulting DLL into AntRunner.exe.

## Debugging
Execute the AntRunner.exe with the argument `debug` then attach to the process with the Visual Studio debugger. The debug argument waits for each ant to finish before continuing so you have time to debug. You can also press **F5** while AntRunner.exe is running to toggle debug mode on and off.
```
AntRunner.exe debug
```
<<<<<<< HEAD


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
>>>>>>> 3203942... Update README.md
=======
>>>>>>> 537b2c9... Update README.md
=======
Download the [latest release](https://github.com/GimpArm/AntRunner/releases/download/1.0.0.0/AntRunner-Latest.zip) and you will find everything you need to start with an [ExampleAnt project](bin/AntRunner-1.0.0.0/ExampleAnt). [Read the wiki](https://github.com/GimpArm/AntRunner/wiki) for more information about creating and debugging an ant and an explanation of all objects in the AntRunner.Interface library.
=======
Download the [latest release](https://github.com/GimpArm/AntRunner/releases/download/1.1.0.0/AntRunner-1.1.0.zip) and you will find everything you need to start with an [ExampleAnt project](bin/AntRunner-1.0.0.0/ExampleAnt). [Read the wiki](https://github.com/GimpArm/AntRunner/wiki) for more information about creating and debugging an ant and an explanation of all objects in the AntRunner.Interface library.
>>>>>>> 3801230... Update README.md

>>>>>>> 573aee1... Update README.md
