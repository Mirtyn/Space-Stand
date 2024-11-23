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
        if (Game.SpaceManager == null)
        {
            Game.SpaceManager = SpaceGenerator.Generate(
                new SpaceGenerator.SpaceSettings()
                {
                    MinPlanetCount = 20,
                    MaxPlanetCount = 50,
                });
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F5)) 
        {
            if (Game.SpaceManager != null)
            {
                Game.SpaceManager.Destroy();

                Game.SpaceManager = SpaceGenerator.Generate(
                    new SpaceGenerator.SpaceSettings()
                    {
                        MinPlanetCount = 20,
                        MaxPlanetCount = 50,
                    });
            }
        }
    }
}
