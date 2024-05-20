
using Engine.Particles;
using System.Numerics;
namespace JetSquid;
public class InkTrailEmitter : ConeEmitterType
{
  public InkTrailEmitter(Vector2 direction, float spread) : base(direction, spread)
    { }
}

