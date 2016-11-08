using InvadersWindowsStoreApp.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;

namespace InvadersWindowsStoreApp.Model
{
    public  class InvaderModel
    {
        private Player _player;
        private DateTime? _playerDied = null;
        private DateTime _lastUpdated = DateTime.MinValue;
        private readonly List<Invader> _invaders = new List<Invader>();
        private readonly List<Shot> _playerShots = new List<Shot>();
        private readonly List<Shot> _invaderShots = new List<Shot>();
        private readonly List<Point> _stars = new List<Point>();
        private readonly Random _random = new Random();
        private Direction _invaderDirection = Direction.Left;
        private bool _hasMovedDown;

        public readonly static Size playAreaSize = new Size(400, 300);

        public InvaderModel()
        {
            EndGame();
        }

        public int Score { get; private set; }
        public int Wave { get; private set; }
        public int Lives { get; private set; }

        public bool IsGameOver { get; private set; }
        public bool IsPlayerDead { get { return _playerDied.HasValue; } }

        public event EventHandler<StarChangedEventArgs> StarChanged;
        public event EventHandler<ShotMovedEventArgs> ShotMoved;
        public event EventHandler<ShipChangedEventArgs> ShipChanged;

        /// <summary>
        /// only for testing:
        public List<Point> Stars { get { return _stars; } }
        /// </summary>

        public void EndGame()
        {
            IsGameOver = true;
        }

        public void StartGame()
        {
            IsGameOver = false;

            foreach (Invader invader in _invaders)
            {
                OnShipChanged(invader, true);
            }
            _invaders.Clear();

            foreach (Shot shot in _playerShots)
            {
                OnShotMoved(shot, true);
            }
            _playerShots.Clear();

            foreach (Point star in _stars)
            {
                OnStarChanged(star, true);
            }
            _stars.Clear();

            for (int i = 0; i < Constants.INITIAL_STAR_COUNT; i++)
            {
                AddAStar();
           }

            Score = 0;
            Lives = 2;
            Wave = 0;
            _player = new Player();
            OnShipChanged(_player, false);
            NextWave();
        }

        public void FireShot()
        {
            if (IsGameOver)
            {
                return;
            }

            var playerShots =
                from Shot shot in _playerShots
                where shot.Direction == Direction.Up
                select shot;

            if (playerShots.Count() < Constants.MAXIMUM_PLAYER_SHOTS)
            {
                Point shotLocation = new Point(_player.Location.X + _player.Area.Width / 2, _player.Location.Y);
                Shot shot = new Shot(shotLocation, Direction.Up);
                _playerShots.Add(shot);
                OnShotMoved(shot, false);
            }
        }

        public void MovePlayer(Direction direction)
        {
            if (IsPlayerDead)
            {
                return;
            }
            _player.Move(direction);
            OnShipChanged(_player, killed: false);
        }

        public void Twinkle()
        {
            if ((_random.Next(2) == 0) && _stars.Count > Constants.INITIAL_STAR_COUNT * 0.75)
            {
                RemoveAStar();
            }
            else if (_stars.Count < Constants.INITIAL_STAR_COUNT * 1.5)
            {
                AddAStar();
            }
        }

        public void Update(bool paused)
        {
            if (paused)
            {
                return;
            }

            if (_invaders.Count == 0)
            {
                NextWave();
            }

            if (!IsPlayerDead)
            {
                MoveInvaders();
                MoveShots();
                ReturnFire();
                CheckForInvaderCollisions();
                CheckForPlayerCollisions();
            }
            else if (_playerDied.HasValue && (DateTime.Now - _playerDied > TimeSpan.FromSeconds(2.5)))
            {
                _playerDied = null;
                OnShipChanged(_player, false);
            }

            Twinkle();
        }

        protected void OnShipChanged(Ship shipUpdated, bool killed)
        {
            ShipChanged?.Invoke(this, new ShipChangedEventArgs(shipUpdated, killed));
        }

        protected void OnShotMoved(Shot shot, bool disappeared)
        {
            ShotMoved?.Invoke(this, new ShotMovedEventArgs(shot, disappeared));
        }

        protected void OnStarChanged(Point point, bool disappeared)
        {
            StarChanged?.Invoke(this, new StarChangedEventArgs(point, disappeared));
        }

        internal void UpdateAllShipsAndStars()
        {
            foreach (Shot shot in _playerShots)
            {
                OnShotMoved(shot, false);
            }

            foreach (Invader ship in _invaders)
            {
                OnShipChanged(ship, false);
            }

            OnShipChanged(_player, false);

            foreach (Point star in _stars)
            {
                OnStarChanged(star, false);
            }
        }

        private void RemoveAStar()
        {
            if (_stars.Count <= 0)
            {
                return;
            }
            int starIndex = _random.Next(_stars.Count);
            OnStarChanged(_stars[starIndex], true);
            _stars.RemoveAt(starIndex);
        }

        internal void AddAStar() 
        {
            Point point = new Point(_random.Next((int)playAreaSize.Width), _random.Next(20, (int)playAreaSize.Height) - 20);
            if (!_stars.Contains(point))
            {
                _stars.Add(point);
                OnStarChanged(point, false);
            }
        }

