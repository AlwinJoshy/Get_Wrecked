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
    static class Physics
    {
        static public List<Rigidbody> rigidBodyList = new List<Rigidbody>();
        static public List<TriggerListObjectData> triggerObjectList = new List<TriggerListObjectData>();
        static private Vector2f thisObjPos = new Vector2f();
        static private Vector2f otherObjPos = new Vector2f();
        static private Vector2f reflectNormal = new Vector2f();
        static private Vector2f objectsVector = new Vector2f();
        static private float penetration = 0;

        public struct TriggerListObjectData
        {
            public int tiggerId;
            public int causeId;
        }


        static public void allActions()
        {
            CollisionManagenent();
            moveRigidbodies();
        }

        static private void moveRigidbodies()
        {
            for (int i = 0; i < rigidBodyList.Count; i++)
            {
                if (!rigidBodyList[i].isStatic)
                {
                    rigidBodyList[i].transform.position += rigidBodyList[i].velocity * (Init.renderCountPerSecond + Init.deltaTime - Init.lastRenderTime) / 1000;
                    rigidBodyList[i].velocity *= rigidBodyList[i].drag;
                }
            }
        }

        static private void CollisionManagenent()
        {
            triggerObjectList.Clear();
            for (int i = 0; i < rigidBodyList.Count; i++)
            {
                // selected body
                if (rigidBodyList[i].active && !rigidBodyList[i].isStatic)
                {
                    if(Gizmo.drawGizmo) Gizmo.DrawLine(rigidBodyList[i]);
                    // compare body
                    for (int n = 0; n < rigidBodyList.Count; n++)
                    {
                        if (rigidBodyList[n].active && rigidBodyList[i] != rigidBodyList[n])
                        {
                            thisObjPos = rigidBodyList[i].transform.position + rigidBodyList[i].collider.min;
                            if (Gizmo.drawGizmo) Gizmo.DrawColliderBox(ref rigidBodyList[i].collider, thisObjPos);
                            otherObjPos = rigidBodyList[n].transform.position + rigidBodyList[n].collider.min;
                            //   Console.WriteLine("inside loop 2");
                            if (thisObjPos.X < otherObjPos.X + rigidBodyList[n].collider.dimension.X &&
                               thisObjPos.X + rigidBodyList[i].collider.dimension.X > otherObjPos.X &&
                               thisObjPos.Y < otherObjPos.Y + rigidBodyList[n].collider.dimension.Y &&
                               thisObjPos.Y + rigidBodyList[i].collider.dimension.Y > otherObjPos.Y)
                            {
                                if (rigidBodyList[i].isKinamatic)
                                {
                                    objectsVector = (rigidBodyList[n].transform.position + rigidBodyList[n].collider.min + rigidBodyList[n].collider.dimension / 2) -
                                                    (rigidBodyList[i].transform.position + rigidBodyList[i].collider.min + rigidBodyList[i].collider.dimension/2);
                                    if (Gizmo.drawGizmo) Gizmo.DrawColliderBox(ref rigidBodyList[n].collider, otherObjPos);
                                    if (Gizmo.drawGizmo) Gizmo.DrawPoint(rigidBodyList[n].transform.position + rigidBodyList[n].collider.min + rigidBodyList[n].collider.dimension / 2);
                                    if (Gizmo.drawGizmo) Gizmo.DrawPoint(rigidBodyList[i].transform.position + rigidBodyList[i].collider.min + rigidBodyList[i].collider.dimension / 2);
                                    float overlapX = (rigidBodyList[i].collider.dimension.X + rigidBodyList[n].collider.dimension.X) * 0.5f - Math.Abs(objectsVector.X);

                                    if (overlapX > 0)
                                    {
                                        float overlapY = (rigidBodyList[i].collider.dimension.Y + rigidBodyList[n].collider.dimension.Y) * 0.5f - Math.Abs(objectsVector.Y);
                                        
                                        if (overlapY > 0)
                                        {
                                            if (overlapX < overlapY)
                                            {
                                                if (objectsVector.X < 0)
                                                {
                                                    reflectNormal = new Vector2f(-1, 0);
                                                }
                                                else
                                                {
                                                    reflectNormal = new Vector2f(1, 0);
                                                }
                                                penetration = overlapX;
                                            }

                                            else
                                            {
                                                if (objectsVector.Y < 0)
                                                {
                                                    reflectNormal = new Vector2f(0, -1);
                                                }
                                                else
                                                {
                                                    reflectNormal = new Vector2f(0, 1);;
                                                }
                                                penetration = overlapY;
                                            }
                                        }

                                    }

                                    // getting relative velocity
                                    Vector2f relativeVelocity = rigidBodyList[n].velocity - rigidBodyList[i].velocity;
                                    // get the velocity along normal face
                                    float velocityToNormal = VectorMath.Dot(relativeVelocity, reflectNormal);
                                    if (velocityToNormal < 0)
                                    {
                                        //    rigidBodyList[i].velocity += 0.2f * reflectNormal * velocityToNormal * penetration;
                                        //     rigidBodyList[i].velocity += 3.5f * reflectNormal * velocityToNormal * (penetration + 10) * Math.Min(rigidBodyList[i].elasticity, rigidBodyList[n].elasticity) * rigidBodyList[i].inversMass;
                                        rigidBodyList[i].velocity += 5.5f * reflectNormal * velocityToNormal * (penetration + 1f) * Math.Min(rigidBodyList[i].elasticity, rigidBodyList[n].elasticity) * rigidBodyList[i].inversMass;
                                        if (VectorMath.DistanceSquared(rigidBodyList[i].velocity)/1000000 > 20000) Console.WriteLine("Over fired object of ID : " + (Init.PhysicsObjectTypes)rigidBodyList[i].transform.objectID);
                                    }

                                    //         rigidBodyList[i].transform.position -= rigidBodyList[i].velocity * ((Init.renderCountPerSecond + Init.deltaTime - Init.lastRenderTime) / 1000) * 1.3f;

                                    if (rigidBodyList[i].isTrigger == true)
                                    {
                                        triggerObjectList.Add(new TriggerListObjectData { tiggerId = i, causeId = n });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < triggerObjectList.Count; i++)
            {
                try

                {
                    //   triggerObjectList[i].obj.DoOnTrigger(ref rigidBodyList[n].transform.objectID);
                    rigidBodyList[triggerObjectList[i].tiggerId].obj.DoOnTrigger(ref rigidBodyList[triggerObjectList[i].causeId].transform.objectID);
                }
                catch
                {
                    Console.WriteLine("Trigger function not found...");
                }
            }

        }

        static public void Raycast(int checkObjectID)
        {

        }
    }
}
