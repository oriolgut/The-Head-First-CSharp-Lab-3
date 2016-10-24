using InvadersWindowsStoreApp.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace InvadersWindowsStoreApp.Model
{
    public class Invader : Ship
    {
        public Invader(InvaderType invaderType, Point location, int score) : base(location, invaderSize)
        {
            InvaderType = invaderType;
            Score = score;
        }

        public override void Move(Direction invaderDirection)
        {
            switch (invaderDirection)
            {
                case Direction.Right:
                    Location = new Point(Location.X + Constants.HORIZONTAL_INTERVAL, Location.Y);
                    break;
                case Direction.Left:
                    Location = new Point(Location.X - Constants.HORIZONTAL_INTERVAL, Location.Y);
                    break;
                default:
                    Location = new Point(Location.X, Location.Y + Constants.VERTICAL_INTERVAL);
                    break;
            }
        }

        public static readonly Size invaderSize = new Size(15, 15);
        public InvaderType InvaderType { get; private set; }
        public int Score { get; private set; }
    }
}
