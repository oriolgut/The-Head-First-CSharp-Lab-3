using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvadersWindowsStoreApp.Model
{
    public class ShipChangedEventArgs : EventArgs
    {
        public ShipChangedEventArgs(Ship shipUpdated, bool killed)
        {
            ShipUpdated = shipUpdated;
            IsKilled = killed;
        }

        public Ship ShipUpdated { get; private set; }
        public bool IsKilled { get; private set; }
    }
}
