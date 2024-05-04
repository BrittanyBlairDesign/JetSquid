

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Collections.Generic;

public class ButtonObject : BaseGameObject
{

    protected Color _color;
    private bool isDebug = false;

    public ButtonObject(Texture2D texture, Vector2 location)
    {
        this._texture = texture;
        this.Position = location - new Vector2(Width / 2, 0);
    }

    public ButtonObject(GraphicsDevice graphics, Texture2D texture, Vector2 location)
    {
        this._texture = texture;
        this.Position = location - new Vector2(Width / 2, 0);

        isDebug = true;

    }

    public virtual bool isHovering()
    {
        MouseState state = Mouse.GetState();
        if (BoundingBoxes[0].Rectangle.Contains(state.Position))
        {
            _color = Color.WhiteSmoke;
            
            return true;
        }
        else
        {
            Trace.WriteLine("Mouse Position = " + state.Position);
            _color = Color.White;
            return false;
        }
    }

    public override void Render(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, _position, _color);
        
        if(_Debug)
        {
            RenderBoundingBoxes(spriteBatch);
        }
    }
}

public class TextButton :ButtonObject
{

    private string text;
    private SpriteFont font;

    public TextButton(Texture2D texture, Vector2 location, string text, SpriteFont font) : base(texture, location)
    {
        this.text = text;
        this.font = font;
    }

    public override void Render(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(font, text, Position, _color);
        base.Render(spriteBatch);
    }
}