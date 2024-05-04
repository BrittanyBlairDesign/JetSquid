
using Microsoft.Xna.Framework;

public class Segment
{
    public Vector2 point1 { get; set; }
    public Vector2 point2 { get; set; }

    public Segment(Vector2 point1, Vector2 point2)
    {
        this.point1 = point1;
        this.point2 = point2;
    }
}
