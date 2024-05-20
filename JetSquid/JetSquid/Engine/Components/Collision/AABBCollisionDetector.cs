
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Engine.Objects;

namespace Engine.Components.Collision
{
    public class AABBCollisionDetector<P, A>
        where P : BaseGameObject
        where A : BaseGameObject
    {
        private List<P> _passiveObjects;

        public AABBCollisionDetector(List<P> passiveObjects)
        {
            _passiveObjects = passiveObjects;
        }

        public void DetectCollisions(A activeObject, Action<P, A> collisionHandler)
        {
            foreach (var passiveObject in _passiveObjects)
            {
                if (DetectCollision(passiveObject, activeObject))
                {
                    collisionHandler(passiveObject, activeObject);
                }
            }
        }

        public void DetectCollisions(List<A> activeObjects, Action<P, A> collisionHandler)
        {
            foreach (var passiveObject in _passiveObjects)
            {
                foreach (var activeObject in activeObjects)
                {
                    if (DetectCollision(passiveObject, activeObject))
                    {
                        collisionHandler(passiveObject, activeObject);
                    }
                }
            }
        }

        private bool DetectCollision(P passiveObject, A activeObject)
        {
            foreach (var passiveBB in passiveObject.BoundingBoxes)
            {
                foreach (var activeBB in activeObject.BoundingBoxes)
                {
                    if (passiveBB.CollidesWith(activeBB))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
