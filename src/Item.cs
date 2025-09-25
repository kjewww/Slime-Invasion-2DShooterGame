using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShooterGame2D
{
    public abstract class Item : IDrawable
    {
        public Size size = new Size(48, 48);
        public PointF Position { get; set; }
        public Collider collider;

        public Image image;

        public Item(PointF pos)
        {
            this.Position = pos;
            collider = new Collider
            {
                size = this.size,
                Position = this.Position
            };
        }

        public abstract void GiveEffect(Player player);

        public virtual void Draw(Graphics g) 
        {
            g.FillRectangle(Brushes.Green, new Rectangle((int)Position.X, (int)Position.Y, size.Width, size.Height));
        }
    }
}
