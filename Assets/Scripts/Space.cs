using NUnit.Framework;
using UnityEngine;

public class Space
{
    public int? Seed { get; set; }

    public int Size { get; set; }

    public Space()
        : this(2000, null)
    {
    }

    public Space(int size, int? seed)
    {
        Seed = seed ?? RandomGenerator.RandomSeed();
        Size = size;
    }
}
