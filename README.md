This game is a combination of bullet hell space shooter and a bit of RTS with AI.

In this project there are 2 bigger separate projects and few smaller that I tried to put together and failed(second attempt at this project). Now i'm reworking the whole thing so the whole project structure is a bit messy.

Projects:
## Motherships and bullet hell
Some stuff is under /Assets/Motherships/, but this project is really not organized because it's the main project where i'm putting code together.
Sprites are under /Assets/Motherships/Sprites
Only real level with proper gameplay for now is scene "Level0". I'm rewriting some code now so a few things might not work properly. A working demo for older version is on dropbox: https://www.dropbox.com/s/u9ogu5z3mlo50ik/MothershipHell_FirstTestLevel_SecondFeedback_23_08_2016.rar?dl=0

Gameplay for the game on dropbox link:
Destroy all 5 carriers in 3 minutes. Actualy it's more like 2 minutes because get game starts to lag because of poor collisions.

Tips: Because the cannons cant reach carriers you have to first collect enough cash, then build laser cannon on grey square.
There are 6 carriers, 1 close, 5 far on top symetricaly spread around.
You should manualy control the laser gun

Controls:
Select guns to swap between auto and manual aim on them.

Building platform is the grey square. Select it and build laser cannon on it.

You DO NOT move the ship around.


## basic space shooter with a few levels
/Assets/SimpleSpaceShooter
Second bigger project in this 1, experimented with space shooter made of basic shapes.
Levels are fully working. /Assets/SimpleSpaceShooter/Levels/

Gameplay: shoot enemies and get to the end of the level.

Controls: 
move with mouse
shoot by holding space


## some "realistic" space movement simulation
/Assets/TotalyAccurateBattleSim/
Contains ship movement based on simulation of engines in space instead of regular movement.
Also contains ai that dymanicaly moves in formations.
- broken movement

## basic tower defense
Only 1 working level, /Assets/SimpleTD/Scenes/demo
Building system, wave system and some pathfinding with waypoints.