        private void MoveInvaders()
        {
            double millisecondsBetweenMovements = Math.Min(10 - Wave, 1) * (2 * _invaders.Count());

            if (DateTime.Now - _lastUpdated > TimeSpan.FromMilliseconds(millisecondsBetweenMovements))
            {
                _lastUpdated = DateTime.Now;
                if (!_hasMovedDown)
                {
                    InvadersTouchingBoundary();
                    _hasMovedDown = true;
                }
                else
                {
                    _hasMovedDown = false;
                    foreach (Invader invader in _invaders)
                    {
                        invader.Move(_invaderDirection);
                        OnShipChanged(invader, false);
                    }
                }
            }
        }

        private void InvadersTouchingBoundary()
        {
            var invadersTouchingLeftBoundary = from invader in _invaders where invader.Area.Left < Constants.HORIZONTAL_INTERVAL select invader;
            var invadersTouchingRightBoundary = from invader in _invaders where invader.Area.Right > playAreaSize.Width - (Constants.HORIZONTAL_INTERVAL * 2) select invader;

            if (invadersTouchingLeftBoundary.Count() > 0)
            {
                foreach (Invader invader in _invaders)
                {
                    invader.Move(Direction.Down);
                    OnShipChanged(invader, false);
                }
                _invaderDirection = Direction.Right;
            }
            else if (invadersTouchingRightBoundary.Count() > 0)
            {
                foreach (Invader invader in _invaders)
                {
                    invader.Move(Direction.Down);
                    OnShipChanged(invader, false);
                }
                _invaderDirection = Direction.Left;
            }
        }

        private void MoveShots()
        {
            foreach (Shot shot in _playerShots)
            {
                shot.Move();
                OnShotMoved(shot, false);
            }

            var outOfBounds =
                from shot in _playerShots
                where (shot.Location.Y < 10 || shot.Location.Y > playAreaSize.Height - 10)
                select shot;

            foreach (Shot shot in outOfBounds.ToList())
            {
                _playerShots.Remove(shot);
                OnShotMoved(shot, true);
            }
        }

        private void ReturnFire()
        {
            if (_invaderShots.Count >= Wave + 1)
            {
                return;
            }

            if (_random.Next(10) < 10 - Wave)
            {
                return;
            }

            var result =
                from invader in _invaders
                group invader by invader.Area.X into invaderGroup
                orderby invaderGroup.Key descending
                select invaderGroup;

            var randomGroup = result.ElementAt(_random.Next(result.Count()));
            var bottomInvader = randomGroup.Last();

            Point shotLocation = new Point(bottomInvader.Area.X + bottomInvader.Area.Width / 2, bottomInvader.Area.Bottom + 2);
            Shot invaderShot = new Shot(shotLocation, Direction.Down);
            _playerShots.Add(invaderShot);
            OnShotMoved(invaderShot, false);
        }

        private void CheckForInvaderCollisions()
        {
            List<Shot> hittedShots = new List<Shot>();
            List<Invader> killedInvaders = new List<Invader>();

            foreach (Shot shot in _playerShots)
            {
                foreach (Invader invader in _invaders)
                {
                    if (invader.Area.Contains(shot.Location) && shot.Direction == Direction.Up)
                    {
                        killedInvaders.Add(invader);
                        hittedShots.Add(shot);
                    }
                }
            }

            foreach (Invader killedInvader in killedInvaders)
            {
                Score += killedInvader.Score;
                _invaders.Remove(killedInvader);
                OnShipChanged(killedInvader, true);
            }

            foreach (Shot hittedShot in hittedShots)
            {
                _playerShots.Remove(hittedShot);
                OnShotMoved(hittedShot, true);
            }
        }

        private void CheckForPlayerCollisions()
        {
            bool hasToRemoveAllShots = false;
            InvadersReachedTheBottom();

            var shotsHit = from shot in _playerShots where shot.Direction == Direction.Down && _player.Area.Contains(shot.Location) select shot;
            if (shotsHit.Count() > 0)
            {
                Lives--;
                if (Lives >= 0)
                {
                    _playerDied = DateTime.Now;
                    OnShipChanged(_player, true);
                    hasToRemoveAllShots = true;
                }
                else
                {
                    EndGame();
                }
            }

            if (hasToRemoveAllShots)
            {
                foreach (Shot shot in _playerShots.ToList())
                {
                    _playerShots.Remove(shot);
                    OnShotMoved(shot, true);
                }
            }
        }

        private void InvadersReachedTheBottom()
        {
            var result = from invader in _invaders where invader.Area.Bottom > _player.Area.Top - _player.Size.Height select invader;
            if (result.Count() > 0)
            {
                EndGame();
            }
        }

        private void NextWave()
        {
            Wave++;
            _invaders.Clear();

            for (int row = 0; row <= 5; row++)
            {
                for (int column = 0; column < 11; column++)
                {
                    Point location = new Point(column * Invader.invaderSize.Width * 1.4, row * Invader.invaderSize.Height * 1.4);
                    Invader invader;
                    switch (row)
                    {
                        case 0:
                            invader = new Invader(InvaderType.Spaceship, location, 50);
                            break;
                        case 1:
                            invader = new Invader(InvaderType.Bug, location, 40);
                            break;
                        case 2:
                            invader = new Invader(InvaderType.Saucer, location, 30);
                            break;
                        case 3:
                            invader = new Invader(InvaderType.Satellite, location, 20);
                            break;
                        case 4:
                        case 5:
                            invader = new Invader(InvaderType.Star, location, 10);
                            break;
                        default:
                            throw new NotSupportedException(row.ToString());
                    }
                    _invaders.Add(invader);
                    OnShipChanged(invader, false);
                }
            }
        }
    }
}
