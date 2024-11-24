using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Faction
{

    public int Name { get; set; }
    public FactionTraits Traits { get; set; } = new FactionTraits();
}

public class FactionTraits
{
    public Dictionary<int, FactionPlanetStatus> FactionPlanetStatusDictionary = new Dictionary<int, FactionPlanetStatus>();
}

public class FactionPlanetStatus
{
    public FactionPlanetStatusType Type { get; set; } = FactionPlanetStatusType.Unknown;
}

public enum FactionPlanetStatusType
{
    Unknown,
    Owned,
    AtWar,
}
