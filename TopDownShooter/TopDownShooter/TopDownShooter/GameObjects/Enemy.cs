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
    public class Enemy
    {
        public bool alive = false;
        public SpriteRendered spriteRend;
        public Rigidbody rb;
        public AABBBox collider;
        public Transform transform;
        public float health = 100;
        public float points = 0;
        public float maxSpeed = 400;
        public float rotationmaxSeep = 10;
        // process variabes
        Vector2f moveDir = new Vector2f();
        Vector2f wanderDir = new Vector2f();

        private ParticleEffect explosion = new ParticleEffect(new Texture("deathBlast.png"), -1);

     //   private List<Bullet> bulletPool = new List<Bullet>();

        private float nextShootTime = 5;
        private int fireRange = 50000;
        private float lastActionSwitchback = 0;
        private float actionSwitchbackMinSpan = 3;
        private Vector2f mylastPos;

        private enum EnemyAIAction
        {
            chase,
            boost,
            wander
        }

        private EnemyAIAction currentState;
        private float nextStateSwitchTime = 0;

        private List<Vector2f> movePoints = new List<Vector2f>();
        private int currentPoint = 0;
        private bool wayPointsCollected = false;

        public Enemy(int enemyType, Vector2f pos)
        {
            this.transform = new Transform();
            this.transform.position = pos;

            if (enemyType == (int)Init.PhysicsObjectTypes.enemyHot)
            {
                this.transform.objectID = (int)Init.PhysicsObjectTypes.enemyHot;
                spriteRend = new SpriteRendered(new Texture("EnemyStunner.png"), 1, true);
                maxSpeed = 80;
            }

            else if (enemyType == (int)Init.PhysicsObjectTypes.enemyShock)
            {
                this.transform.objectID = (int)Init.PhysicsObjectTypes.enemyShock;
                spriteRend = new SpriteRendered(new Texture("EnemyShocker.png"), 1, true);
                maxSpeed = 150;
            }

            else if (enemyType == (int)Init.PhysicsObjectTypes.enemyShoot)
            {
                this.transform.objectID = (int)Init.PhysicsObjectTypes.enemyShoot;
                spriteRend = new SpriteRendered(new Texture("EnemyShooter.png"), 1, true);
                fireRange = Init.randGen.Next(40000, 100000);
                maxSpeed = 150;
            }

            spriteRend.sprite.Origin = new Vector2f(16, 16);
            spriteRend.transform = this.transform;
            spriteRend.moving = true;

            collider = new AABBBox { min = new Vector2f(-16, -16), dimension = new Vector2f(32, 32) };
            rb = new Rigidbody(transform, ref collider);
            rb.isTrigger = true;
            rb.drag = 1;
            rb.elasticity = 1;
            rb.inversMass = 1 / 20.0f;
            rb.obj = this;
            Disable();
        }

        public void ShootBullet()
        {
            for (int i = 0; i < RandomSceneMaker.bulletList.Length; i++)
            {
                if (!RandomSceneMaker.bulletList[i].rb.active)
                {
                    Vector2f fireDir = VectorMath.Normalize(moveDir * 15 + new Vector2f(Init.randGen.Next(-10, 10) / 5, Init.randGen.Next(-10, 10) / 5));
                    RandomSceneMaker.bulletList[i].rb.active = true;
                    RandomSceneMaker.bulletList[i].transform.position = transform.position + (fireDir * 40);
                    RandomSceneMaker.bulletList[i].transform.Angle = (float)Math.Atan2((double)fireDir.Y, (double)fireDir.X) / 0.0174533f; ;
                    RandomSceneMaker.bulletList[i].rb.velocity = fireDir * 600f;
                    RandomSceneMaker.bulletList[i].spriteRend.sprite.Position = transform.position + fireDir;
                    RandomSceneMaker.bulletList[i].spriteRend.draw = true;
                    break;
                }
            }
        }

        public void spawnEnemy(Vector2f pos)
        {
            fireRange = Init.randGen.Next(40000, 100000);
            transform.position = pos;
            Enable();
        }

        private void EmergencyTermination()
        {
            Console.WriteLine("enemy entity eliminated due to out of bound..");
            Console.WriteLine("enemy grid position was.." + transform.position.X / RandomSceneMaker.tileSize + "-" + transform.position.Y / RandomSceneMaker.tileSize);
            Disable();
            RandomSceneMaker.RespawnEnemy();
            transform.position = mylastPos;
        }

        public void EnemyAIActions()
        {
            float diadonal = transform.position.X + transform.position.Y;
            if (transform.position.X / RandomSceneMaker.tileSize >= RandomSceneMaker.sceneSize ||
                transform.position.Y / RandomSceneMaker.tileSize >= RandomSceneMaker.sceneSize ||
                transform.position.X <= 0 ||
                transform.position.Y <= 0)
            {
                EmergencyTermination();
            }

            else
            {
                mylastPos = transform.position;
            }

            if (alive)
            {
                ActionSelect();
            }
        }

        private void ActionSelect()
        {
            if (Init.mainClock.ElapsedTime.AsSeconds() >= lastActionSwitchback + actionSwitchbackMinSpan && VectorMath.DistanceSquared(RandomSceneMaker.playerObj.transform.position - transform.position) > 60000)
            {

                if (wayPointsCollected)
                {
                    MoveAlongPoints();
                }

                else
                {
                    GetPath();
                }

                if (wayPointsCollected && VectorMath.DistanceSquared(RandomSceneMaker.playerObj.transform.position - movePoints[movePoints.Count - 1]) > 30000)
                {
                    GetPath();
                }


            }

            else
            {
                if (wayPointsCollected)
                {
                    lastActionSwitchback = Init.mainClock.ElapsedTime.AsSeconds();
                    wayPointsCollected = false;
                }

                if (Init.mainClock.ElapsedTime.AsSeconds() >= nextStateSwitchTime)
                {
                    currentState = (EnemyAIAction)Init.randGen.Next(0, 1);
                    nextStateSwitchTime = Init.randGen.Next(1, 5) + Init.mainClock.ElapsedTime.AsSeconds();
                }
                AIActionSet();
            }

        }

        public void Disable()
        {
            alive = false;
            rb.velocity *= 0;
            this.rb.active = false;
            this.spriteRend.draw = false;
            lastActionSwitchback = 0;
            wayPointsCollected = false;
        }

        private void Enable()
        {
            this.health = 100;
            this.rb.velocity *= 0;
            this.rb.active = true;
            this.spriteRend.draw = true;
            alive = true;
        }

        private void AIActionSet()
        {
            switch (currentState)
            {
                case EnemyAIAction.chase:

                    moveDir = VectorMath.Normalize(RandomSceneMaker.playerObj.transform.position - this.transform.position);
                    transform.Angle = (float)Math.Atan2((double)moveDir.Y, (double)moveDir.X) / 0.0174533f;

                    if (transform.objectID == (int)Init.PhysicsObjectTypes.enemyShoot && VectorMath.DistanceSquared(RandomSceneMaker.playerObj.transform.position - this.transform.position) < fireRange)
                    {
                        rb.velocity *= 0;

                        if (transform.objectID == (int)Init.PhysicsObjectTypes.enemyShoot && Init.mainClock.ElapsedTime.AsSeconds() >= nextShootTime)
                        {
                            nextShootTime = Init.mainClock.ElapsedTime.AsSeconds() + 0.5f;
                            ShootBullet();
                        }
                    }

                    else
                    {
                        rb.AddForceMaxClamped(maxSpeed * moveDir, maxSpeed);
                    }

                    break;

                case EnemyAIAction.boost:
                    moveDir = VectorMath.Normalize(RandomSceneMaker.playerObj.transform.position - this.transform.position);
                    //       rb.AddForceMaxClamped(1.5f * maxSeep * moveDir, 1.5f * maxSeep);
                    transform.Angle = (float)Math.Atan2((double)moveDir.Y, (double)moveDir.X) / 0.0174533f;

                    if (transform.objectID == (int)Init.PhysicsObjectTypes.enemyShoot && VectorMath.DistanceSquared(RandomSceneMaker.playerObj.transform.position - this.transform.position) < fireRange)
                    {
                        rb.velocity *= 0;
                        if (transform.objectID == (int)Init.PhysicsObjectTypes.enemyShoot && Init.mainClock.ElapsedTime.AsSeconds() >= nextShootTime)
                        {
                            nextShootTime = Init.mainClock.ElapsedTime.AsSeconds() + 0.2f;
                            ShootBullet();
                        }
                    }

                    else
                    {
                        rb.AddForceMaxClamped(maxSpeed * moveDir, maxSpeed);
                    }


                    break;

                case EnemyAIAction.wander:
                    moveDir = VectorMath.Normalize(rb.velocity + moveDir) * 10f + (VectorMath.GetDirVector(Init.randGen.Next(0, 360)) * 7f);
                    rb.AddForceMaxClamped(VectorMath.Normalize(moveDir) * 10, 100);
                    //       rb.AddForce(moveDir * 5);
                    transform.Angle = (float)Math.Atan2((double)rb.velocity.Y, (double)rb.velocity.X) / 0.0174533f;
                    break;
            }
        }

        public void GetPath()
        {
            if (RandomSceneMaker.movePoints[(int)transform.position.X / RandomSceneMaker.tileSize, (int)transform.position.Y / RandomSceneMaker.tileSize].active == false)
            {
                EmergencyTermination();
                return;
            }

            movePoints.Clear();
            currentPoint = 0;
            movePoints = RandomSceneMaker.GeneratePath(this.transform, RandomSceneMaker.playerObj.transform);
            wayPointsCollected = true;
        }

        public void MoveAlongPoints()
        {
            if (movePoints == null)
            {
                Console.WriteLine("breakage prenevted due to null rout..");
                GetPath();
                return;
            }

            if (currentPoint < movePoints.Count)
            {
                //   moveDir = VectorMath.Normalize(movePoints[currentPoint] - transform.position) * 10f + (VectorMath.GetDirVector(Init.randGen.Next(0, 360)) * 3f);
                moveDir = VectorMath.Normalize(movePoints[currentPoint] - transform.position);
                transform.Angle = (float)Math.Atan2((double)moveDir.Y, (double)moveDir.X) / 0.0174533f;
                rb.AddForceMaxClamped(moveDir * 300, maxSpeed);
                Gizmo.bufferPoints.Add(movePoints[currentPoint]);
                if (VectorMath.DistanceSquared(movePoints[currentPoint] - transform.position) <= 5000)
                {
                    currentPoint += 1;
                }
            }

            else
            {
                wayPointsCollected = false;
            }
        }

        public void DoOnTrigger(ref int otherObjId)
        {
            if (otherObjId == (int)Init.PhysicsObjectTypes.bulletPlayer)
            {
                if (transform.objectID == (int)Init.PhysicsObjectTypes.enemyHot) health -= 35;
                else if (transform.objectID == (int)Init.PhysicsObjectTypes.enemyShock) health -= 25;
                else if (transform.objectID == (int)Init.PhysicsObjectTypes.enemyShoot) health -= 15;

                if (health <= 0)
                {
                    if (transform.objectID == (int)Init.PhysicsObjectTypes.enemyHot)
                    {
                        RandomSceneMaker.ChangeHealth(5);
                        RandomSceneMaker.totalScore += 10;
                    }
                    else if (transform.objectID == (int)Init.PhysicsObjectTypes.enemyShock)
                    {
                        RandomSceneMaker.ChangeHealth(10);
                        RandomSceneMaker.totalScore += 30;
                    }
                    else if (transform.objectID == (int)Init.PhysicsObjectTypes.enemyShoot)
                    {
                        RandomSceneMaker.ChangeHealth(15);
                        RandomSceneMaker.totalScore += 60;
                    }

                    Disable();
                    explosion.spriteAnim.Start(transform.position);
                    RandomSceneMaker.RespawnEnemy();
                }
            }
        }

    }
}
