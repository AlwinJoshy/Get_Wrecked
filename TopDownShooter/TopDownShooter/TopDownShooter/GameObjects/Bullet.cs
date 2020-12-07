using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;
using SFML.Audio;
using SFML.System;


namespace TopDownShooter
{
    public class Bullet : Ammo
    {
        public Bullet(int damage, int bulletId, Texture tex)
        {
            spriteRend = new SpriteRendered(tex, 1, true);
            FloatRect offset = spriteRend.sprite.GetGlobalBounds();
            spriteRend.transform = this.transform;
            spriteRend.sprite.Origin = new Vector2f(offset.Width / 2, offset.Height / 2);
            spriteRend.moving = true;
            this.damage = damage;
            this.transform.objectID = bulletId;
            rb.obj = this;
            
        }

        public void DoOnTrigger(ref int otherObjId)
        {
            //   Console.WriteLine("Hit the enemy");
            RandomSceneMaker.bulletBlast(this.transform.objectID, this.transform.position);
            rb.active = false;
            spriteRend.draw = false;
        }
    }
}
