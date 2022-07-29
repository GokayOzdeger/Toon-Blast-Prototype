# Grid System

Grid system is an abstract grid calculator which offers helper functions for other systems
All grid changes are created by `IGridEvent`s.
After the execution of a grid event ends all cached resulting grid changes starts to execute.

---

## Sub Systems And Their Responsibilities

> ### Grid Entity 
> An entity system that is controlled by the Grid.
> New Grid Entity scripts can easily be created from `IGridEntity`.

> ### GridEntitySpawnController
> Commanded by outside, its only purpose is to create the right GridEntities in the right places.
> [GridStartLayout](GridEntitySpawnController/ReadMe.md)

> ### GridEvents 
> Events that cause changes in the grid are created and controlled by GridEvents

> ### GridGoalsController
> This system manages the goals of the current level and creates visiual effects like flying entity sprites or particles
> the system to watch level completion is this one

> ### Moves Controller
> Checks the possibility of requsted moves and tracks the number of moves left for level lose
> after last move is played and grid is ready for next move (all grid changes/calculations completed) calls the Level Failed


> ### Shuffle Controller
> This class controls if there area any legals moves or not, accordingly it shuffles the grid in a fancy way

---





