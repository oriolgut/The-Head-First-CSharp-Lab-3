using System;
using Windows.Foundation;

namespace Invaders.Model
{
    class StarChangedEventArgs : EventArgs
    {
        public StarChangedEventArgs(Point point, bool disappeard)
        {
            Point = point;
            Disappeared = disappeard;
        }

        public Point Point { get; private set; }
        public bool Disappeared { get; private set; }
    }
}
