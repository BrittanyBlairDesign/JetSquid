
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace JetSquid;

public class PlayerSquid : PlayerSprite
{
    public PlayerSquid(Texture2D texture, bool debug = false) :
                  base(texture, debug) { }
    public PlayerSquid(Texture2D texture, Vector2 startPos, bool debug = false) : 
                  base(texture, startPos, debug) { }

}
