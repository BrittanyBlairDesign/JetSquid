

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Objects;

namespace JetSquid;

public class Obstacle : SpriteObject
{
    public Obstacle(Texture2D texture, bool debug = false) :
              base(texture, debug)
    { }
    public Obstacle(Texture2D texture, Vector2 startPos, bool debug = false) :
                  base(texture, startPos, debug)
    { }

}
