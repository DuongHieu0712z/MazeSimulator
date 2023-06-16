namespace Maze_Simulator.Models
{
    public class Linker
    {
        public Linker(Cell start, Cell end)
        {
            Start = start;
            End = end;
        }

        public Cell Start { get; set; }

        public Cell End { get; set; }
    }
}
