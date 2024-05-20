

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Objects.UI;
public class TextObject : BaseGameObject
{
    public string text;
    public SpriteFont font;
    public string fontStr;
    private Vector2 textLocation;
    public TextObject(Texture2D texture,string text, SpriteFont font)
    {
        this._texture = texture;
        textLocation = new Vector2(10, 10);
        this.text = text;
        this.font = font;
    }

    public TextObject(Texture2D texture, string text, SpriteFont font, Vector2 location)
    {
        this._texture = texture;
        this._position = location;
        this.textLocation = location + new Vector2(10,10);
        this.text = text;
        this.font = font;
    }

    public virtual void setText(string text)
    {
        this.text = text;
    }

    public override void Render(SpriteBatch spriteBatch)
    {
        base.Render(spriteBatch);

        spriteBatch.DrawString(font, text, textLocation, Color.White);
    }
}
