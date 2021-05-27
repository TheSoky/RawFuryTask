# RawFuryTask
Raw Fury job application Unity task

This task consists of two different parts separated by folder structure and scenes.

Task 1:
character collects cubes that randomly fall on ground and throws them into appropiate colored container
possible bug on lower FPS is that character misses the goal
point is AI character behaviour

Task 2:
On play random maze is generated with beggining in the middle and end randomly on edge
When maze  is generated pathfinding algorithm is searching for fastest route out
when fastest route is found it is colored green

maze generator was taken from:
https://github.com/c00pala/Unity-2D-Maze-Generator/blob/master/MazeGenerator.cs

with some modifications:
[Modify]Line 310 cellSize = (cellPrefab.transform.localScale.x * 4) / cellPrefab.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit; (adjusted for image size and pixels per unit)
[Added]Line 176 newCell.cScript.IsGoalCell = true; stores Goal cell data outside maze generator for independency
[Commented]Line 47 //private int centreSize = 2; unneeded line
[Added]Line59 public Cell[] CentreCells { get { return centreCells; } } starting cells (centre of maze) getter
[Modify]Line 85 private -> public GenerateMaze method
[Added]Line 82 added if block to start method to generate new maze only if there is no existing one
[Modify]Line 80 -> auto maze generation from start to awake
[Added]Line 71 added serialize field so the value would persist from edit to play mode
[Added]Line 60 added serialize field so the value would persist from edit to play mode
[Modify]Line 304 Destroy replaced with DestroyImmediate so it works in Edit mode
[Added]Line 273 Added neighbouring cells for center into CellScript
[Added]Line 217 Added neighbouring cells for cells aside center, into CellScript

*note* line number may vary as i was modifying it
