using SpaceStand.Attributes;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpaceObject
{
    protected Vector2 position;
    protected SpaceObjectVisual visual;
    protected string description => attributes.GetDescription();
    protected string name => attributes.GetName();
    protected ISpaceObjectAttributes attributes;

    public SpaceObject(Vector2 position, SpaceObjectVisual visual)
    {
        this.position = position;
        this.visual = visual;
    }

    public void Clicked()
    {
        SpaceObjectInspector.Instance.SetSpaceObject(name, description);
    }
}


public interface ISpaceObjectAttributes
{
    public abstract string GetAttributesAsJson();
    public abstract string GetDescription();
    public abstract string GetName();
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
        [CoreSoilType(CoreSoilType.Stone)]
        [SurfaceSoilType(SurfaceSoilType.Stone)]
        [MineralType(MineralsType.Abundant)]
        Terrestrial,
        [CoreSoilType(CoreSoilType.Stone)]
        [SurfaceSoilType(SurfaceSoilType.Ocean)]
        [MineralType(MineralsType.Abundant, MineralsType.Common)]
        Ocean,
        [CoreSoilType(CoreSoilType.Stone, CoreSoilType.MoltenStone)]
        [SurfaceSoilType(SurfaceSoilType.Dirt, SurfaceSoilType.Sand)]
        [MineralType(MineralsType.Abundant, MineralsType.Common, MineralsType.Rare)]
        EarthLike,
        [CoreSoilType(CoreSoilType.None, CoreSoilType.MoltenStone, CoreSoilType.Stone)]
        [SurfaceSoilType(SurfaceSoilType.None)]
        [MineralType(MineralsType.Rare, MineralsType.None)]
        Gas,
        [CoreSoilType(CoreSoilType.Stone, CoreSoilType.MoltenStone)]
        [SurfaceSoilType(SurfaceSoilType.Lava)]
        [MineralType(MineralsType.Abundant, MineralsType.Common, MineralsType.Rare, MineralsType.None)]
        Lava
    }

    public enum ClimateType
    {
        None,
        [VegitaitionType(VegitaitionType.Icy, VegitaitionType.Steppe, VegitaitionType.Tundra)]
        Frozen,
        [VegitaitionType(VegitaitionType.Forest, VegitaitionType.Grassland)]
        Temperate,
        [VegitaitionType(VegitaitionType.Forest)]
        Tropical,
        [VegitaitionType(VegitaitionType.None)]
        Dry,
        [VegitaitionType(VegitaitionType.None)]
        Volcanic
    }

    public enum CoreSoilType
    {
        None,
        Stone,
        MoltenStone
    }

    public enum SurfaceSoilType
    {
        None,
        Dirt,
        Sand,
        Stone,
        Ocean,
        Lava
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
