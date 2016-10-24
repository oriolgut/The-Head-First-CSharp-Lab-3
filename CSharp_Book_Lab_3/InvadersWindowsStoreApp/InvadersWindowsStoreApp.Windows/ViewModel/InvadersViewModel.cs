using InvadersWindowsStoreApp.Model;
using InvadersWindowsStoreApp.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Xaml;

namespace InvadersWindowsStoreApp.ViewModel
{
    public class InvadersViewModel : INotifyPropertyChanged
    {
        private readonly ObservableCollection<FrameworkElement> _sprites = new ObservableCollection<FrameworkElement>();
        private readonly ObservableCollection<object> _lives = new ObservableCollection<object>();
        private readonly InvaderModel _model = new InvaderModel();
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private FrameworkElement _playerControl = null;
        private bool _isPlayerFlashing = false;
        private bool _isLastPaused = true;
        private DateTime? _leftAction = null;
        private DateTime? _rightAction = null;
        private readonly Dictionary<Invader, FrameworkElement> _invaders = new Dictionary<Invader, FrameworkElement>();
        private readonly Dictionary<FrameworkElement, DateTime> _shotInvaders = new Dictionary<FrameworkElement, DateTime>();
        private readonly Dictionary<Shot, FrameworkElement> _shots = new Dictionary<Shot, FrameworkElement>();
        private readonly Dictionary<Point, FrameworkElement> _stars = new Dictionary<Point, FrameworkElement>();
        private readonly List<FrameworkElement> _scanLines = new List<FrameworkElement>();

