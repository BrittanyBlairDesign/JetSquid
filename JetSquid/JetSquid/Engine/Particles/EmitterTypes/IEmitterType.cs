using Microsoft.Xna.Framework;

namespace Engine.Particles;
public interface IEmitterType
{
    Vector2 GetParticleDirection();
    Vector2 GetParticlePosition(Vector2 emitterPosition);
}