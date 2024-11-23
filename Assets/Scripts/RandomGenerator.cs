using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class RandomGenerator
{
    private int _seed;
    private Random _random;
    private static Random _seedGenerator = new Random();

    public RandomGenerator()
        : this(_seedGenerator.Next())
    {

    }

    public RandomGenerator(int? seed)
    {
        ReSeed(seed ?? _seedGenerator.Next());
    }

    public int Seed
    {
        get { return _seed; }
    }

    public int ReSeed()
    {
        return ReSeed(_seedGenerator.Next());
    }

    public int ReSeed(int seed)
    {
        _seed = seed;
        _random = new Random(_seed);
        return _seed;
    }

    public int Reset()
    {
        return ReSeed(Seed);
    }

    public double Value()
    {
        return _random.NextDouble();
    }

    public double Value(double max)
    {
        return Value(0, max);
    }

    public double Value(double min, double max)
    {
        return min + _random.NextDouble() * (max - min);
    }

    public float Value(float max)
    {
        return Value(0, max);
    }

    public float Value(float min, float max)
    {
        return min + (float)_random.NextDouble() * (max - min);
    }

    public int Value(int max)
    {
        return Value(0, max);
    }

    public int Value(int min, int max)
    {
        return (int)(min + _random.NextDouble() * (max - min));
    }

    public byte Byte()
    {
        return Byte(0, 256);
    }

    public byte Byte(int max)
    {
        return Byte(0, max);
    }

    public byte Byte(int min, int max)
    {
        return (byte)(min + _random.NextDouble() * (max - min));
    }

    public bool Bool()
    {
        return Value(0, 2) == 0;
    }
}
