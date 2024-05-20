
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using Engine.Components.Animation;
using System.Diagnostics;
using Engine.States;
using SpriteSheetAnimationContentPipeline;
using System.Reflection.Metadata.Ecma335;



namespace Engine.Objects;
public class AnimatedSpriteObject : SpriteObject
{
    protected AnimationManager2D _animManager = null;
    public Texture2D _spriteSheet { get { return _texture; } set { _texture = value; } }
  
    public AnimatedSpriteObject(SpriteSheetAnimation animationSheet, bool debug = false, float scale = 1.0f):
            base(animationSheet.TextureSheet, debug, scale) { }

    public AnimatedSpriteObject(SpriteSheetAnimation animationSheet,Vector2 startPos, bool debug = false, float scale = 1.0f) :
          base(animationSheet.TextureSheet, startPos, debug, scale)
    { }
    public virtual void SetManager(int frameRate)
    {
        
        _animManager = new AnimationManager2D( frameRate, _Debug);
    }


    public virtual void SetAnimations()
    {
        // Specific to each animated sprite
    }

    public override void OnNotify(BaseGameStateEvent gameEvent)
    {
        // This is where animation changes are made.
    }


    public override void Render(SpriteBatch spriteBatch)
    {
        _animManager.Draw(spriteBatch, Position,SpriteEffects.None);
        if(_Debug)
        {
            RenderBoundingBoxes(spriteBatch);
        }
    }
}