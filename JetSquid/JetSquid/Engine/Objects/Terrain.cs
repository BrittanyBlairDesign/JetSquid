using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Engine.Components.Physics;

namespace Engine.Objects;
public class Terrain:BaseGameObject
{
    protected float SCROLLING_SPEED = -2.0f;
    protected Vector2 SCROLLING_DIRECTION;
    protected int DesignedWidth;
    protected int DesignedHeight;
    public Terrain(Texture2D texture, float speed = 0.0f, Direction direction = Direction.STOP, int DesignedWidth = 1920, int DesignedHeight = 1080)
    {
        _texture = texture;
        _position = new Vector2(0, 0);
        SCROLLING_SPEED = speed;
        SCROLLING_DIRECTION = new Directions().GetDirection(direction);

        this.DesignedWidth = DesignedWidth;
        this.DesignedHeight = DesignedHeight;
    }
    
    public override void Render(SpriteBatch spriteBatch)
    {
        
        var sourceRectangle = new Rectangle(0,0,_texture.Width, _texture.Height);
        for(int nbVertical = -1;
            nbVertical < DesignedWidth/ _texture.Height +1;
            nbVertical++)
        {
            var y = (int) _position.Y + nbVertical * _texture.Height;

            for (int nbHorizontal = -0;
                nbHorizontal < DesignedWidth / _texture.Width + 1;
                nbHorizontal++)
            {
                var x = (int) _position.X + nbHorizontal * _texture.Width;

                var destRectangle = new Rectangle(x,y,_texture.Width,_texture.Height);
                spriteBatch.Draw(_texture, destRectangle, sourceRectangle, Color.White);
            }
        }
        
        if(SCROLLING_DIRECTION.X!= 0.0f)
        {
            _position.X = (int)(Position.X + (SCROLLING_SPEED * SCROLLING_DIRECTION.X)) % _texture.Width;
        }
        
        if(SCROLLING_DIRECTION.Y != 0.0f)
        {
            _position.Y = (int)(Position.Y + (SCROLLING_SPEED * SCROLLING_DIRECTION.Y)) % _texture.Height;
        }
    }
}

