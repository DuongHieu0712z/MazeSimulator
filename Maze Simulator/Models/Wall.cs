using System;

namespace Maze_Simulator.Models
{
    [Flags]
    public enum Wall
    {
        None  = 0,
        North = 1,
        South = 2,
        East  = 4,
        West  = 8,
        Full  = North | South | East | West
    }
}
