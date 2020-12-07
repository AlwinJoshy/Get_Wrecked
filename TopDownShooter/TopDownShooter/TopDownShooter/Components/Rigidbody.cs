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
    public class Rigidbody
    {
        public bool active = true;
        public bool isStatic = false;
        public bool isTrigger = false;
        public bool isKinamatic = true;
        public Transform transform;
        public AABBBox collider;
        public Vector2f velocity = new Vector2f(0.2f,0.2f);
        public float drag = 0.5f;
        public float elasticity = 0.5f;
        public float inversMass = 1/10f;
        public dynamic obj;

        public bool physCalcDone = false;

        public Rigidbody(Transform transform, ref AABBBox coll)
        {
            this.transform = transform;
            collider = coll;
            Physics.rigidBodyList.Add(this);
        }

        public void AddForce(Vector2f force)
        {
            velocity += force;
        }

        public void AddForceMaxClamped(Vector2f force, float clamp)
        {
            velocity += force;

            if (VectorMath.Distance(velocity) > clamp)
            {
                velocity = VectorMath.Normalize(velocity) * clamp;
            }
        }

    }
}
