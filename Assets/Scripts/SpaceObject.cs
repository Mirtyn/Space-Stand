using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpaceObject
{
    protected Vector2 position;
    protected SpaceObjectVisual visual;
    protected string description => attributes.GetAttributesAsString();
    protected string name;
    protected ISpaceObjectAttributes attributes;

    public SpaceObject(Vector2 position, SpaceObjectVisual visual)
    {
        this.position = position;
        this.visual = visual;
    }

    public void Clicked()
    {

    }
}


public interface ISpaceObjectAttributes
{
    public abstract string GetAttributesAsString();
    public abstract string GetName();
    public abstract void GetAttributes();
}

namespace SpaceStand.Planets.Attributes
{
    public enum PlanetSize
    {
        Dwarf,
        Normal,
        Giant
    }

    public enum PlanetType
    {
        None,
        //[PlanetTypeAttribute(Water)]
        Terrestrial,
        Ocean,
        EarthLike,
        Gas,
        Lava
    }

    public enum ClimateType
    {
        None,
        Frozen,
        Temperate,
        Tropical,
        Dry,
        Volcanic
    }

    public enum SoilType
    {
        None,
        Dirt,
        Sand,
        Stone,
        MotenStone,
        Ocean
    }

    public enum MineralsType
    {
        None,
        Rare,
        Common,
        Abundant
    }

    public enum VegitaitionType
    {
        None,
        Desert,
        Grassland,
        Forest,
        Tundra,
        Steppe,
        Icy
    }
}
