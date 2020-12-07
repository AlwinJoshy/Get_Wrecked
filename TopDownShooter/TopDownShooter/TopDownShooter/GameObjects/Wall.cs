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
    public class Wall
    {
        public SpriteRendered spriteRend;
        public Rigidbody rb;
        public AABBBox collider;
        public Transform transform;

        public Wall(Texture image, Vector2f setPos)
        {
            spriteRend = new SpriteRendered(image, 1, true);
            transform = new Transform();
            transform.position = setPos;
            transform.objectID = (int)Init.PhysicsObjectTypes.wall;
            spriteRend.sprite.Position = setPos;
            collider.min = new Vector2f(0, 0);
            collider.dimension = new Vector2f(RandomSceneMaker.tileSize, RandomSceneMaker.tileSize);
            rb = new Rigidbody(transform, ref collider);
            rb.isStatic = true;
        }

        public void hideWall()
        {
            spriteRend.draw = false;
            rb.active = false;
        }

        public void resetWall(Vector2f pos, Texture img)
        {
            spriteRend.draw = true;
            rb.active = true;
            transform.position = pos;
            spriteRend.sprite.Texture = img;
            spriteRend.sprite.Position = transform.position;
        }
    }
}
