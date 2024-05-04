
using Microsoft.Xna.Framework;


public abstract class EmitterParticleState
{
    private RandomNumberGenerator _rand = new RandomNumberGenerator();

    public abstract int MinLifespan { get; }
    public abstract int MaxLifespan { get; }

    public abstract float Velocity { get; }
    public abstract float VelocityDeviation { get; }
    public abstract float Acceleration { get; }
    public abstract Vector2 Gravity { get; }

    public abstract float Opacity { get; }
    public abstract float OpacityDeviation { get; }
    public abstract float OpacityFadingRate { get; }

    public abstract float Rotation { get; }
    public abstract float RotationDeviation { get; }
    
    public abstract float Scale { get; }
    public abstract float ScaleDeviation { get; }

    public int GenerateLifespan()
    {
        return _rand.NextRandom(MinLifespan, MaxLifespan);
    }

    public float GenerateVelocity()
    {
        return GenerateFloat(Velocity, VelocityDeviation);
    }

    public float GenerateOpacity()
    {
        return GenerateFloat(Opacity, OpacityDeviation);
    }

    public float GenerateRotation()
    {
        return GenerateFloat(Rotation, RotationDeviation);
    }

    public float GenerateScale()
    {
        return GenerateFloat(Scale, ScaleDeviation);
    }

    protected float GenerateFloat(float startN, float deviation)
    {
        var halfDeviation = deviation / 2;
        return _rand.NextRandom(startN - halfDeviation, startN + halfDeviation);
    }
}