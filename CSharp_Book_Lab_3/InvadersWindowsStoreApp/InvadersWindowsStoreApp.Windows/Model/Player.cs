using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace InvadersWindowsStoreApp.Model
{
    public class Player : Ship
    {
        private const double PIXELS_TO_MOVE = 10;

        public Player() : base(new Point(playerSize.Width, InvaderModel.playAreaSize.Height - InvaderModel.playAreaSize.Height * 3), playerSize)
        {
            Location = new Point(Location.X, InvaderModel.playAreaSize.Height - playerSize.Height * 3);
        }

        public static readonly Size playerSize = new Size(25, 15);

        public override void Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    if (Location.X > playerSize.Width)
                    {
                        Location = new Point(Location.X - PIXELS_TO_MOVE, Location.Y);
                    }
                break;
                case Direction.Right:
                    if (Location.X < InvaderModel.playAreaSize.Width - playerSize.Width * 2)
                    {
                        Location = new Point(Location.X + PIXELS_TO_MOVE, Location.Y);
                    }
                break;
            }
        }
    }
}
