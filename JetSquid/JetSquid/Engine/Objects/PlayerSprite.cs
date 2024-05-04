
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System;



public class PlayerSprite : BaseGameObject
{
    public PlayerSprite(Texture2D texture, bool debug = false)
    {
        _texture = texture;
        _Debug = debug;
    }

    public PlayerSprite(Texture2D texture, Vector2 startPos, bool debug = false)
    {
        _texture = texture;
        Position = startPos;
        _Debug = debug;
    }
    

    public override void Render(SpriteBatch spriteBatch)
    {
        base.Render(spriteBatch);
    }
}
