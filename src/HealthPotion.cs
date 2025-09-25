using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShooterGame2D
{
    public class HealthPotion : Item
    {
        public int Health = 10;
        public HealthPotion(PointF pos) : base(pos)
        {
            image = Resource.PotionHP;
        }

        public override void GiveEffect(Player player)
        {
            player.Health += Health;
            if (player.Health > 100)
            {
                player.Health = 100;
            }
        }

        public override void Draw(Graphics g)
        {
            // debug
            //g.FillRectangle(Brushes.Green, new Rectangle((int)Position.X, (int)Position.Y, size.Width, size.Height));

            g.DrawImage(
                image,
                new Rectangle((int)Position.X, (int)Position.Y, size.Width, size.Height),
                new Rectangle(0, 0, image.Width, image.Height),
                GraphicsUnit.Pixel
            );
        }
    }
}
