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
    public class Ammo
    {
        public int damage;
        public float lifeSpan = 3;
        public int parentId;
        public Rigidbody rb;
        public AABBBox collider;
        public Transform transform;
        public SpriteRendered spriteRend;

        public Ammo()
        {
            transform = new Transform();
            collider = new AABBBox { min = new Vector2f(-4, -4), dimension = new Vector2f(8, 8)};
            rb = new Rigidbody(transform, ref collider);
            rb.active = false;
            rb.isTrigger = true;
            rb.drag = 1;
            rb.inversMass = 10;
            rb.elasticity = 0.2f;
            rb.isStatic = false;
            rb.isKinamatic = true;
        }
    }
   
}
