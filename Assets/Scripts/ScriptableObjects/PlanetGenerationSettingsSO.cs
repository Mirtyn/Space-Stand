using SpaceStand.Planets.Attributes;
using UnityEngine;

[CreateAssetMenu]
public class PlanetGenerationSettingsSO : ScriptableObject
{
    public int MinTemperature;
    public int MaxTemperature;

    public string[] PlanetNameStart = "A B C D E F G H I J K L M N O P Q R S T U V W X Y Z".Split(' ');
    public string[] PlanetNameEnd = "0 1 2 3 4 5 6 7 8 9".Split(' ');
}
