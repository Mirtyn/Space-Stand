using NUnit.Framework.Internal;
using System.Collections.Generic;
using System.Xml.Linq;
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
        public float MinPlanetRadius { get; set; } = 4;
        public float MaxPlanetRadius { get; set; } = 32;
        public int SpaceSize { get; set; } = 4000;
        public float MinPlanetRotateSpeed { get; set; } = 2f;
        public float MaxPlanetRotateSpeed { get; set; } = 12f;

        public SpaceSettings()
        {
            Seed = 0; // RandomGenerator.RandomSeed();
        }
    }

    public static SpaceSettings DefaultSpaceSettings()
    {
        return new SpaceSettings()
        {
            Seed = 0,
            SpaceSize = 5000,
            MinPlanetCount = 50,
            MaxPlanetCount = 100,
            MinPlanetRadius = 8f,
            MaxPlanetRadius = 24f,
            MinPlanetRotateSpeed = 2f,
            MaxPlanetRotateSpeed = 12f,
        };
    }

    public class IcosahedronSettings
    {
        public GameObject Parent { get; set; }
        public float Radius { get; set; }
        public float RadiusOffset { get; set; } = 0.1f; // 10%
        public int SubdivideSteps { get; set; }
        public float RotateSpeed { get; set; }

        public Color32[] Colors { get; set; }
    }

    public class PlanetSettings : IcosahedronSettings
    {
        public Vector3 Position { get; set; }
    }

    public class MoonSettings : IcosahedronSettings
    {
        public float Offset { get; set; }
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
        var spaceManager = new SpaceManager
        {
            Space = new Space(settings.SpaceSize, settings.Seed),
        };
        
        var random = new RandomGenerator(settings.Seed);

        var planetCount = random.Int(settings.MinPlanetCount, settings.MaxPlanetCount);

        var root = GameObject.Find(_rootGameObjectName);

        GeneratePlanets(
            planetCount,
            root,
            settings,
            spaceManager,
            random);

        return spaceManager;
    }

    public static void GeneratePlanets(int count, GameObject parent, SpaceSettings settings, SpaceManager spaceManager, RandomGenerator random)
    {
        var _planetRootPrefab = Resources.Load<GameObject>(_planetRootPrefabName);

        for (var i = 0; i < count; i++)
        {
            random.Push();
            
            GeneratePlanet(
                new PlanetSettings()
                {
                    Parent = parent,
                    Radius = random.Value(settings.MinPlanetRadius, settings.MaxPlanetRadius),
                    Position = new Vector3(
                        random.Value(spaceManager.Space.Size * -0.5f, random.Value(spaceManager.Space.Size * 0.5f)),
                        random.Value(spaceManager.Space.Size * -0.5f, random.Value(spaceManager.Space.Size * 0.5f)),
                        0f),
                    SubdivideSteps = 1,
                    Colors = _planetColors,
                    RotateSpeed = random.Value(settings.MinPlanetRotateSpeed, settings.MaxPlanetRotateSpeed),
                },
                _planetRootPrefab, 
                spaceManager, 
                random);

            random.Pop();
        }
    }

    public static string GeneratePlanetName(RandomGenerator random)
    {
        int nameStartLength = PlanetGenerationSettingsSO.PlanetNameStart.Length;
        int nameEndLength = PlanetGenerationSettingsSO.PlanetNameEnd.Length;

        return PlanetGenerationSettingsSO.PlanetNameStart[random.Int(nameStartLength)] +
            PlanetGenerationSettingsSO.PlanetNameStart[random.Int(0, nameStartLength)] +
            "-" +
            PlanetGenerationSettingsSO.PlanetNameEnd[random.Int(0, nameEndLength)] +
            PlanetGenerationSettingsSO.PlanetNameEnd[random.Int(0, nameEndLength)] +
            PlanetGenerationSettingsSO.PlanetNameEnd[random.Int(0, nameEndLength)];
    }

    public static void GeneratePlanet(PlanetSettings settings, GameObject prefab, SpaceManager spaceManager, RandomGenerator random)
    {
        var planetGameObject = UnityEngine.Object.Instantiate(prefab, settings.Position, Quaternion.identity);

        var spaceObjectVisual = planetGameObject.GetComponent<SpaceObjectVisual>();

        var planet = new Planet();

        planet.Name = GeneratePlanetName(random);
        planet.Radius = settings.Radius;
        planet.Seed = random.Seed;
        planet.position = settings.Position;
        planet.visual = spaceObjectVisual;

        spaceObjectVisual.SetSpaceObject(planet);

        var meshFilter = planetGameObject.GetComponent<MeshFilter>();

        var circleCollider2D = planetGameObject.GetComponent<CircleCollider2D>();
        circleCollider2D.radius = GenerateIcosahedronMesh(settings, meshFilter, random) * 1.5f;

        planetGameObject.name = planet.name;
        planetGameObject.transform.SetParent(settings.Parent.transform, false);

        AssignMaterialColor(settings, planetGameObject, random);

        var rotateBehaviour = planetGameObject.GetComponent<RotateBehaviour>();

        var axisAngle = random.Value(0, Mathf.PI * 2f);

        rotateBehaviour.Axis = new Vector3(Mathf.Sin(axisAngle), Mathf.Cos(axisAngle), 0f);
        rotateBehaviour.Speed = settings.RotateSpeed;



        var _moonRootPrefab = Resources.Load<GameObject>(_moonRootPrefabName);

        GenerateMoons(planetGameObject, planet, random.Value(1, 5), _moonRootPrefab, spaceManager, random);



        var _asteroidRootPrefab = Resources.Load<GameObject>(_asteroidRootPrefabName);

        GenerateAsteroids(planetGameObject, planet, random.Value(4, 16), _asteroidRootPrefab, spaceManager, random);



        random.Pop();
    }

    public static void GenerateMoons(
        GameObject parent, 
        Planet planet, 
        int count, 
        GameObject prefab, 
        SpaceManager spaceManager, 
        RandomGenerator random)
    {
        for (var i = 0; i < count; i++)
        {
            random.Push();

            GenerateMoon(
                new MoonSettings
                {
                    Parent = parent,
                    Offset =  random.Value(planet.Radius * 2.25f, planet.Radius * 3.75f),
                    Radius = random.Value(planet.Radius * 0.15f, planet.Radius * 0.30f),
                    Colors = _asteroidColors,
                    SubdivideSteps = 0,
                },
                prefab, 
                spaceManager, 
                random);

            random.Pop();
        }
    }

    public static void GenerateAsteroids(
        GameObject parent,
        Planet planet,
        int count,
        GameObject prefab,
        SpaceManager spaceManager,
        RandomGenerator random)
    {
        for (var i = 0; i < count; i++)
        {
            random.Push();

            GenerateMoon(
                new MoonSettings
                {
                    Parent = parent,
                    Offset = random.Value(planet.Radius * 2.25f, planet.Radius * 3.75f),
                    Radius = random.Value(planet.Radius * 0.15f, planet.Radius * 0.30f),
                    Colors = _asteroidColors,
                    SubdivideSteps = 0,
                },
                prefab,
                spaceManager,
                random);

            random.Pop();
        }
    }

    public static void GenerateMoon(MoonSettings settings, GameObject prefab, SpaceManager spaceManager, RandomGenerator random)
    {
        var parentRotateBehaviour = settings.Parent.GetComponent<RotateBehaviour>();

        var a = Vector3.Cross(parentRotateBehaviour.Axis, Vector3.back);

        a = Quaternion.AngleAxis(random.Value(0, 360), parentRotateBehaviour.Axis) * a;

        var position = a * settings.Offset;

        var planetGameObject = UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity);

        var spaceObjectVisual = planetGameObject.GetComponent<SpaceObjectVisual>();

        var planet = new Planet
        {
            Radius = settings.Radius,
            Seed = random.Seed,
            position = position,
            visual = spaceObjectVisual
        };

        spaceObjectVisual.SetSpaceObject(planet);

        var meshFilter = planetGameObject.GetComponent<MeshFilter>();

        GenerateIcosahedronMesh(settings, meshFilter, random);

        planetGameObject.name = planet.name;
        planetGameObject.transform.SetParent(settings.Parent.transform, false);

        AssignMaterialColor(settings, planetGameObject, random);

        var rotateBehaviour = planetGameObject.GetComponent<RotateBehaviour>();

        rotateBehaviour.enabled = false;
    }

    private static void AssignMaterialColor(IcosahedronSettings settings, GameObject gameObject, RandomGenerator random)
    {
        var material = new Material(gameObject.GetComponent<MeshRenderer>().material);

        var color = settings.Colors[random.Value(0, _planetHues.Length)];

        material.SetColor("_BaseColor", color);

        gameObject.GetComponent<MeshRenderer>().material = material;
    }

    private static float GenerateIcosahedronMesh(IcosahedronSettings settings, MeshFilter meshFilter, RandomGenerator random)
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

        var sqrRadius = 0f;

        for (int i = 0; i < vertices.Length; i++)
        {
            var o = new Vector3(vertices[i].x, vertices[i].y, vertices[i].z);

            var t = o.normalized * (settings.Radius - random.Value(settings.RadiusOffset));

            var r = t.sqrMagnitude;

            if(r > sqrRadius)
            {
                sqrRadius = r;
            }

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

        return Mathf.Sqrt(sqrRadius);
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
