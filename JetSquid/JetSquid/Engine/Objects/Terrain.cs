using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


public class Terrain:BaseGameObject
{
    protected float SCROLLING_SPEED = -2.0f;
    protected Vector2 SCROLLING_DIRECTION;
    public Terrain(Texture2D texture, float speed = 0.0f, Direction direction = Direction.STOP)
    {
        _texture = texture;
        _position = new Vector2(0, 0);
        SCROLLING_SPEED = speed;
        SCROLLING_DIRECTION = new Directions().GetDirection(direction);
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
        

        Vector2 velocity = new Vector2((SCROLLING_SPEED * SCROLLING_DIRECTION.X) % _texture.Width,
                                       (SCROLLING_SPEED * SCROLLING_DIRECTION.Y) % _texture.Height);
        _position += velocity;  
    }
}

