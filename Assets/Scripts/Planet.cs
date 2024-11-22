using UnityEngine;
using SpaceStand.Planets.Attributes;
using System;
using Random = UnityEngine.Random;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class Planet : SpaceObject
{
    public Planet(Vector2 position, SpaceObjectVisual visual) : base(position, visual)
    {

    }
}


public class PlanetAttributes : ISpaceObjectAttributes
{
    private string name;
    private int temperature;

    private PlanetSize size;
    private PlanetType type;
    private ClimateType climate;
    private SoilType coreSoil;
    private SoilType surfaceSoil;
    private MineralsType minerals;
    private VegitaitionType vegitation;

    public PlanetAttributes()
    {
        PlanetGenerationSettingsSO planetGenerationSettings = SpaceGenerator.Instance.PlanetGenerationSettings;

        temperature = Random.Range(planetGenerationSettings.MinTemperature, planetGenerationSettings.MaxTemperature + 1);
        size = (PlanetSize)Random.Range(0, 3);
        type = (PlanetType)Random.Range(1, 5);

        if (temperature >= 110)
        {
            type = PlanetType.Lava;
        }

        switch (type)
        {
            case PlanetType.Terrestrial:
                if (temperature >= 80)
                {
                    climate = ClimateType.Volcanic;
                }
                else if (temperature >= -10)
                {
                    climate = ClimateType.Dry;
                }
                else
                {
                    climate = ClimateType.Frozen;
                }
                break;

            case PlanetType.Ocean:
                if (temperature >= 80)
                {
                    climate = ClimateType.Volcanic;
                }
                else if ( temperature >= -10)
                {
                    climate = ClimateType.Temperate;
                }
                else
                {
                    climate = ClimateType.Frozen;
                }
                break;

            case PlanetType.Gas:
                climate = ClimateType.Dry;
                break;

            case PlanetType.Lava:
                climate = ClimateType.Volcanic;
                break;

            case PlanetType.EarthLike:
                if (temperature >= 80)
                {
                    climate = ClimateType.Volcanic;
                }
                else if (temperature >= 40)
                {
                    climate = ClimateType.Dry;
                }
                else if (temperature >= 20)
                {
                    climate = ClimateType.Tropical;
                }
                else if (temperature >= -10)
                {
                    climate = ClimateType.Temperate;
                }
                else
                {
                    climate = ClimateType.Frozen;
                }
                break;
        }

        byte r;

        r = type switch
        {
            PlanetType.EarthLike => (byte)(SoilType.MotenStone | SoilType.Stone),
            PlanetType.Gas => (byte)(SoilType.None),
            PlanetType.Lava => (byte)(SoilType.MotenStone | SoilType.Stone),
            PlanetType.Ocean => (byte)(SoilType.Stone),
            PlanetType.Terrestrial => (byte)(SoilType.MotenStone | SoilType.Stone),
        };

        //coreSoil = (SoilType)GetRandomEnumValue(r);
    }

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

    public void GetAttributes()
    {

    }

    public string GetName()
    {
        return name;
    }

    public string GetAttributesAsString()
    {
        return string.Empty;
    }
}
