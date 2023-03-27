namespace Snake
{
    public partial class Snake
    {
        // povolene smery pohybu hada
        public enum Direction
        {
            Right,
            Left,
            Up,
            Down,
        }

        // nedostane data hry, vraci jmeno hada
        public string? Index()
        {
            return Name;
        }

        // dostane data hry, vraci prazdnou odpoved
        public string Init(Game game)
        {
            return string.Empty;
        }

        // dostane data hry, vraci smer pohybu hada
        public Direction Move(Game game)
        {


            // pro ukazku se vraci nahodny smer
            Direction[] directions = (Direction[])Enum.GetValues(typeof(Direction));
            Direction randomDirection = directions[(new Random()).Next(directions.Length)];

            return randomDirection;
        }
    }
}
