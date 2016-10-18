using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Invaders.Model
{
    class Invader : Ship
    {
        public Invader(Point location, Size size) : base(location, size)
        {
        }

        public override void Move(Direction direction)
        {
            throw new NotImplementedException();
        }
    }
}
