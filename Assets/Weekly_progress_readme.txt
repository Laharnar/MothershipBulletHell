File was added 9.5.2017, third attempt at this game.
In this project there are 2 bigger separate projects and few smaller that I tried to put together and failed(second attempt at this project).

Note: the whole project structure is a bit messy.

1) Motherships and bullet hell
Some stuff is under /Assets/Motherships/, but this project is really not organized because it's a mix of multiple projects.
Sprites are under /Assets/Motherships/Sprites
The biggest project here, the rest were made more or less as prototypes.
Only real level with proper gameplay for now is scene "Level0". Search for it in project window. I'm rewriting some code now so a few things might not work properly.

2) simple space shooter
/Assets/SimpleSpaceShooter
Second bigger project in this 1, experimented with space shooter made of basic shapes.
A few levels and a map are under /Assets/SimpleSpaceShooter/Levels/

3) some "realistic" space movement simulation
/Assets/TotalyAccurateBattleSim/
Also contains ai that dymanicaly moves in formations.
- broken movement

4) basic tower defense
Only 1 working level, /Assets/SimpleTD/Scenes/demo
Just building system, wave system and some pathfinding.

5) 

Health/collisions are still broken.

Goals (long term) ***************************
- focus on creating interesting experience first, healthy gameplay second.
- redo all ai into utility based ai while replacing old code.
- redo building system(currently completly broken)
- redo collisions
- redo movement, so it won't be lag sensitive

! add realtime formations again from that other project
! add drop pods with units inside. add inside battles in ships

Targets(short term) *************************
Priority 1-5, 5 is highest, rated on time required for it and value
(5)
Pooling doesn't work on bullets. (forgot to derive bullet/gun class from it?)

Rework first level.

(4)
Paths are made with vectors instead of instantiated objects.
- every node decides which paths are valid from it based on some "model" function, like a circle



(2)
Base ai class for all ai, use it to display action in ai editor.

(1-currently)
Figure out what is wrong with bullet collisions.


Weekly updates:
15.5.2017
Reworked bomber ai. Look at SuicideBomber_v3.cs for more details.
You can select any Ai that goes from AIBase class in Ai display editor.
Added script that enables me to use "nested prefabs", by parenting objects at runtime.
Added replace groups of objects editor.

9.5.2017
Reworked Assault craft AI.
Added AI display editor.
Added pooling system for bullets and ships - better performance.
Got rid of those 10000 null reference bugs.
Fixed camera zoom.