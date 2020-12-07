using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using SFML.Audio;
using SFML.Window;

namespace TopDownShooter
{
    static class VectorMath
    {
        static public Vector2f returnVector;

        static public Vector2f Normalize(Vector2f a)
        {
            returnVector *= 0;
            float h = (float)Math.Sqrt(a.X * a.X + a.Y * a.Y);
            returnVector.X = a.X / h;
            returnVector.Y = a.Y / h;
            return returnVector;
        }

        static public Vector2f GetDirVector(float angle)
        {
            returnVector.X = (float)Math.Sin(angle * 0.0174533f);
            returnVector.Y = (float)Math.Cos(angle * 0.0174533f);
            return returnVector;
        }

        static public float Dot(Vector2f a, Vector2f b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        static public float DistanceSquared(Vector2f a)
        {
            return (float)a.X * a.X + a.Y * a.Y;
        }

        static public float Distance(Vector2f a)
        {
            return (float)Math.Sqrt(a.X * a.X + a.Y * a.Y);
        }

    }
}
