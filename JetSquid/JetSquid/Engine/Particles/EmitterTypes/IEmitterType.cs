using Microsoft.Xna.Framework;

public interface IEmitterType
{
    Vector2 GetParticleDirection();
    Vector2 GetParticlePosition(Vector2 emitterPosition);
}