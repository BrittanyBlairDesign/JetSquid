
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Components.Physics;
using Engine.Particles;

namespace Engine.Objects;

public class Projectile : BaseGameObject
{
    public Movement _movement;
    protected Emitter _trail;
    
    public Projectile(Texture2D texture, Vector2 startPos, bool debug = false, float speed = 1.0f, Direction direction = Direction.STOP)
    {
        _texture = texture;
        Position = startPos;
        _Debug = debug;

        _movement = new Movement(speed, direction);
    }
}