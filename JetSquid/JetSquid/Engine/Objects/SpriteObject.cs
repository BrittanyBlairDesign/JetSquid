
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
namespace Engine.Objects;

public class SpriteObject : BaseGameObject
{
    public SpriteObject(Texture2D texture, bool debug = false, float scale = 1.0f)
    {
        _texture = texture;
        _scale = scale;
        _Debug = debug;
    }

    public SpriteObject(Texture2D texture, Vector2 startPos, bool debug = false, float scale = 1.0f)
    {
        _texture = texture;
        Position = startPos;
        _scale = scale;
        _Debug = debug;
    }
    
    public override void Render(SpriteBatch spriteBatch)
    {
        base.Render(spriteBatch);
    }
}
