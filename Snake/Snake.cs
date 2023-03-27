using Microsoft.AspNetCore.Mvc.Diagnostics;
using System.Drawing;
using System.Numerics;
using static Snake.Snake;

namespace Snake
{
    public partial class Snake
    {
        private int maxDepth { get; set; }

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
            maxDepth = 2;
        }

        // dostane data hry, vraci smer pohybu hada
        public Direction Move(Game game)
        {
            Direction? direction = null;

            Queue<Point> queue = new();
            Dictionary<Vector2, bool> check = new();

            game.Board.Snakes.FirstOrDefault(s => s.Id == game.You.Id).Head = null;

            if (game.Board.Food.Length == 0)
            {
                //TODO
                game.Board.Food = new Coordinate[] { new Coordinate() { X = (int)(game.Board.Width / 2), Y = (int)(game.Board.Height / 2) } };
            }

            queue.Enqueue(new Point(game.You.Head.X, game.You.Head.Y));

            do
            {
                direction = BFS(queue, check, game.Board);

                if (direction != null)
                    break;
            } while (queue.Count() > 0);
                        
            return direction ?? RandMove(game.Board, game.You.Head);
        }

        private Direction RandMove(Board board, Coordinate point)
        {
            Direction? direction = null;

            int x, y;

            if (point.X > 0)
            {
                x = point.X - 1;
                y = point.Y;

                if (CheckCollision(board, x, y))
                    direction = Direction.Left;
            }

            else if (point.Y > 0)
            {
                x = point.X;
                y = point.Y - 1;

                if (CheckCollision(board, x, y))
                    direction = Direction.Up;
            }

            else if (point.X < board.Width - 1)
            {
                x = point.X + 1;
                y = point.Y;

                if (CheckCollision(board, x, y))
                    direction = Direction.Left;
            }

            else if (point.Y < board.Height - 1)
            {
                x = point.X;
                y = point.Y + 1;

                if (CheckCollision(board, x, y))
                    direction = Direction.Down;
            }

            return direction ?? Direction.Up;
        }

        private Direction? BFS(Queue<Point> queue, Dictionary<Vector2, bool> check, Board board, int depth = 0) 
        {
            Direction? direction = null;

            Point point = queue.Dequeue();

            if (check.TryGetValue(new Vector2(point.X, point.Y), out _)) return direction;

            check.Add(new Vector2(point.X, point.Y), true);

            Coordinate? _food = null;

            foreach (var food in board.Food ?? new Coordinate[0])
            {
                if (food.X == point.X && food.Y == point.Y)
                {
                    _food = food;
                    break;
                }
            }
            if (_food != null) 
            {
                //if (depth <= maxDepth)

                return point.GetDirection();
            }
            else
            {
                AddToQueue(queue, point, board);
            }

            return direction;
        }

        /*
        private void AddToQueue(Queue<Point> queue, Point point, Board board)
        {
            int x, y;

            if (point.X > 0)
            {
                x = point.X - 1;
                y = point.Y;

                if (CheckCollision(board, x, y))
                    queue.Enqueue(new (x, y, point));
            }

            if (point.Y > 0)
            {
                x = point.X;
                y = point.Y - 1;

                if (CheckCollision(board, x, y))
                    queue.Enqueue(new(x, y, point));
            }

            if (point.X < board.Width - 1)
            {
                x = point.X + 1;
                y = point.Y;

                if (CheckCollision(board, x, y))
                    queue.Enqueue(new(x, y, point));
            }

            if (point.Y < board.Height - 1)
            {
                x = point.X;
                y = point.Y + 1;

                if (CheckCollision(board, x, y))
                    queue.Enqueue(new(x, y, point));
            }
        }

        private bool CheckCollision(Board board, int x, int y)
        {
            foreach (var arr in new List<Coordinate[]>(board.Snakes.Select(s => s.Body)) { board.Obstacles })
            {
                foreach (var ob in arr ?? new Coordinate[0])
                {
                    if (ob.X == x && ob.Y == y)
                    {
                        return false;
                    }
                }
            }

            foreach (var head in new List<Coordinate>(board.Snakes.Where(s => s.Head != null).Select(s => s.Head)) { })
            {
                if (head.X - 1 == x && head.Y == y) { return false; }
                if (head.X + 1 == x && head.Y == y) { return false; }
                if (head.X == x && head.Y - 1 == y) { return false; }
                if (head.X == x && head.Y + 1 == y) { return false; }
                if (head.X == x && head.Y == y) { return false; }
            }

            return true;
        } //*/

        
        private void AddToQueue(Queue<Point> queue, Point point, Board board)
        {
            for (int x = -1; x < 2; x += 2)
                if (CheckCollision(board, point.X + x, point.Y))
                    queue.Enqueue(new Point(point.X + x, point.Y, point));
            for (int y = -1; y < 2; y += 2)
                if (CheckCollision(board, point.X, point.Y + y))
                    queue.Enqueue(new Point(point.X, point.Y + y, point));
        }

        private bool CheckCollision(Board board, int x, int y)
        {
            if (x < 0) { return false; }
            if (y < 0) { return false; }
            if (x > board.Width - 1) { return false; }
            if (y > board.Height - 1) { return false; }

            foreach (var arr in new List<Coordinate[]>(board.Snakes.Select(s => s.Body)) { board.Obstacles })
            {
                foreach (var ob in arr ?? new Coordinate[0])
                {
                    if (ob.X == x && ob.Y == y)
                    {
                        return false;
                    }
                }
            }

            foreach (var head in new List<Coordinate>(board.Snakes.Where(s => s.Head != null).Select(s => s.Head)) { })
            {
                if (head.X - 1 == x && head.Y == y) { return false; }
                if (head.X + 1 == x && head.Y == y) { return false; }
                if (head.X == x && head.Y - 1 == y) { return false; }
                if (head.X == x && head.Y + 1 == y) { return false; }
                if (head.X == x && head.Y == y) { return false; }
            }

            return true;
        }
        //*/
    }
}
