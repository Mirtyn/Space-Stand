using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class RandomGenerator
{
    private static System.Random _seedGenerator = new System.Random();

    private int _seedOffset = 1;
    private int _entriesIndex = 0;
    private List<Entry> _entries = new List<Entry>();

    private class Entry
    {
        internal int _seed;
        internal System.Random _random;
    }

    public RandomGenerator()
        : this(RandomSeed())
    {

    }

    public RandomGenerator(int? seed)
    {
        _entries.Add(new Entry());

        ReSeed(seed ?? RandomSeed());
    }

    public static int RandomSeed()
    {
        return _seedGenerator.Next();
    }

    public int Seed
    {
        get { return _entries[_entriesIndex]._seed; }
    }

    public int ReSeed()
    {
        return ReSeed(_seedGenerator.Next());
    }

    public int ReSeed(int seed)
    {
        _entries[_entriesIndex]._seed = seed;
        _entries[_entriesIndex]._random = new System.Random(_entries[_entriesIndex]._seed);
        return _entries[_entriesIndex]._seed;
    }

    public int Push()
    {
        _entries.Add(new Entry());
        _entriesIndex++;
        ReSeed(_entries[0]._seed + _seedOffset++);
        return Seed;
    }

    public void Pop()
    {
        if(_entriesIndex > 0)
        {
            _entries.RemoveAt(_entriesIndex - 1);
            _entriesIndex--;
        }
    }

    public int Reset()
    {
        return ReSeed(Seed);
    }

    public double Value()
    {
        return _entries[_entriesIndex]._random.NextDouble();
    }

    public double Value(double max)
    {
        return Value(0, max);
    }

    public double Value(double min, double max)
    {
        return min + _entries[_entriesIndex]._random.NextDouble() * (max - min);
    }

    public float Value(float max)
    {
        return Value(0, max);
    }

    public float Value(float min, float max)
    {
        return min + (float)_entries[_entriesIndex]._random.NextDouble() * (max - min);
    }

    public int Value(int max)
    {
        return Value(0, max);
    }

    public int Value(int min, int max)
    {
        return (int)(min + _entries[_entriesIndex]._random.NextDouble() * (max - min));
    }

    public int Int()
    {
        return Int(0, int.MaxValue);
    }

    public int Int(int max)
    {
        return Int(0, max);
    }

    public int Int(int min, int max)
    {
        return (int)(min + _entries[_entriesIndex]._random.NextDouble() * (max - min));
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
        return (byte)(min + _entries[_entriesIndex]._random.NextDouble() * (max - min));
    }

    public bool Bool()
    {
        return Value(0, 2) == 0;
    }

    public float Radian()
    {
        return Radian(MinRadians, MaxRadians);
    }

    public float Radian(float max)
    {
        return Radian(MinRadians, max);
    }

    public float Radian(float min, float max)
    {
        return (float)(min + _entries[_entriesIndex]._random.NextDouble() * (max - min));
    }

    private static readonly float MinRadians = 0;
    private static readonly float MaxRadians = 6.283185307179586f;

    public void UnitCircleVector(out float x, out float y)
    {
        float angle = Radian();

        x = Mathf.Cos(angle);
        y = Mathf.Sin(angle);
    }

    public Vector2 UnitCircleVector()
    {
        UnitCircleVector(out float x, out float y);

        return new Vector2(x, y);
    }

    public void UnitSphereVector(out float x, out float y, out float z)
    {
        float angle1 = Radian();
        float angle2 = Radian();

        x = Mathf.Sin(angle1) * Mathf.Cos(angle2);
        y = Mathf.Sin(angle1) * Mathf.Sin(angle2);
        z = Mathf.Cos(angle1);
    }

    public Vector3 UnitSphereVector()
    {
        UnitSphereVector(out float x, out float y, out float z);

        return new Vector3(x, y, z);
    }
}
