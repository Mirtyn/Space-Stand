using Newtonsoft.Json;
using SpaceStand.Attributes;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpaceObject
{
    private static int _IdRoot = 1;

    public int Id { get; set; }

    public Vector2 position;
    public SpaceObjectVisual visual;
    protected string JSonData;
    public string description => attributes.GetDescription();
    public string name => attributes.GetName();

    public ISpaceObjectAttributes attributes;

    public SpaceObject()
    {
        Id = _IdRoot++;
    }

    public SpaceObject(Vector2 position, SpaceObjectVisual visual)
        : this()
    {
        this.position = position;
        this.visual = visual;
    }

    public virtual void Clicked(SpaceObjectClickEvent e)
    {
        //JsonConvert.
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
        [ResourceAttribute(ResourceType.Coal, ResourceType.Copper, ResourceType.Gold, ResourceType.Iron, ResourceType.Mithril)]
        Terrestrial,
        [CoreSoilType(CoreSoilType.Stone)]
        [SurfaceSoilType(SurfaceSoilType.Ocean)]
        [ResourceAttribute(ResourceType.Coal, ResourceType.Copper, ResourceType.Gold, ResourceType.Iron, ResourceType.Mithril)]
        Ocean,
        [CoreSoilType(CoreSoilType.Stone, CoreSoilType.MoltenStone)]
        [SurfaceSoilType(SurfaceSoilType.Dirt, SurfaceSoilType.Sand)]
        [ResourceAttribute(ResourceType.Coal, ResourceType.Copper, ResourceType.Gold, ResourceType.Iron, ResourceType.Wood, ResourceType.Rubber)]
        EarthLike,
        [CoreSoilType(CoreSoilType.None, CoreSoilType.MoltenStone, CoreSoilType.Stone)]
        [SurfaceSoilType(SurfaceSoilType.None)]
        [ResourceAttribute(ResourceType.Copper, ResourceType.Gold, ResourceType.Iron, ResourceType.Uranium)]
        Gas,
        [CoreSoilType(CoreSoilType.Stone, CoreSoilType.MoltenStone)]
        [SurfaceSoilType(SurfaceSoilType.Lava)]
        [ResourceAttribute(ResourceType.Gold, ResourceType.Mithril, ResourceType.Uranium)]
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