using static Snake.Snake;

namespace Snake
{
    public class Point
    {
        public Point? PrewPoint { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public Point(int x, int y, Point? prewPoint = null)
        {
            PrewPoint = prewPoint;
            X = x;
            Y = y;
        }

        public Direction? GetDirection()
        {
            Point point = this;

            while (point.PrewPoint?.PrewPoint != null) 
            { 
                point = point.PrewPoint;
            }

            if (point.PrewPoint?.X > point.X && point.PrewPoint?.Y == point.Y)
            {
                 return Direction.Left;
            }
            else if (point.PrewPoint?.X < point.X && point.PrewPoint?.Y == point.Y)
            {
                return Direction.Right;
            }
            else if (point.PrewPoint?.X == point.X && point.PrewPoint?.Y > point.Y)
            {
                return Direction.Down;
            }
            else if (point.PrewPoint?.X == point.X && point.PrewPoint?.Y < point.Y)
            {
                return Direction.Up;
            }

            return null;
        }
    }
}
