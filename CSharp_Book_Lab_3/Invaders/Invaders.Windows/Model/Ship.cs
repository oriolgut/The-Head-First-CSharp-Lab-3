using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Invaders.Model
{
    abstract class Ship
    {
        public Ship(Point location, Size size)
        {

        }

        public Point Location { get; protected set; }
        public Size Size { get; private set; }
        public Rect Area
        {
            get
            {
                return new Rect(Location, Size);
            }
        }

        public abstract void Move(Direction direction);
    }
}
