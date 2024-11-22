using System.Collections.Generic;
using UnityEngine;

public class SpaceGenerator : ProjectBehaviour
{
    public static SpaceGenerator Instance { get ; private set; }

    [SerializeField] private GameObject blackHolePrefab;
    [SerializeField] private GameObject starPrefab;
    [SerializeField] private GameObject planetPrefab;
    [SerializeField] private GameObject astroidPrefab;

    [SerializeField] private SpaceLayersSO spaceLayersSO;
    public PlanetGenerationSettingsSO PlanetGenerationSettings;

    private void Awake()
    {
        Instance = this;
        ScatterPlanets(50);
    }

    private List<SpaceObject> ScatterPlanets(int number)
    {
        List<SpaceObject> spaceObjects = new List<SpaceObject>();

        for (int i = 0; i < number; i++)
        {
            Vector2 pos = new Vector2(Random.Range(0, Space.Instance.Width), Random.Range(0, Space.Instance.Height));

            var obj = GameObject.Instantiate(planetPrefab, pos, Quaternion.identity, Space.Instance.transform);
            var spaceObjVisual = obj.GetComponent<SpaceObjectVisual>();

            var planet = new Planet(pos, spaceObjVisual);
            spaceObjects.Add(planet);

            spaceObjVisual.SetSpaceObject(planet);
        }

        return spaceObjects;
    }

    private List<SpaceObject> GenerateLayer(GenerationLayer generationLayer, float distance, int numberObjects)
    {
        List<SpaceObject> spaceObjects = new List<SpaceObject>();

        GameObject prefab = generationLayer.Type switch
        {
            SpaceObjectType.Planet => planetPrefab,
            SpaceObjectType.Astroid => astroidPrefab,
            SpaceObjectType.Blackhole => blackHolePrefab,
            SpaceObjectType.Star => starPrefab,
        };

        for (int i = 0; i < numberObjects; i++)
        {

        }

        return spaceObjects;
    }

    private void GenerateLayer(GenerationLayer generationLayer, Vector2 centerPoint)
    {

    }
}
