using UnityEngine;
using SpaceStand.Planets.Attributes;
using Random = UnityEngine.Random;
using SpaceStand.Attributes;
using Newtonsoft.Json;
using System.Linq;

public class Planet : SpaceObject
{
    public int Seed { get; set; }
    public float Radius { get; set; }
    public string Name { get { return ((PlanetAttributes)attributes).Name; } set { ((PlanetAttributes)attributes).Name = value; } }

    private bool _isSelected = false;

    public bool IsSelected 
    { 
        get 
        { 
            return _isSelected; 
        } 
        set 
        { 
            _isSelected = value;
            TogglePlanetPointer();
        } 
    }

    public Planet(Vector2 position, SpaceObjectVisual visual) : base(position, visual)
    {
        attributes = new PlanetAttributes();
        JSonData = JsonConvert.SerializeObject(attributes);
    }

    public Planet() : base()
    {
        attributes = new PlanetAttributes();
        attributes.GetAttributesAsJson();
    }

    private void TogglePlanetPointer()
    {

    }

    public override void Clicked(SpaceObjectClickEvent e)
    {
        if(!IsSelected)
        {
            IsSelected = true;
        }

        base.Clicked(e);
    }
}


public class PlanetAttributes : ISpaceObjectAttributes
{
    public string Name;
    public string Description;
    public int Temperature;

    public PlanetSize Size;
    public PlanetType Type;
    public ClimateType Climate;
    public CoreSoilType CoreSoil;
    public SurfaceSoilType SurfaceSoil;
    public ResourceType[] PlanetResources;
    public VegitaitionType Vegitation;

    public PlanetAttributes()
    {
        Name = SpaceGenerator.GeneratePlanetName(new RandomGenerator());

        Temperature = Random.Range(PlanetGenerationSettingsSO.MinTemperature, PlanetGenerationSettingsSO.MaxTemperature + 1);
        Size = (PlanetSize)Random.Range(0, 3);
        Type = (PlanetType)Random.Range(1, 5);

        if (Temperature >= 110)
        {
            Type = PlanetType.Lava;
        }

        switch (Type)
        {
            case PlanetType.Terrestrial:
                if (Temperature >= 80)
                {
                    Climate = ClimateType.Volcanic;
                }
                else if (Temperature >= -10)
                {
                    Climate = ClimateType.Dry;
                }
                else
                {
                    Climate = ClimateType.Frozen;
                }
                break;

            case PlanetType.Ocean:
                if (Temperature >= 80)
                {
                    Climate = ClimateType.Volcanic;
                }
                else if ( Temperature >= -10)
                {
                    Climate = ClimateType.Temperate;
                }
                else
                {
                    Climate = ClimateType.Frozen;
                }
                break;

            case PlanetType.Gas:
                Climate = ClimateType.Dry;
                break;

            case PlanetType.Lava:
                Climate = ClimateType.Volcanic;
                break;

            case PlanetType.EarthLike:
                if (Temperature >= 80)
                {
                    Climate = ClimateType.Volcanic;
                }
                else if (Temperature >= 40)
                {
                    Climate = ClimateType.Dry;
                }
                else if (Temperature >= 20)
                {
                    Climate = ClimateType.Tropical;
                }
                else if (Temperature >= -10)
                {
                    Climate = ClimateType.Temperate;
                }
                else
                {
                    Climate = ClimateType.Frozen;
                }
                break;
        }

        var coreSoilTypes = Type.GetAttribute<CoreSoilTypeAttribute>().SoilTypes;
        int length = coreSoilTypes.Length;

        CoreSoil = coreSoilTypes[Random.Range(0, length)];

        var surfaceSoilTypes = Type.GetAttribute<SurfaceSoilTypeAttribute>().SoilTypes;
        length = surfaceSoilTypes.Length;

        SurfaceSoil = surfaceSoilTypes[Random.Range(0, length)];


        var resourceTypes = Type.GetAttribute<ResourceAttribute>().ResourceTypes.ToList();
        var count = resourceTypes.Count;

        int numResources;
        if (CoreSoil == CoreSoilType.MoltenStone)
        {
            numResources = 3;
        }
        else
        {
            numResources = Random.Range(2, 4);
        }

        PlanetResources = new ResourceType[numResources];
        int r;
        for (int i = 0; i < numResources; i++)
        {
            r = Random.Range(0, count);
            
            PlanetResources[i] = resourceTypes[r];
            resourceTypes.RemoveAt(r);
            count--; 
        }

        var vegitationTypes = Climate.GetAttribute<VegitaitionTypeAttribute>().VegitationTypes;
        length = vegitationTypes.Length;

        Vegitation = vegitationTypes[Random.Range(0, length)];

        string resourcesText = "Planet resources: \n";
        
        for (int i = 0; i < PlanetResources.Length; i++)
        {
            resourcesText += PlanetResources[i] + "\n";
        }


        Description = $"Temperature: {Temperature}\n" +
            $"Size: {Size.ToString()}\n" +
            $"PlanetType: {Type}\n" +
            $"Climate: {Climate}\n" +
            $"Surface soil: {SurfaceSoil}\n" +
            $"Core soil: {CoreSoil}\n" + 
            resourcesText +
            //$"Resource density: {Minerals}\n" +
            $"Vegitation: {Vegitation}";
    }

    public string GetName()
    {
        return Name;
    }

    public string GetAttributesAsJson()
    {
        return JsonUtility.ToJson(this);
    }

    public string GetDescription()
    {
        return Description;
    }
}
