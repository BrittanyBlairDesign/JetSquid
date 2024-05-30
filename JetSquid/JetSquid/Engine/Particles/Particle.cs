

using Engine.Components.Physics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Engine.Particles;
public class Particle
{
    public Vector2 Position { get; private set; }
    public float Scale { get; private set; }
    public float Opacity { get; private set; }

    private int _lifespan; // will tick up every update and monogame updates 60 times per second
    private int _age;
    private Vector2 _direction;
    private Vector2 _gravity;
    private float _velocity;
    private float _acceleration;
    private float _rotation;
    private float _opacityFadingRate;

    public Particle() { }

    public void Activate(int lifespan, Vector2 position, Vector2 direction, Vector2 gravity,
                         float velocity, float acceleration,
                         float scale, float rotation, float opacity, float opacityFadingRate)
    {
        _lifespan = lifespan;
        _direction = direction;
        _velocity = velocity;
        _gravity = gravity;
        _acceleration = acceleration;
        _rotation = rotation;
        _opacityFadingRate = opacityFadingRate;
        _age = 0;

        Position = position;
        Opacity = opacity;
        Scale = scale;
    }

    // returns false if it went past its lifespan
    public bool Update(GameTime gameTime)
    {
        var speed = _acceleration * (_gravity + _direction);
        var positionDelta = speed * _velocity;
        //_direction.Normalize();

        

        Position += positionDelta;

        Opacity *= _opacityFadingRate;

        // return true if particle can stay alive
        _age++;
        return _age < _lifespan;
    }

    public virtual void KillParticle()
    {
        _age = _lifespan;
    }
}


public abstract class ParticleComponent
{
    public ParticleComponent() { }

    public virtual void Update(GameTime gameTime) { }
}

public class ParticleMovement : ParticleComponent
{
    protected Movement _movement;
    public Vector2 _position;
    public ParticleMovement(Movement movement , Vector2 Position)
    {
        _movement = movement;
        _position = Position;
    }
    public ParticleMovement(float speed, Vector2 Position, Vector2 direction )
    {
        _movement = new Movement(speed, direction);
        _position = Position;
    }
    public ParticleMovement(Vector2 maxAcceleration, float mass, Vector2 gravity, Vector2 Position)
    {
        _movement = new PhysicsMovement(maxAcceleration, mass, gravity);
        _position = Position;
    }

    public override void Update(GameTime gameTime)
    {
        _position += _movement.Update(gameTime);
    }
}

public class ParticleOpacity : ParticleComponent
{
    public float _opacity;
    protected float _opacityFadingRate;
    protected float _minOpacity;
    protected float _maxOpacity;

    public ParticleOpacity(float maxOpacity, float minOpacity = 0.0f, float opacityRate = 0.0f, float startOpacity = 0.0f)
    {
        _maxOpacity = maxOpacity;
        _minOpacity = minOpacity;

        RandomNumberGenerator rand = new RandomNumberGenerator();

        if (startOpacity != 0.0f) {_opacity = startOpacity; }
        else
        {
            float opacity = rand.NextRandom(_maxOpacity, _minOpacity);
            _opacity = opacity;
        }

        if (opacityRate != 0.0f) { _opacityFadingRate = opacityRate; }
        else
        {
            _opacityFadingRate = rand.NextRandom(-0.5f, .5f);
        }
    }
    public override void Update(GameTime gameTime)
    {
        float opacity = _opacity;

        opacity += _opacityFadingRate;

        if (opacity > _maxOpacity) { opacity = _maxOpacity; }
        else if (opacity < _minOpacity) { opacity = _minOpacity; }

        _opacity = opacity;
    }

}

public class ParticleScaling : ParticleComponent
{
    public Vector2 _scale;
    protected float _minScale;
    protected float _maxScale;
    protected float _scaleRate;


    public ParticleScaling( float maxScale, float minScale = 0.0f, float scaleRate = 0.0f, float startScale = 0.0f)
    {
        _minScale = minScale;
        _maxScale = maxScale;

        RandomNumberGenerator rand = new RandomNumberGenerator();

        if (startScale > 0.0f) { _scale = new Vector2(startScale, startScale); } 
        else 
        { 
            float scale = rand.NextRandom(minScale, maxScale);
            _scale = new Vector2(scale, scale);
        }

        if(scaleRate != 0.0f) { _scaleRate = scaleRate; }
        else
        {
            _scaleRate = rand.NextRandom(-0.5f, .5f);
        }
    }

    public override void Update(GameTime gameTime)
    {
        Vector2 scale = _scale;

        scale.X += _scaleRate;
        scale.Y += _scaleRate;

        if (scale.X > _maxScale) { scale.X = _maxScale; }
        else if(scale.X < _minScale) { scale.X = _minScale; }

        if(scale.Y > _maxScale) { scale.Y = _maxScale; }
        else if (scale.Y < _minScale) { scale.Y = _minScale; }

        _scale = scale;
    }
}
