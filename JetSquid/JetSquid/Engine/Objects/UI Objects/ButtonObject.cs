

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

using Engine.Components.Collision;
namespace Engine.Objects.UI;
public class ButtonObject : BaseGameObject
{

    protected Color _color = Color.White;

    public ButtonObject(Texture2D texture, Vector2 location, int offsetX = 0 , int offsetY = 0, int offsetWidth = 0, int offsetHeight = 0)
    {
        this._texture = texture;
        this.Position = location - new Vector2(Width / 2, 0);

        Vector2 offset = new Vector2(offsetX + Position.X, offsetY + Position.Y);
        BoundingBox2D collision = new BoundingBox2D(offset, Width + offsetWidth, Height + offsetHeight);
        BoundingBoxes.Add(collision);
        _Debug = true;
    }

    public ButtonObject(GraphicsDevice graphics, Texture2D texture, Vector2 location, int offsetX = 0 , int offsetY = 0)
    {
        this._texture = texture;
        this.Position = location - new Vector2(Width / 2, 0);

        Vector2 offset = new Vector2(offsetX + Position.X, offsetY + Position.Y);


        BoundingBox2D collision = new BoundingBox2D(offset, _texture.Width, _texture.Height);
        BoundingBoxes.Add(collision);

        _Debug = true;
    }

    public virtual bool isHovering()
    {
        MouseState state = Mouse.GetState();
        foreach (BoundingBox2D box in BoundingBoxes)
        {
            

            Rectangle rect = box.Rectangle;
            if (rect.Contains(state.Position))
            {
                _color = Color.Red;
                Trace.WriteLine(" Mouse is hovering over Button.");
                return true;
            }
            else
            {
                Trace.WriteLine("Mouse Position = " + state.Position);
                _color = Color.White;
            }
        }

        return false;
    }

    public override void Render(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, _position, _color);
        
        if(_Debug )
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