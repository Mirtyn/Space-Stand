using UnityEngine;

public class GameManagerBehaviour : ObjectBehaviour
{
    private const int TargetFrameRate = 60;

    void Awake()
    {
        QualitySettings.vSyncCount = 0;

        Application.targetFrameRate = TargetFrameRate;

        Game.Initialize();

        Debug.Log($"Screen resolution: {Screen.width}x${Screen.height}");
    }

    void Start()
    {

    }

    void Update()
    {
        Game.Update();

        if (Input.GetKeyDown(KeyCode.F5))
        {
            if (Game.SpaceManager != null)
            {
                Game.SpaceManager.DestroyRootChildren();

                var settings = new SpaceGenerator.SpaceSettings();

                //settings.MinPlanetCount = settings.MaxPlanetCount = 1;
                //settings.SpaceSize = 1;

                Game.SpaceManager = SpaceGenerator.Generate(settings);
            }
        }
    }
}
