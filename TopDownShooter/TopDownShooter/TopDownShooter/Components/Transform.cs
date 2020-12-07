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
    public class Transform
    {
        public int objectID;
        public float angle;
        public Vector2f forward;
        public Vector2f position;

        public Transform()
        {
            angle = 0;
            forward = new Vector2f(0, 1);
            position = new Vector2f(0, 0);
        }

        public Transform(Vector2f position)
        {
            angle = 0;
            forward = new Vector2f(0, 1);
            this.position = position;
        }

        public float Angle
        {
            get { return angle; }
            set { angle = value;
                GetForward();
            }
        }

        public void GetForward()
        {
            forward = new Vector2f((float)Math.Cos(angle * 0.0174533f), (float)Math.Sin(angle * 0.0174533f));
        }
    }
}
