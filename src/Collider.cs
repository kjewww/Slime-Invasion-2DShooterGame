using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShooterGame2D
{
    public class Collider
    {
        public PointF Position { get; set; }
        public Size size { get; set; }
        public RectangleF Bounds => new RectangleF(Position.X + 10, Position.Y + 10, size.Width - 15, size.Height - 20);

        public bool Intersects(Collider other)
        {
            return this.Bounds.IntersectsWith(other.Bounds);
        }
    }
}
