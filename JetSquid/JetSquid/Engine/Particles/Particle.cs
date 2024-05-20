
using Microsoft.Xna.Framework;

namespace Engine.Particles;
public class Particle
{
    public Vector2 Position { get; private set; }
    public float Scale { get; private set; }
    public float Opacity { get; private set; }

    private int _lifespan;
    private int _age;
    private Vector2 _direction;
    private Vector2 _gravity;
    private float _velocity;
    private float _acceleration;
    private float _rotation;
    private float _opacityFadingRate;

    public Particle() { }

    public virtual void Activate (int lifespan, Vector2 position, Vector2 direction, Vector2 gravity,
                          float velocity, float acceleration, float scale, float rotation,
                          float opacity, float opacityFadingRate)
    {
        _lifespan = lifespan;
        _direction = direction;
        _velocity = velocity;
        _acceleration = acceleration;
        _rotation = rotation;
        _opacityFadingRate = opacityFadingRate;
        _age = 0;

        Position = position;
        Opacity = opacity;
        Scale = scale;
    }

    public virtual bool Update(GameTime gameTime)
    {
        _velocity *= _acceleration;
        _direction *= _gravity;

        var positionDelta = _direction * _velocity;

        Position += positionDelta;
        Opacity *= _opacityFadingRate;

        _age++;
        return _age < _lifespan;
    }

}
