using Invaders.Help;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Invaders.Model
{
    class Shot
    {

        private DateTime _lastMoved;

        public Shot(Point location, Direction direction)
        {
            Location = location;
            Direction = direction;

            _lastMoved = DateTime.Now;
        }

        public Point Location { get; private set; }
        public Direction Direction { get; private set; }
        public static Size ShotSize = new Size(2, 10);

        public void Move()
        {
            TimeSpan timeSinceLastMoved = DateTime.Now - _lastMoved;
            double distance = timeSinceLastMoved.Milliseconds * Constants.SHOT_PIXELS_PER_SECOND / 1000;
            if(Direction == Direction.Up)
            {
                distance *= -1;
            }
            Location = new Point(Location.X, Location.Y + distance);
            _lastMoved = DateTime.Now;
        }
    }
}
