
using System;

public class RandomNumberGenerator
{
    private Random _rand;

    public RandomNumberGenerator()
    {
        _rand = new Random();
    }

    public int NextRandom() => _rand.Next();
    public int NextRandom(int Max) => _rand.Next(Max);
    public int NextRandom(int Min, int Max) => _rand.Next(Min, Max);
    public float NextRandom(float Max) => (float)_rand.NextDouble() * Max;
    public float NextRandom(float Min, float Max) => (float)_rand.NextDouble() * (Max - Min) + Min;

}