        public InvadersViewModel()
        {
            Scale = 1;
            _model.ShipChanged += ModelShipChangedEventHandler;
            _model.ShotMoved += ModelShotMovedEventHandler;
            _model.StarChanged += ModelStarChangedEventHandler;

            _timer.Interval = TimeSpan.FromMilliseconds(100);
            _timer.Tick += TimerTickEventHandler;

            _model.EndGame();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public INotifyCollectionChanged Sprites { get { return _sprites; } }
        public bool IsGameOver { get { return _model.IsGameOver; } }
        public INotifyCollectionChanged Lives { get { return _lives; } }
        public bool IsPaused { get; set; }
        public static double Scale { get; private set; }
        public int Score { get; private set; }

        public Size PlayAreaSize
        {
            set
            {
                Scale = value.Width / 405;
                _model.UpdateAllShipsAndStars();
                RecreateScanLines();
            }
        }
        
        public void StartGame()
        {
            IsPaused = false;
            foreach (var invader in _invaders.Values)
            {
                _sprites.Remove(invader);
            }

            foreach (var shot in _shots.Values)
            {
                _sprites.Remove(shot);
            }

            _model.StartGame();
            OnPropertyChanged(nameof(IsGameOver));
            _timer.Start();
        }



        internal void KeyDown(VirtualKey virtualKey)
        {
            if (virtualKey == VirtualKey.Space)
            {
                _model.FireShot();
            }

            if (virtualKey == VirtualKey.Left)
            {
                _leftAction = DateTime.Now;
            }

            if (virtualKey == VirtualKey.Right)
            {
                _rightAction = DateTime.Now;
            }
        }

        internal void KeyUp(VirtualKey virtualKey)
        {
            if (virtualKey == VirtualKey.Left)
            {
                _leftAction = null;
            }

            if (virtualKey == VirtualKey.Right)
            {
                _rightAction = null;
            }
        }

        internal void LeftGestureStarted()
        {
            _leftAction = DateTime.Now;
        }

        internal void LeftGestureCompleted()
        {
            _leftAction = null;
        }

        internal void RightGestureStarted()
        {
            _rightAction = DateTime.Now;
        }

        internal void RightGestureCompleted()
        {
            _rightAction = null;
        }

        internal void Tapped()
        {
            _model.FireShot();
        }

        private void RecreateScanLines()
        {
            foreach (FrameworkElement scanLine in _scanLines)
            {
                if (_sprites.Contains(scanLine))
                {
                    _sprites.Remove(scanLine);
                }
            }
            _scanLines.Clear();
            for (int y = 0; y < 300; y += 2)
            {
                FrameworkElement scanLine = InvadersHelper.ScanLineFactory(y, 400, Scale);
                _scanLines.Add(scanLine);
                _sprites.Add(scanLine);
            }
        }

        private void ModelShipChangedEventHandler(object sender, ShipChangedEventArgs e)
        {
            if (!e.IsKilled)
            {
                if (e.ShipUpdated is Invader)
                {
                    Invader invader = e.ShipUpdated as Invader;
                    CreateOrMoveInvader(invader);
                }
                else if (e.ShipUpdated is Player)
                {
                    Player player = e.ShipUpdated as Player;
                    StopPlayerFromFlashing();
                    CreatePlayer(player);
                }
            }
            else
            {
                if (e.ShipUpdated is Invader)
                {
                    Invader invader = e.ShipUpdated as Invader;
                    if (!_invaders.ContainsKey(invader))
                    {
                        return;
                    }
                    AnimatedImage invaderControl = _invaders[invader] as AnimatedImage;
                    if (invaderControl != null)
                    {
                        invaderControl.InvaderShot();
                        _shotInvaders[invaderControl] = DateTime.Now;
                        _invaders.Remove(invader);
                    }
                }
                else if (e.ShipUpdated is Player)
                {
                    AnimatedImage control = _playerControl as AnimatedImage;
                    if (control != null)
                    {
                        control.StartFlashing();
                    }
                    _isPlayerFlashing = true;
                }
            }
        }

        private void ModelShotMovedEventHandler(object sender, ShotMovedEventArgs e)
        {
            if (!e.IsDisappeared)
            {
                if (!_shots.ContainsKey(e.Shot))
                {
                    FrameworkElement shotControl = InvadersHelper.ShotControlFactory(e.Shot, Scale);
                    _shots[e.Shot] = shotControl;
                    _sprites.Add(shotControl);
                }
                else
                {
                    FrameworkElement shotControl = _shots[e.Shot];
                    InvadersHelper.MoveElementOnCanvas(shotControl, e.Shot.Location.X * Scale, e.Shot.Location.Y * Scale);
                }
            }
            else
            {
                if (_shots.ContainsKey(e.Shot))
                {
                    FrameworkElement shotControl = _shots[e.Shot];
                    _sprites.Remove(shotControl);
                    _shots.Remove(e.Shot);
                }
            }
        }

        private void ModelStarChangedEventHandler(object sender, StarChangedEventArgs e)
        {
            if (e.isDisappeared && _stars.ContainsKey(e.Point))
            {
                FrameworkElement starControl = _stars[e.Point];
                _sprites.Remove(starControl);
            }
            else
            {
                if (!_stars.ContainsKey(e.Point))
                {
                    FrameworkElement starControl = InvadersHelper.StarControlFactory(e.Point, Scale);
                    _stars[e.Point] = starControl;
                    _sprites.Add(starControl);
                }
                else
                {
                    FrameworkElement starControl = _stars[e.Point];
                    InvadersHelper.SetCanvasLocation(starControl, e.Point.X * Scale, e.Point.Y * Scale);
                }
            }
        }

        private void MovePlayer()
        {
            if (!IsPaused)
            {
                if (_leftAction.HasValue && _rightAction.HasValue)
                {
                    _model.MovePlayer(_leftAction > _rightAction ? Direction.Left : Direction.Right);
                }

                if (_leftAction.HasValue)
                {
                    _model.MovePlayer(Direction.Left);
                }

                if (_rightAction.HasValue)
                {
                    _model.MovePlayer(Direction.Right);
                }
            }
        }

        private void TimerTickEventHandler(object sender, object e)
        {
            if (_isLastPaused != IsPaused)
            {
                OnPropertyChanged(nameof(IsPaused));
            }

            MovePlayer();
            _model.Update(IsPaused);

            if (_model.Score != Score)
            {
                Score = _model.Score;
                OnPropertyChanged(nameof(Score));
            }

            if (_model.Lives >= 0)
            {
                while (_lives.Count > _model.Lives)
                {
                    _lives.RemoveAt(0);
                }
                while (_lives.Count < _model.Lives)
                {
                    _lives.Add(new object());
                }
            }

            foreach (FrameworkElement control in _shotInvaders.Keys.ToList())
            {
                DateTime elapsed = _shotInvaders[control];
                if (DateTime.Now - elapsed > TimeSpan.FromSeconds(0.5))
                {
                    _sprites.Remove(control);
                    _shotInvaders.Remove(control);
                }
            }

            if (_model.IsGameOver)
            {
                OnPropertyChanged(nameof(IsGameOver));
                _timer.Stop();
            }
        }

        private void StopPlayerFromFlashing()
        {
            if (_isPlayerFlashing)
            {
                _isPlayerFlashing = false;
                AnimatedImage control = _playerControl as AnimatedImage;
                if (control != null)
                {
                    control.StopFlashing();
                }
            }
        }

        private void CreatePlayer(Player player)
        {
            if (_playerControl == null)
            {
                _playerControl = InvadersHelper.PlayerControlFactory(player, Scale);
                _sprites.Add(_playerControl);
            }
            else
            {
                InvadersHelper.MoveElementOnCanvas(_playerControl, player.Location.X * Scale, player.Location.Y * Scale);
                InvadersHelper.ResizeElement(_playerControl, player.Size.Width * Scale, player.Size.Height * Scale);
            }
        }

        private void CreateOrMoveInvader(Invader invader)
        {
            if (!_invaders.ContainsKey(invader))
            {
                FrameworkElement invaderControl = InvadersHelper.InvaderControlFactory(invader, Scale);
                _invaders[invader] = invaderControl;
                _sprites.Add(invaderControl);
            }
            else
            {
                FrameworkElement invaderControl = _invaders[invader];
                InvadersHelper.MoveElementOnCanvas(invaderControl, invader.Location.X * Scale, invader.Location.Y * Scale);
                InvadersHelper.ResizeElement(invaderControl, invader.Size.Width * Scale, invader.Size.Height * Scale);
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
