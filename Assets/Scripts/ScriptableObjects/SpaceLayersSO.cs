using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SpaceLayersSO : ScriptableObject
{
    public GenerationLayer StartSpaceObject;
    public float Distance;
    public int NumberStartObjects;
    public List<GenerationLayer> Layers;
}


[Serializable]
public struct GenerationLayer
{
    public SpaceObjectType Type;
    public int MinRings;
    public int MaxRings;
    public float MinRingDistance;
    public float MaxRingDistance;
}

public enum SpaceObjectType
{
    Planet,
    Star,
    Astroid,
    Blackhole
}
