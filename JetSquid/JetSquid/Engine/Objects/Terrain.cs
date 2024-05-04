using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


public class Terrain:BaseGameObject
{
    protected float SCROLLING_SPEED = -2.0f;
    protected bool isSCROLL_HORIZONTAL = true;
    public Terrain(Texture2D texture)
    {
        _texture = texture;
        _position = new Vector2(0, 0);
    }
    
    public override void Render(SpriteBatch spriteBatch)
    {
        var viewport = spriteBatch.GraphicsDevice.Viewport;
        var sourceRectangle = new Rectangle(0,0,_texture.Width, _texture.Height);
        for(int nbVertical = -1;
            nbVertical < viewport.Height/ _texture.Height +1;
            nbVertical++)
        {
            var y = (int) _position.Y + nbVertical * viewport.Height;

            for (int nbHorizontal = -0;
                nbHorizontal < viewport.Width / _texture.Width + 1;
                nbHorizontal++)
            {
                var x = (int) _position.X + nbHorizontal * viewport.Width;

                var destRectangle = new Rectangle(x,y,_texture.Width,_texture.Height);
                spriteBatch.Draw(_texture, destRectangle, sourceRectangle, Color.White);
            }
        }
        
        if(isSCROLL_HORIZONTAL)
        {
            _position.X = (int)(_position.X + SCROLLING_SPEED) % _texture.Width;
        }
        else
        {
            _position.Y = (int)(_position.Y + SCROLLING_SPEED) % _texture.Height;
        }
        
    }

    
}

