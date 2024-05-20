
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Engine.Objects;

namespace Engine.Components.Collision
{
    // Useful for detecting collisions of projectiles more accurately.
    public class SegmentAABBCollisionDetector<A>
        where A : BaseGameObject
    {
        private A _passiveObject;

        public SegmentAABBCollisionDetector(A passiveObject)
        {
            _passiveObject = passiveObject;
        }

        public void DetectCollisions(Segment segment, Action<A> collisionHandler)
        {
            if (DetectCollision(_passiveObject, segment))
            {
                collisionHandler(_passiveObject);
            }
        }

        public void DetectCollisions(List<Segment> segments, Action<A> collisionHandler)
        {
            foreach (var segment in segments)
            {
                if (DetectCollision(_passiveObject, segment))
                {
                    collisionHandler(_passiveObject);
                }
            }
        }

        public bool DetectCollision(A PassiveObject, Segment segment)
        {
            foreach (var activeBB in PassiveObject.BoundingBoxes)
            {
                if (DetectCollision(segment.point1, activeBB) ||
                   DetectCollision(segment.point2, activeBB))
                {
                    return true;
                }
                else return false;
            }
            return false;
        }

        public bool DetectCollision(Vector2 p, BoundingBox2D box)
        {
            if (p.X < box.Position.X + box.Width &&
               p.X > box.Position.X &&
               p.Y < box.Position.Y + box.Height &&
               p.Y > box.Position.Y)
            {
                return true;
            }
            else return false;
        }
    }
}