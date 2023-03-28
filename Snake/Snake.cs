using Microsoft.AspNetCore.Mvc.Diagnostics;
using System.Drawing;
using System.Numerics;
using static Snake.Snake;

namespace Snake
{
    public partial class Snake
    {
        private int maxDepth { get; set; } = 1;

        private Coordinate Mid { get; set; }

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
            if (!game.You.Alive) return Direction.Left;

            Mid = Mid ?? new Coordinate() { X = (int)(game.Board.Width / 2), Y = (int)(game.Board.Height / 2) };

            Direction? direction = null;

            game.Board.Snakes.FirstOrDefault(s => s.Id == game.You.Id).Head = null;

            if (game.Board.Food.Length == 0)
            {
                //TODO
                if (CheckCollision(game.Board, Mid.X, Mid.Y)) 
                    game.Board.Food = new Coordinate[] { Mid };
            }

            direction = InitBFS(game.Board, game.You.Head);
                        
            return direction ?? RandMove(game.Board, game.You.Head);
        }

        private Direction? InitBFS(Board board, Coordinate point, int depth = 0, bool mid = false)
        {
            Queue<Point> queue = new();
            Dictionary<Vector2, bool> check = new();

            Direction? direction = null;

            queue.Enqueue(new Point(point.X, point.Y));

            List<Point> firsPoints = new List<Point>();

            do
            {
                direction = BFS(queue, check, board, depth, mid);

                if (direction != null)
                    break;

                if (firsPoints.Count == 0)
                    firsPoints.AddRange(queue);
            } while (queue.Count() > 0);

            /*if (firsPoints.Count == 2 && direction == null)
                direction = BetterDirection(board, point, check);*/

            return direction;
        }

        private Direction BetterDirection(Board board, Coordinate point, Dictionary<Vector2, bool> check)
        {
            Direction? direction = null;

            int dir1 = 0, dir2 = 0;

            if (check.TryGetValue(new Vector2(point.X - 1, point.Y), out _))
                dir1 = -1;
            if (check.TryGetValue(new Vector2(point.X + 1, point.Y), out _))
                dir1 = +1;
            if (check.TryGetValue(new Vector2(point.X, point.Y - 1), out _))
                dir2 = -1;
            if (check.TryGetValue(new Vector2(point.X, point.Y + 1), out _))
                dir2 = +1;

            check.Remove(new Vector2(point.X, point.Y));

            int dir1count = 0, dir2count = 0;

            CountOFPoints(board, check, point.X + dir1, point.Y, ref dir1count);
            CountOFPoints(board, check, point.X, point.Y + dir2, ref dir2count);

            if (dir1count > dir2count)
                return dir1 != -1 ? Direction.Right : Direction.Left;
            else 
                return dir2 != -1 ? Direction.Up : Direction.Down;
        }

        private void CountOFPoints(Board board, Dictionary<Vector2, bool> check, int x, int y, ref int count)
        {
            if (check[new Vector2(x, y)]) { count++; }

            for (int _x = -1; _x < 2; _x += 2)
                if (check.TryGetValue(new Vector2(x + _x, y), out _) && CheckBoundries(board, _x + x, y))
                    CountOFPoints(board, check, _x + x, y, ref count);
            for (int _y = -1; _y < 2; _y += 2)
                if (check.TryGetValue(new Vector2(x, y + _y), out _) && CheckBoundries(board, x, _y + y))
                    CountOFPoints(board, check, x, _y + y, ref count);
        }

        private Direction? BFS(Queue<Point> queue, Dictionary<Vector2, bool> check, Board board, int depth = 0, bool mid = false) 
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
                if (depth < maxDepth && maxDepth > 0 && board.Food[0] != Mid)
                {
                    var copy = new Coordinate[board.Food.Count() + 1];
                    if (board.Food.Count() == depth + 1 && !mid && CheckCollision(board, Mid.X, Mid.Y))
                    {
                        for (int i = 0; i < board.Food.Count(); i++)
                        {
                            copy[i] = board.Food[i];
                        }

                        copy[board.Food.Count()] = Mid;

                        mid = true;
                    }
                    else
                    {
                        copy = board.Food;
                    }
                    var _boa = new Board() { Food = copy, Height = board.Height, Width = board.Width, Obstacles = new Coordinate[board.Obstacles.Length+1], Snakes = board.Snakes };

                    _boa.Food = _boa.Food.Where(f => f.X != point.X || f.Y != point.Y).ToArray();
                    for (int i = 0; i < board.Obstacles.Count(); i++)
                    {
                        _boa.Obstacles[i] = board.Obstacles[i];
                    }
                    if (point.PrewPoint != null)
                        _boa.Obstacles[board.Obstacles.Length] = new Coordinate() { X = point.PrewPoint.X, Y = point.PrewPoint.Y };

                    Direction? dir = InitBFS(_boa, new Coordinate() { X = point.X, Y = point.Y }, depth+1, mid);

                    if (dir == null) { 
                        return null; 
                    }
                }
                return point.GetDirection();
            }
            else
            {
                AddToQueue(queue, point, board);
            }

            return direction;
        }

        private Direction RandMove(Board board, Coordinate point)
        {
            Direction? direction = null;

            int x, y;

            x = point.X - 1;
            y = point.Y;

            if (CheckCollision(board, x, y))
                return Direction.Left;
            
            x = point.X;
            y = point.Y - 1;

            if (CheckCollision(board, x, y))
                return Direction.Down;

            x = point.X + 1;
            y = point.Y;

            if (CheckCollision(board, x, y))
                return Direction.Right;

            x = point.X;
            y = point.Y + 1;

            if (CheckCollision(board, x, y))
                return Direction.Up;

            return direction ?? Direction.Up;
        }

        private void AddToQueue(Queue<Point> queue, Point point, Board board)
        {
            for (int x = -1; x < 2; x += 2)
                if (CheckCollision(board, point.X + x, point.Y))
                    queue.Enqueue(new Point(point.X + x, point.Y, point));
            for (int y = -1; y < 2; y += 2)
                if (CheckCollision(board, point.X, point.Y + y))
                    queue.Enqueue(new Point(point.X, point.Y + y, point));
        }

        private bool CheckBoundries(Board board, int x, int y) 
        {
            if (x < 0) { return false; }
            if (y < 0) { return false; }
            if (x > board.Width - 1) { return false; }
            if (y > board.Height - 1) { return false; }

            return true;
        }

        private bool CheckCollision(Board board, int x, int y)
        {
            if (!CheckBoundries(board, x, y)) return false; 

            foreach (var arr in new List<Coordinate[]>(board.Snakes.Where(s => s.Alive).Select(s => s.Body)) { board.Obstacles })
            {
                foreach (var ob in arr ?? new Coordinate[0])
                {
                    if (ob.X == x && ob.Y == y)
                    {
                        return false;
                    }
                }
            }

            foreach (var head in new List<Coordinate>(board.Snakes.Where(s => s.Head != null && s.Alive).Select(s => s.Head)) { })
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

    }
}
