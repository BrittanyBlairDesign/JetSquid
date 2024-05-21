
using Engine.Particles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
namespace JetSquid
{
    public class InkParticleState : EmitterParticleState
    {
        public override int MinLifespan => 60; // equivalent to 1 second

        public override int MaxLifespan => 150;

        public override float Velocity => 1.0f;

        public override float VelocityDeviation => 1.0f;

        public override float Acceleration => 1.0f;

        public override Vector2 Gravity => new Vector2(0, 9.8f);

        public override float Opacity => 0.8f;

        public override float OpacityDeviation => 0.1f;

        public override float OpacityFadingRate => .98f;

        public override float Rotation => 0.0f;

        public override float RotationDeviation => 0.0f;

        public override float Scale => 0.3f;

        public override float ScaleDeviation => 0.5f;
    }

    public class InkJetEmitter : Emitter
    {
        private const int NbParticles = 10;
        private const int MaxParticles = 1000;
        private static Vector2 Direction = new Vector2(-0.5f, 0.5f); // pointing downward
        private const float Spread = 30.0f;

        public InkJetEmitter(Texture2D texture, Vector2 position) :
            base(texture, position, new InkParticleState(), new ConeEmitterType(Direction, Spread), NbParticles, MaxParticles)
        { }
    }
}
