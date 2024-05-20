

using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Engine.Components.Physics
{
    public enum Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        UP_LEFT,
        UP_RIGHT,
        DOWN_LEFT,
        DOWN_RIGHT,
        STOP
    }
    public class Directions
    {
        private readonly Dictionary<Direction, Vector2> _directions = new Dictionary<Direction, Vector2>()
    {
        { Direction.UP, new Vector2(0, -1) },
        { Direction.DOWN, new Vector2(0, 1) },
        { Direction.LEFT, new Vector2(1, 0) },
        { Direction.RIGHT, new Vector2(-1, 0) },
        { Direction.UP_LEFT, new Vector2(1, -1) },
        { Direction.UP_RIGHT, new Vector2(-1, -1) },
        { Direction.DOWN_LEFT, new Vector2(1, 1) },
        { Direction.DOWN_RIGHT, new Vector2(-1, 1) },
        { Direction.STOP, new Vector2(0, 0) },
    };

        public Vector2 GetDirection(Direction dir)
        {
            return _directions[dir];
        }
    }
}
