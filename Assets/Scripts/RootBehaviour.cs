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
            var settings = SpaceGenerator.DefaultSpaceSettings();
            settings.Seed = 0;

            Game.SpaceManager = SpaceGenerator.Generate(settings);
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F5)) 
        {
            if (Game.SpaceManager != null)
            {
                Game.SpaceManager.Destroy();

                Game.SpaceManager = SpaceGenerator.Generate(SpaceGenerator.DefaultSpaceSettings());
            }
        }
    }
}
