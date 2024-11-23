using SpaceStand.Planets.Attributes;
using System;

namespace SpaceStand.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class CoreSoilTypeAttribute : Attribute
    {
        public CoreSoilType[] SoilTypes;

        public CoreSoilTypeAttribute(params CoreSoilType[] soilTypes)
        {
            SoilTypes = soilTypes;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class SurfaceSoilTypeAttribute : Attribute
    {
        public SurfaceSoilType[] SoilTypes;

        public SurfaceSoilTypeAttribute(params SurfaceSoilType[] soilTypes)
        {
            SoilTypes = soilTypes;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class MineralTypeAttribute : Attribute
    {
        public MineralsType[] MineralTypes;

        public MineralTypeAttribute(params MineralsType[] mineralTypes)
        {
            MineralTypes = mineralTypes;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class VegitaitionTypeAttribute : Attribute
    {
        public VegitaitionType[] VegitationTypes;

        public VegitaitionTypeAttribute(params VegitaitionType[] vegitationTypes)
        {
            VegitationTypes = vegitationTypes;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ResourceAttribute : Attribute
    {
        public ResourceType[] ResourceTypes;

        public ResourceAttribute(params ResourceType[] resourceTypes)
        {
            ResourceTypes = resourceTypes;
        }
    }
}
