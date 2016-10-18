using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Invaders.Model
{
    class InvaderModel
    {
        public const int MAXIMUM_PLAYER_SHOTS = 3;
        public const int INITIAL_STAR_COUNT = 50;

        private Player _player;
        private DateTime? _playerDied = null;
        private DateTime _lastUpdated = DateTime.MinValue;
        private readonly List<Invader> _invaders = new List<Invader>();
        private readonly List<Shot> _playerShots = new List<Shot>();
        private readonly List<Shot> _invaderShots = new List<Shot>();
        private readonly List<Point> _stars = new List<Point>();
        private readonly Random _random = new Random();
        private Direction _invaderDirection = Direction.Left;
        private bool _hasMovedDown = false;

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

        public void EndGame()
        {
            IsGameOver = true;
        }

        public void StartGame()
        {
            foreach(Invader invader in _invaders)
            {
                OnShipChanged(invader, true);
            }
            _invaders.Clear();

            foreach(Shot shot in _playerShots)
            {
                OnShotMoved(shot, true);
            }
            _playerShots.Clear();

            foreach(Point star in _stars)
            {
                OnStarChanged(star, true);
            }
            _stars.Clear();

            for (int i = 0; i < INITIAL_STAR_COUNT; i++)
            {
                AddAStar();
            }
            
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

            if (playerShots.Count() < MAXIMUM_PLAYER_SHOTS)
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
            OnShipChanged(_player, false);
        }

        public void Twinkle()
        {
            if ((_random.Next(2) == 0) && _stars.Count > INITIAL_STAR_COUNT * .75)
            {
                RemoveAStar();
            }
            else if (_stars.Count < INITIAL_STAR_COUNT * 1.5)
            {
                AddAStar();
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

        private void AddAStar()
        {
            Point point = new Point(_random.Next((int)playAreaSize.Width), _random.Next(20, (int)playAreaSize.Height) - 20);
            if (!_stars.Contains(point))
            {
                _stars.Add(point);
                OnStarChanged(point, false);
            }
        }

        public void Update()
        {

        }

        private void NextWave()
        {
            throw new NotImplementedException();
        }

        public event EventHandler<ShipChangedEventArgs> ShipChanged;

        protected void OnShipChanged(Ship shipUpdated, bool killed)
        {
            ShipChanged?.Invoke(this, new ShipChangedEventArgs(shipUpdated, killed));
        }

        public event EventHandler<ShotMovedEventArgs> ShotMoved;

        protected void OnShotMoved(Shot shot, bool disappeared)
        {
            ShotMoved?.Invoke(this, new ShotMovedEventArgs(shot, disappeared));
        }

        public event EventHandler<StarChangedEventArgs> StarChanged;

        protected void OnStarChanged(Point point, bool disappeared)
        {
            StarChanged?.Invoke(this, new StarChangedEventArgs(point, disappeared));
        }
    }
}
