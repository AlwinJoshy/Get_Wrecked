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
    static class Gizmo
    {
        #region Public Variables
        static public bool drawGizmo = false;
        #endregion

        #region Private Variables
        static private RectangleShape colliderBox = new RectangleShape();
        static private CircleShape pointSpot = new CircleShape();
        static private RectangleShape rayLine = new RectangleShape();

        static public List<Vector2f> bufferPoints= new List<Vector2f> (50);
        #endregion

        #region Public Methods
        static public void StartGizmo()
        {
            colliderBox.FillColor = Color.Transparent;
            colliderBox.OutlineThickness = 1.5f;
            colliderBox.OutlineColor = Color.Green;

            rayLine.FillColor = Color.Blue;
            rayLine.Size = new Vector2f(0, 0.5f);

            pointSpot.Radius = 5;
            pointSpot.Origin = new Vector2f(5, 5);
            pointSpot.FillColor = Color.Red;
        }

        static public void DrawColliderBox(ref AABBBox collider, Vector2f pos)
        {
            colliderBox.Position = pos;
            colliderBox.Size = collider.dimension;
            Init.window.Draw(colliderBox);
        }

        static public void DrawPoint(Vector2f pos)
        {
            pointSpot.Position = pos;
            Init.window.Draw(pointSpot);
        }

        static public void DrawLine(Rigidbody rb)
        {
            rayLine.Size = new Vector2f(VectorMath.Distance(rb.velocity), 1f);
            rayLine.Position = rb.transform.position;
            rayLine.Rotation = (float)Math.Atan2((double)rb.velocity.Y, (double)rb.velocity.X) / 0.0174533f;
            Init.window.Draw(rayLine);
        }

        static public void BufferPointsDraw()
        {
            if (drawGizmo)
            {
                for (int i = 0; i < bufferPoints.Count; i++)
                {
                    pointSpot.Position = bufferPoints[i];
                    Init.window.Draw(pointSpot);
                }
            }
            bufferPoints.Clear();
        }

        #endregion

        #region Private Methods

        #endregion
    }
}
