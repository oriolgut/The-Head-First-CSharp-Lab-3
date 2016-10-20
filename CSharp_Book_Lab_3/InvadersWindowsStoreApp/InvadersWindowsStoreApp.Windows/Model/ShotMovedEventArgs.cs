using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvadersWindowsStoreApp.Model
{
    class ShotMovedEventArgs : EventArgs
    {
        public ShotMovedEventArgs(Shot shot, bool disappeared)
        {
            Shot = shot;
            IsDisappeared = disappeared;
        }

        public Shot Shot { get; private set; }
        public bool IsDisappeared { get; private set; }
    }
}
