using UnityEngine;

public class RootBehaviour : ObjectBehaviour
{
    private const int TargetFrameRate = 60;

    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = TargetFrameRate;

        if (Game.SpaceManager == null)
        {
            var settings = new SpaceGenerator.SpaceSettings();

            //settings.MinPlanetCount = settings.MaxPlanetCount = 1;
            //settings.SpaceSize = 1;
            settings.Seed = 1024;

            Game.SpaceManager = SpaceGenerator.Generate(settings);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            if (Game.SpaceManager != null)
            {
                Game.SpaceManager.Destroy();

                var settings = new SpaceGenerator.SpaceSettings();

                //settings.MinPlanetCount = settings.MaxPlanetCount = 1;
                //settings.SpaceSize = 1;

                Game.SpaceManager = SpaceGenerator.Generate(settings);
            }
        }
    }
}
