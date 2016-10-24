using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace InvadersWindowsStoreApp.Model
{
    public class StarChangedEventArgs : EventArgs
    {
        public StarChangedEventArgs(Point point, bool disappeard)
        {
            Point = point;
            isDisappeared = disappeard;
        }

        public Point Point { get; private set; }
        public bool isDisappeared { get; private set; }
    }
}
