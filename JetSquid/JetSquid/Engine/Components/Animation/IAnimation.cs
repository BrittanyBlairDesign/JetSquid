
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Components.Animation
{
    public enum PlayState
    {
        PLAY,
        PAUSE,
        RESET,
        REWIND
    }
    public interface IAnimation
    {
        public void ChangeFrameRate(int rate);
        public void ChangePlayState(PlayState state);
        public void Reset();
        public void Update(float deltaTime);
        public void Draw(SpriteBatch spriteBatch, Vector2 location, SpriteEffects effects = SpriteEffects.None);
    }
}
