using Microsoft.AspNetCore.Mvc.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
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

            direction = BFS(queue, check, board, depth, mid);

            if (depth == 0)
            {
                var worst = WorstDirection(board, point);
                if (worst != null)
                    queue = new(queue.Where(p => worst.FirstOrDefault(w => w.X == p.X && w.Y == p.Y) == null));
            }

            while (queue.Count() > 0)
            {
                direction = BFS(queue, check, board, depth, mid);

                if (direction != null)
                    break;
            } 

            return direction;
        }

        private List<Coordinate>? WorstDirection(Board board, Coordinate point)
        {
            Direction? direction = null;

            Queue<Point> queue = new();
            Dictionary<Vector2, bool> check = new();

            queue.Enqueue(new Point(point.X, point.Y));

            Board _board = new Board() { Height = board.Height, Width = board.Height, Obstacles = board.Obstacles, Snakes = board.Snakes };

            while (queue.Count() > 0)
            {
                BFS(queue, check, _board, 0, false);

                if (direction != null)
                    break;
            }

            Dictionary<Coordinate, int> dirs = new();

            if (check.ContainsKey(new Vector2(point.X - 1, point.Y)))
                dirs.Add(new Coordinate() { X = -1, Y = 0 }, 0);
            if (check.ContainsKey(new Vector2(point.X + 1, point.Y)))
                dirs.Add(new Coordinate() { X = 1, Y = 0 }, 0);
            if (check.ContainsKey(new Vector2(point.X, point.Y - 1)))
                dirs.Add(new Coordinate() { X = 0, Y = -1 }, 0);
            if (check.ContainsKey(new Vector2(point.X, point.Y + 1)))
                dirs.Add(new Coordinate() { X = 0, Y = 1 }, 0);

            check.Remove(new Vector2(point.X, point.Y));

            if (check.Count == 0) return null;
            
            foreach (var dir in dirs)
            {
                var dircount = 0;
                CountOFPoints(board, check, point.X + dir.Key.X, point.Y + dir.Key.Y, ref dircount, new());
                dirs[dir.Key] = dircount;
            }

            var mins = new List<Coordinate>();

            if (dirs.Where(m => m.Value == dirs.MinBy(d => d.Value).Value).Count() == dirs.Count() || dirs.Count() <= 1)
                return null;

            foreach (var dir in dirs.Where(d => d.Value == dirs.MinBy(d => d.Value).Value).Select(d => d.Key))
                mins.Add(new Coordinate() { X = point.X + dir.X, Y = point.Y + dir.Y});

            return mins;
            /*
            if (a.X != 0)
                if (a.X == -1)
                    return Direction.Left;
                else return Direction.Right;
            else
                if (a.Y == -1)
                return Direction.Down;
            else return Direction.Up;
            //*/
        }

        private void CountOFPoints(Board board, Dictionary<Vector2, bool> freePoints, int x, int y, ref int count, Dictionary<Vector2, bool> check)
        {
            if (check.ContainsKey(new Vector2(x, y))) return;

            check.Add(new Vector2(x,y), true);

            if (freePoints.ContainsKey(new Vector2(x, y))) { count++; }

            for (int _x = -1; _x < 2; _x += 2)
                if (freePoints.ContainsKey(new Vector2(x + _x, y)) && CheckBoundries(board, _x + x, y))
                    CountOFPoints(board, freePoints, x + _x, y, ref count, check);
            for (int _y = -1; _y < 2; _y += 2)
                if (freePoints.ContainsKey(new Vector2(x, y + _y)) && CheckBoundries(board, x, _y + y))
                    CountOFPoints(board, freePoints, x, y + _y, ref count, check);
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
            if (_food != null && _food != Mid) 
            {
                if (depth < maxDepth && maxDepth > 0 && board.Food[0] != Mid)
                {
                    var copy = new Coordinate[board.Food.Count() + 1];
                    if (board.Food.Length == depth + 1 && !mid && CheckCollision(board, Mid.X, Mid.Y))
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

        private Direction RandMove(Board board, Coordinate point, bool wOffset = true)
        {
            Direction? direction = null;

            var worsts = WorstDirection(board, point);

            int x, y;

            x = point.X - 1;
            y = point.Y;

            if (CheckCollision(board, x, y, wOffset) && worsts?.FirstOrDefault(w => w.X == x && w.Y == y) == null)
                return Direction.Left;
            
            x = point.X;
            y = point.Y - 1;

            if (CheckCollision(board, x, y, wOffset) && worsts?.FirstOrDefault(w => w.X == x && w.Y == y) == null)
                return Direction.Down;

            x = point.X + 1;
            y = point.Y;

            if (CheckCollision(board, x, y, wOffset) && worsts?.FirstOrDefault(w => w.X == x && w.Y == y) == null)
                return Direction.Right;

            x = point.X;
            y = point.Y + 1;

            if (CheckCollision(board, x, y, wOffset) && worsts?.FirstOrDefault(w => w.X == x && w.Y == y) == null)
                return Direction.Up;

            if (direction == null && wOffset)
                return RandMove(board, point, false);

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

        private bool CheckBoundries(Board board, int x, int y, bool rand = false) 
        {
            int offset = rand ? 1 : 0;

            if (x < 0 + offset) { return false; }
            if (y < 0 + offset) { return false; }
            if (x > board.Width - (1+offset)) { return false; }
            if (y > board.Height - (1+offset)) { return false; }

            return true;
        }

        private bool CheckCollision(Board board, int x, int y, bool rand = false)
        {
            if (!CheckBoundries(board, x, y, rand)) return false; 

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
    }
}
