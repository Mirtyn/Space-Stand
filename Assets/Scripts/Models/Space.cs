using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Space
{
    public int Seed { get; set; }

    public int Size { get; set; }

    public List<Planet> Planets { get; set; } = new List<Planet>();

    public Space()
        : this(2000, RandomGenerator.RandomSeed())
    {
    }

    public Space(int size, int seed)
    {
        Seed = seed;
        Size = size;
    }

    public void Add(Planet planet)
    {
        Planets.Add(planet);
    }

    public void ClearSelectedPlanet()
    {
        foreach(var planet in Planets)
        {
            planet.IsSelected = false;
        }
    }
}
