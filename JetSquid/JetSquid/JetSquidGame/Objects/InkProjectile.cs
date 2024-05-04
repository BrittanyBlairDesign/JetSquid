
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JetSquid;

public class InkProjectile : Projectile
{
    public InkProjectile(Texture2D texture, Vector2 startPos,
                         bool debug = false, float speed = 1.0f,
                         Direction direction = Direction.STOP) :
                    base(texture, startPos, debug, speed, direction) { }

}