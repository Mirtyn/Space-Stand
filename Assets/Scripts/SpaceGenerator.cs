using NUnit.Framework.Internal;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class SpaceGenerator
{
    private const string _rootGameObjectName = "Root";
    private static string _planetRootPrefabName = "PlanetRootPrefab";
    private static string _moonRootPrefabName = "MoonRootPrefab";
    private static string _asteroidRootPrefabName = "AsteroidRootPrefab";
    private static IcosahedronGenerator _icosahedronGenerator = new IcosahedronGenerator();
    
    private static float[] _planetHues = new[]
    {
        107f,
        50f,
        10f,
        290f,
        90f,
    };

    private static Color32[] _planetColors = new[]
    {
        new Color32(50, 102, 0, 255),
        new Color32(209, 157, 0, 255),
        new Color32(186, 66, 0, 255),
        new Color32(0, 114, 186, 255),
        new Color32(177, 0, 42, 255),
    };

    private static Color32[] _moonColors = new[]
    {
        new Color32(251, 214, 222, 255),
        new Color32(254, 222, 191, 255),
        new Color32(236, 199, 185, 255),
        new Color32(185, 236, 232, 255),
        new Color32(217, 185, 236, 255),
    };

    private static Color32[] _asteroidColors = new[]
    {
        new Color32(50, 102, 0, 255),
        new Color32(209, 157, 0, 255),
        new Color32(186, 66, 0, 255),
        new Color32(0, 114, 186, 255),
        new Color32(177, 0, 42, 255),
    };

    public class SpaceSettings
    {
        public int Seed { get; set; }
        public int MinPlanetCount { get; set; } = 1;
        public int MaxPlanetCount { get; set; } = 1;
        public float MinPlanetRadius { get; set; } = 10;
        public float MaxPlanetRadius { get; set; } = 24;

        public SpaceSettings()
        {
            Seed = RandomGenerator.RandomSeed();
        }
    }

    public class IcosahedronSettings
    {
        public GameObject Parent { get; set; }
        public float MinRadius { get; set; }
        public float MaxRadius { get; set; }
        public float MinOffset { get; set; }
        public float MaxOffset { get; set; }
        public bool AsignRandomColor { get; set; }
        public int SubdivideSteps { get; set; }
        public float MinRotateSpeed { get; set; } = 0f;
        public float MaxRotateSpeed { get; set; } = 10f;        
        public Color32[] Colors { get; set; }
    }

    public class PlanetSettings : IcosahedronSettings
    {
    }

    public class MoonSettings : IcosahedronSettings
    {
        public int Count { get; set; }
    }

    public class AsteroidSettings : MoonSettings
    {
    }

    public class Result
    {
        public SpaceManager Manager { get; private set; } = new SpaceManager();
    }

    public static SpaceManager Generate(SpaceSettings settings)
    {
        var spaceManager = new SpaceManager();

        spaceManager.Space = new Space(2000, 500);
        
        var random = new RandomGenerator(settings.Seed);

        var planetCount = random.Int(settings.MinPlanetCount, settings.MaxPlanetCount);

        var root = GameObject.Find(_rootGameObjectName);

        GeneratePlanets(
            planetCount, 
            new PlanetSettings()
            {
                Parent = root,
                MinRadius = settings.MinPlanetRadius,
                MaxRadius = settings.MaxPlanetRadius,
                MinOffset = spaceManager.Space.Size * -0.45f,
                MaxOffset = spaceManager.Space.Size *  0.45f,
                AsignRandomColor = true,
                SubdivideSteps = 1,
                Colors = _planetColors,
            }, 
            spaceManager);

        return spaceManager;
    }

    public static void GeneratePlanets(int count, PlanetSettings settings, SpaceManager spaceManager)
    {
        var _planetRootPrefab = Resources.Load<GameObject>(_planetRootPrefabName);

        for (var i = 0; i < count; i++)
        {
            GeneratePlanet(settings, _planetRootPrefab, spaceManager);
        }
    }

    public static void GeneratePlanet(PlanetSettings settings, GameObject prefab, SpaceManager spaceManager)
    {
        var random = new RandomGenerator();        

        var position = new Vector3(random.Value(settings.MinOffset, settings.MaxOffset), random.Value(settings.MinOffset, settings.MaxOffset), 0);

        var planetGameObject = UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity);

        var spaceObjectVisual = planetGameObject.GetComponent<SpaceObjectVisual>();

        var planet = new Planet();

        planet.Radius = random.Value(settings.MinRadius, settings.MaxRadius);
        planet.Seed = random.Seed;
        planet.position = position;
        planet.visual = spaceObjectVisual;

        spaceObjectVisual.SetSpaceObject(planet);

        var meshFilter = planetGameObject.GetComponent<MeshFilter>();

        GenerateIcosahedronMesh(settings, meshFilter, random);

        planetGameObject.name = planet.name;
        planetGameObject.transform.SetParent(settings.Parent.transform, false);

        AssignMaterialColor(settings, planetGameObject, random);

        var rotateBehaviour = planetGameObject.GetComponent<RotateBehaviour>();

        var axisAngle = random.Value(0, Mathf.PI * 2f);

        rotateBehaviour.Axis = new Vector3(Mathf.Sin(axisAngle), Mathf.Cos(axisAngle), 0f);
        rotateBehaviour.Speed = random.Value(settings.MinRotateSpeed, settings.MaxRotateSpeed);



        var _moonRootPrefab = Resources.Load<GameObject>(_moonRootPrefabName);

        GenerateMoons(new MoonSettings
        {
            Parent = planetGameObject,
            Count = random.Value(1, 5),
            MinOffset = planet.Radius * 2.25f,
            MaxOffset = planet.Radius * 3.75f,
            MinRadius = planet.Radius * 0.15f,
            MaxRadius = planet.Radius * 0.30f,
            Colors = _moonColors
        }, _moonRootPrefab, spaceManager);



        var _asteroidRootPrefab = Resources.Load<GameObject>(_asteroidRootPrefabName);

        GenerateMoons(new MoonSettings
        {
            Parent = planetGameObject,
            Count = random.Value(4, 16),
            MinOffset = planet.Radius * 1.25f,
            MaxOffset = planet.Radius * 1.75f,
            MinRadius = planet.Radius * 0.05f,
            MaxRadius = planet.Radius * 0.10f,
            Colors = _asteroidColors
        }, _asteroidRootPrefab, spaceManager);
    }

    public static void GenerateMoons(MoonSettings settings, GameObject prefab, SpaceManager spaceManager)
    {
        for (var i = 0; i < settings.Count; i++)
        {
            GenerateMoon(settings, prefab, spaceManager);
        }
    }

    public static void GenerateMoon(MoonSettings settings, GameObject prefab, SpaceManager spaceManager)
    {
        var random = new RandomGenerator();

        var parentRotateBehaviour = settings.Parent.GetComponent<RotateBehaviour>();

        var a = Vector3.Cross(parentRotateBehaviour.Axis, Vector3.back);

        a = Quaternion.AngleAxis(random.Value(0, 360), parentRotateBehaviour.Axis) * a;

        var position = a * random.Value(settings.MinOffset, settings.MaxOffset);

        var planetGameObject = UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity);

        var spaceObjectVisual = planetGameObject.GetComponent<SpaceObjectVisual>();

        var planet = new Planet();

        planet.Radius = random.Value(settings.MinRadius, settings.MaxRadius);
        planet.Seed = random.Seed;
        planet.position = position;
        planet.visual = spaceObjectVisual;

        spaceObjectVisual.SetSpaceObject(planet);

        var meshFilter = planetGameObject.GetComponent<MeshFilter>();

        GenerateIcosahedronMesh(settings, meshFilter, random);

        planetGameObject.name = planet.name;
        planetGameObject.transform.SetParent(settings.Parent.transform, false);
        //planetGameObject.transform.localScale = new Vector3(planet.Radius, planet.Radius, planet.Radius);

        AssignMaterialColor(settings, planetGameObject, random);

        var rotateBehaviour = planetGameObject.GetComponent<RotateBehaviour>();

        rotateBehaviour.enabled = false;

        //var axisAngle = random.Value(0, Mathf.PI * 2f);

        //rotateBehaviour.Axis = new Vector3(Mathf.Sin(axisAngle), Mathf.Cos(axisAngle), 0f);
        //rotateBehaviour.Speed = random.Value(settings.MinRotateSpeed, settings.MaxRotateSpeed);
    }

    private static void AssignMaterialColor(IcosahedronSettings settings, GameObject gameObject, RandomGenerator random)
    {
        var material = new Material(gameObject.GetComponent<MeshRenderer>().material);

        //var hue = _planetHues[random.Value(0, _planetHues.Length)];

        //var color = Color.HSVToRGB(random.Value((hue - 10f) / 360f, (hue + 10f) / 360f), random.Value(0.8f, 1f), random.Value(0.2f, 0.5f));

        var color = settings.Colors[random.Value(0, _planetHues.Length)];

        material.SetColor("_BaseColor", color);

        gameObject.GetComponent<MeshRenderer>().material = material;
    }

    private static void GenerateIcosahedronMesh(IcosahedronSettings settings, MeshFilter meshFilter, RandomGenerator random)
    {
        var icosahedronGenerator = new IcosahedronGenerator();

        if (settings.SubdivideSteps > 0)
        {
            icosahedronGenerator.Subdivide(settings.SubdivideSteps);
        }

        int vertexCount = icosahedronGenerator.Polygons.Count * 3;
        int[] triangles = new int[vertexCount];

        Vector3[] vertices = new Vector3[vertexCount];

        for (int i = 0; i < icosahedronGenerator.Polygons.Count; i++)
        {
            var poly = icosahedronGenerator.Polygons[i];

            triangles[i * 3 + 0] = i * 3 + 0;
            triangles[i * 3 + 1] = i * 3 + 1;
            triangles[i * 3 + 2] = i * 3 + 2;

            vertices[i * 3 + 0] = icosahedronGenerator.Vertices[poly.vertices[0]];
            vertices[i * 3 + 1] = icosahedronGenerator.Vertices[poly.vertices[1]];
            vertices[i * 3 + 2] = icosahedronGenerator.Vertices[poly.vertices[2]];
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            var o = new Vector3(vertices[i].x, vertices[i].y, vertices[i].z);

            var t = o.normalized * random.Value(settings.MinRadius, settings.MaxRadius);

            //var t = new Vector3(random.Value(-0.1f, 0.1f), random.Value(-0.1f, 0.1f), random.Value(-0.1f, 0.1f));

            for (int j = 0; j < vertices.Length; j++)
            {
                if (o.x == vertices[j].x && o.y == vertices[j].y && o.z == vertices[j].z)
                {
                    vertices[j].x = t.x;
                    vertices[j].y = t.y;
                    vertices[j].z = t.z;
                }
            }
        }

        var mesh = meshFilter.mesh;

        mesh.Clear();

        mesh.vertices = vertices;
        mesh.SetTriangles(triangles, 0);

        //mesh.Optimize();
        mesh.RecalculateNormals();
    }


    //private List<SpaceObject> ScatterPlanets(int number)
    //{
    //    List<SpaceObject> spaceObjects = new List<SpaceObject>();

    //    for (int i = 0; i < number; i++)
    //    {
    //        Vector2 pos = new Vector2(Random.Range(0, Space.Instance.Width), Random.Range(0, Space.Instance.Height));

    //        var obj = GameObject.Instantiate(planetPrefab, pos, Quaternion.identity, Space.Instance.transform);
    //        var spaceObjVisual = obj.GetComponent<SpaceObjectVisual>();

    //        var planet = new Planet(pos, spaceObjVisual);
    //        spaceObjects.Add(planet);

    //        spaceObjVisual.SetSpaceObject(planet);
    //    }

    //    return spaceObjects;
    //}
}
