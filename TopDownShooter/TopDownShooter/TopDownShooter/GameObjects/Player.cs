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
    public class Player
    {
        public SpriteRendered spriteRend;
        public SpriteRendered barrelRend;
        public Rigidbody rb;
        public AABBBox collider;
        public Transform transform;
        public float maxSeep = 200;
        public float rotationmaxSeep = 10;
        // process variabes
        Vector2f gunDir = new Vector2f();
        Vector2f mousePos = new Vector2f();
        // Weapons and Ammo
        public Bullet[] bulletPool = new Bullet[20];

        public Player(Vector2f startPos)
        {
            transform = new Transform();
            transform.objectID = (int)Init.PhysicsObjectTypes.player;
            transform.position = startPos;

            spriteRend = new SpriteRendered(new Texture("playerTank_Body.png"), 1, true);
            spriteRend.moving = true;
            spriteRend.transform = this.transform;
            spriteRend.sprite.Origin = new Vector2f(spriteRend.sprite.GetGlobalBounds().Width/2, spriteRend.sprite.GetGlobalBounds().Height / 2);
            spriteRend.sprite.Position = startPos;

            collider.min = new Vector2f(-10,-10);
            collider.dimension = new Vector2f(20, 20);
            rb = new Rigidbody(transform, ref collider);
            rb.drag = 0.8f;
            rb.elasticity = 0.5f;
            rb.inversMass = 1 / 20.0f;
            rb.obj = this;
            rb.isTrigger = true;
            barrelRend = new SpriteRendered(new Texture("playerTank_barrel.png"), 3, true);
            barrelRend.sprite.Origin = new Vector2f(barrelRend.sprite.GetGlobalBounds().Width / 2, barrelRend.sprite.GetGlobalBounds().Height / 2);
            barrelRend.sprite.Position = startPos;

            CreateBullet();
        }

        public void Move(bool up, bool back, bool left, bool right)
        {
            // player movement
            if (up)
            {
                //    transform.position -= transform.forward * maxSeep;
                rb.AddForceMaxClamped(transform.forward * maxSeep, maxSeep);
            }

            if (back)
            {
                //    transform.position += transform.forward * maxSeep;
                rb.AddForceMaxClamped(-transform.forward * maxSeep, maxSeep);
            }

            if (left)
            {
               transform.Angle = transform.angle - rotationmaxSeep;
            }

            if (right)
            {
                transform.Angle = transform.angle + rotationmaxSeep;
            }

            barrelRend.sprite.Position = transform.position;
            mousePos = (Vector2f)Mouse.GetPosition(Init.window);
            gunDir = VectorMath.Normalize(mousePos - new Vector2f(Init.screenWidth/2, Init.screenHeight / 2));
            barrelRend.sprite.Rotation = (float)Math.Atan2((double)gunDir.Y, (double)gunDir.X) / 0.0174533f;
        }

        private void CreateBullet()
        {
            for (int i = 0; i < bulletPool.Length; i++)
            {
                bulletPool[i] = new Bullet(20, (int)Init.PhysicsObjectTypes.bulletPlayer, new Texture("playerTank_bullet.png"));
            }
        }

        public void ShootBullet()
        {
            for (int i = 0; i < bulletPool.Length; i++)
            {
                if (!bulletPool[i].rb.active)
                {
                    bulletPool[i].rb.active = true;
                    rb.velocity = gunDir * 5;
                    bulletPool[i].transform.position = transform.position + (gunDir * 30);
                    bulletPool[i].transform.Angle =(float)Math.Atan2((double)gunDir.Y, (double)gunDir.X) / 0.0174533f; ;
                    bulletPool[i].rb.velocity = gunDir * 600f;
                    bulletPool[i].spriteRend.sprite.Position = transform.position + gunDir;
                    bulletPool[i].spriteRend.draw = true;
                    break;
                }
            }
        }

        public void DoOnTrigger(ref int otherObjId)
        {
            if (otherObjId == (int)Init.PhysicsObjectTypes.enemyHot)
            {
                RandomSceneMaker.ChangeHealth(-3);
            
        }
            else if (otherObjId == (int)Init.PhysicsObjectTypes.enemyShock)
            {
                RandomSceneMaker.ChangeHealth(-6);
            
        }
            else if (otherObjId == (int)Init.PhysicsObjectTypes.bulletEnemy)
            {
                RandomSceneMaker.ChangeHealth(-12);
            }
        }

    }
}
