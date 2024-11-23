using UnityEngine;
using SpaceStand.Planets.Attributes;
using Random = UnityEngine.Random;
using SpaceStand.Attributes;
using Newtonsoft.Json;

public class Planet : SpaceObject
{
    public int Seed { get; set; }
    public float Radius { get; set; }

    public Planet(Vector2 position, SpaceObjectVisual visual) : base(position, visual)
    {
        attributes = new PlanetAttributes();
        JSonData = JsonConvert.SerializeObject(attributes);
        Debug.Log(JSonData);
        //File.WriteAllText()
    }

    public Planet() : base()
    {
        attributes = new PlanetAttributes();
        attributes.GetAttributesAsJson();
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
    public MineralsType Minerals;
    public VegitaitionType Vegitation;

    public PlanetAttributes()
    {
        //PlanetGenerationSettingsSO planetGenerationSettings = PlanetGenerationSettingsSO.Instance.PlanetGenerationSettings;
        int nameStartLength = PlanetGenerationSettingsSO.PlanetNameStart.Length;
        int nameEndLength = PlanetGenerationSettingsSO.PlanetNameEnd.Length;

        Name = PlanetGenerationSettingsSO.PlanetNameStart[Random.Range(0, nameStartLength)] +
            PlanetGenerationSettingsSO.PlanetNameStart[Random.Range(0, nameStartLength)] +
            "-" +
            PlanetGenerationSettingsSO.PlanetNameEnd[Random.Range(0, nameEndLength)] +
            PlanetGenerationSettingsSO.PlanetNameEnd[Random.Range(0, nameEndLength)] +
            PlanetGenerationSettingsSO.PlanetNameEnd[Random.Range(0, nameEndLength)];

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

        //r = type switch
        //{
        //    PlanetType.EarthLike => (byte)(CoreSoilType.MoltenStone | CoreSoilType.Stone),
        //    PlanetType.Gas => (byte)(CoreSoilType.None),
        //    PlanetType.Lava => (byte)(CoreSoilType.MoltenStone | CoreSoilType.Stone),
        //    PlanetType.Ocean => (byte)(CoreSoilType.Stone),
        //    PlanetType.Terrestrial => (byte)(CoreSoilType.MoltenStone | CoreSoilType.Stone),
        //};
        //coreSoil = (CoreSoilType)GetRandomEnumValue(r);

        var surfaceSoilTypes = Type.GetAttribute<SurfaceSoilTypeAttribute>().SoilTypes;
        length = surfaceSoilTypes.Length;

        SurfaceSoil = surfaceSoilTypes[Random.Range(0, length)];


        var mineralsTypes = Type.GetAttribute<MineralTypeAttribute>().MineralTypes;
        length = mineralsTypes.Length;

        Minerals = mineralsTypes[Random.Range(0, length)];


        var vegitationTypes = Climate.GetAttribute<VegitaitionTypeAttribute>().VegitationTypes;
        length = vegitationTypes.Length;

        Vegitation = vegitationTypes[Random.Range(0, length)];

        Description = $"Temperature: {Temperature}\n" +
            $"Size: {Size.ToString()}\n" +
            $"PlanetType: {Type}\n" +
            $"Climate: {Climate}\n" +
            $"Surface soil: {SurfaceSoil}\n" +
            $"Core soil: {CoreSoil}\n" +
            $"Resource density: {Minerals}\n" +
            $"Vegitation: {Vegitation}";
    }

    /*
    public int GetRandomEnumValue(byte value)
    {
        List<int> values = new List<int>();
        for (int i = 0; i < 8; i++)
        {
            if ((value & (1 << i)) != 0)
            {
                values.Add(i);


            }
        }

        int randomValue = values[(Random.Range(0, values.Count))];
        return randomValue;
    }
    */

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
