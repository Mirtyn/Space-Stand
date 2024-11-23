using UnityEngine;

public class RootBehaviour : ObjectBehaviour
{
    private const int TargetFrameRate = 60;

    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = TargetFrameRate;
    }

    void Start()
    {
        if (Game.Space == null)
        {
            var result = PlanetGenerator.Generate(new PlanetGenerator.PlanetSettings());

            Game.Space = result.Space;
        }
    }
}
