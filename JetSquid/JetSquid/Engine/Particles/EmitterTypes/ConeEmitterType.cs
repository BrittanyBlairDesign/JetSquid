
using Microsoft.Xna.Framework;
using System;

public class ConeEmitterType : IEmitterType
{
    public Vector2 Direction { get; private set; }
    public float Spread { get; private set; }

    private RandomNumberGenerator _rand = new RandomNumberGenerator();

    public ConeEmitterType(Vector2 direction, float spread)
    {
        Direction = direction;
        Spread = spread;
    }

    public Vector2 GetParticleDirection()
    {
        if(Direction == Vector2.Zero)
        {
            return new Vector2(0, 0);
        }

        var angle = (float)Math.Atan2(Direction.Y, Direction.X);
        var newAngle = _rand.NextRandom(angle - Spread / 2.0f, angle + Spread / 2.0f);

        var particleDirection = new Vector2((float)Math.Cos(newAngle), (float)Math.Sin(newAngle));
        particleDirection.Normalize();

        return particleDirection;
    }

    public Vector2 GetParticlePosition(Vector2 emitterPosition)
    {
        var x = emitterPosition.X;
        var y = emitterPosition.Y;

        return new Vector2(x, y);
    }

}
