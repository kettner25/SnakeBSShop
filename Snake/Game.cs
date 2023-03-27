namespace Snake
{
    public class Game
    {
        public Board? Board { get; set; }
        public Snake? You { get; set; }
        public int Iteration { get; set; }
    }

    public partial class Snake
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Url { get; set; }
        public Coordinate? Head { get; set; }
        public Coordinate[]? Body { get; set; }
        public int Length { get; set; }
        public int Energy { get; set; }
        public bool Alive { get; set; }
        public string? Color { get; set; }
        public int Latency { get; set; }
        public string? DeathCause { get; set; }
        public int? DeathIteration { get; set; }
    }

    public class Board
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public Coordinate[]? Food { get; set; }
        public Coordinate[]? Obstacles { get; set; }
        public Snake[]? Snakes { get; set; }
    }

    public class Coordinate
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}
