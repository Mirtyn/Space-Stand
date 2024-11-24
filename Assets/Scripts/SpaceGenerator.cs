using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public static class SpaceGenerator
{
    private const string _rootGameObjectName = "Root";
    private static string _planetRootPrefabName = "PlanetRootPrefab";
    private static string _moonRootPrefabName = "MoonRootPrefab";
    private static string _asteroidRootPrefabName = "AsteroidRootPrefab";
    private static string _gridCrossHairPrefabName = "GridCrossHairPrefab";
    private static string _starPrefabName = "StarPrefab";
    private static string _spaceBackgroundGameObjectName = "SpaceBackground";
    private static IcosahedronGenerator _icosahedronGenerator = new IcosahedronGenerator();
    
    private static float[] _planetHues = new[]
    {
        107f,
        50f,
        10f,
        290f,
        90f,
    };

    private static Color32 Color32From(int color)
    {
        return new Color32(
            (byte)(color >> 16 & 0xff),
            (byte)(color >> 8 & 0xff),
            (byte)(color & 0xff),
            255);
    }

    private static Color32[] _planetColors = new[]
    {
        new Color32(50, 102, 0, 255),
        new Color32(209, 157, 0, 255),
        new Color32(186, 66, 0, 255),
        new Color32(0, 114, 186, 255),
        new Color32(177, 0, 42, 255),
        Color32From(0xFFA0FB),
        Color32From(0x9666FF),
        Color32From(0x89FFFD),
        Color32From(0x3A6BFF),
        Color32From(0x420d0d),
        Color32From(0x324c15),
        Color32From(0x028b02),
        Color32From(0x056655),
        //Color32From(0xFFA0FB),
        //Color32From(0xFFA0FB),
        //Color32From(0xFFA0FB),
        //Color32From(0xFFA0FB),
        //Color32From(0xFFA0FB),
        //Color32From(0xFFA0FB),
        //Color32From(0xFFA0FB),
        //Color32From(0xFFA0FB),
    };

    private static Color32[] _starColors = new[]
    {
        new Color32(251, 214, 222, 255),
        new Color32(254, 222, 191, 255),
        new Color32(236, 199, 185, 255),
        new Color32(185, 236, 232, 255),
        new Color32(217, 185, 236, 255),
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
        public int MinPlanetCount { get; set; } = 50;
        public int MaxPlanetCount { get; set; } = 100;
        public float MinPlanetRadius { get; set; } = 2;
        public float MaxPlanetRadius { get; set; } = 32;
        public int SpaceSize { get; set; } = 4000;
        public float MinPlanetRotateSpeed { get; set; } = 2f;
        public float MaxPlanetRotateSpeed { get; set; } = 24f;

        public int GridStepSize { get; set; } = 250;

        public int StarCount { get; set; } = 4000;

        public SpaceSettings()
        {
            Seed = RandomGenerator.RandomSeed();
        }
    }

    public class IcosahedronSettings
    {
        public GameObject Parent { get; set; }
        public float Radius { get; set; }
        public float RadiusOffset { get; set; } = 0.1f; // 10%
        public int SubdivideSteps { get; set; }
        public float RotateSpeed { get; set; }
        public float Offset { get; set; }

        public Color32[] Colors { get; set; }
    }

    public class PlanetSettings : IcosahedronSettings
    {
        public Vector3 Position { get; set; }
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

        GenerateGrid(
            root, 
            settings, 
            spaceManager, 
            random);

        GenerateStars(
            root,
            settings,
            spaceManager,
            random);

        ScaleBackground(root, settings);

        return spaceManager;
    }

    private static void ScaleBackground(GameObject root, SpaceSettings settings)
    {
        var background = GameObject.Find(_spaceBackgroundGameObjectName);
        background.transform.localScale = new Vector3(settings.SpaceSize, settings.SpaceSize, 1);
    }

    private static void GenerateStars(GameObject parent, SpaceSettings settings, SpaceManager spaceManager, RandomGenerator random)
    {
        var _starPrefab = Resources.Load<GameObject>(_starPrefabName);

        for (var j = 0; j < settings.StarCount; j++)
        {
            var position = new Vector3(
                random.Value(spaceManager.Space.Size * -0.5f, random.Value(spaceManager.Space.Size * 0.5f)),
                random.Value(spaceManager.Space.Size * -0.5f, random.Value(spaceManager.Space.Size * 0.5f)),
                0f);

            var starGameObject = UnityEngine.Object.Instantiate(_starPrefab, position, Quaternion.identity);

            var index = random.Int(0, starGameObject.transform.childCount);

            var child = starGameObject.transform.GetChild(index);
            
            child.gameObject.SetActive(true);

            var material = new Material(child.GetComponent<LineRenderer>().material);

            var color = _starColors[random.Value(0, _starColors.Length)];

            color.a = random.Byte(10, 128);

            material.SetColor("_BaseColor", color);

            child.GetComponent<LineRenderer>().material = material;

            starGameObject.transform.SetParent(parent.transform, false);
        }
    }

    private static void GenerateGrid(GameObject parent, SpaceSettings settings, SpaceManager spaceManager, RandomGenerator random)
    {
        var _gridCrossHairPrefab = Resources.Load<GameObject>(_gridCrossHairPrefabName);

        var steps = 1 + settings.SpaceSize / settings.GridStepSize;
        var y = settings.SpaceSize * -0.5f;

        for(var j = 0; j < steps; j++)
        {
            var x = settings.SpaceSize * -0.5f;

            for (var i = 0; i < steps; i++)
            {
                var gridCrossHairGameObject = UnityEngine.Object.Instantiate(_gridCrossHairPrefab, new Vector3(x, y, 0), Quaternion.identity);

                gridCrossHairGameObject.transform.SetParent(parent.transform, false);

                x += settings.GridStepSize;
            }

            y += settings.GridStepSize;
        }
    }

    private static void GeneratePlanets(int count, GameObject parent, SpaceSettings settings, SpaceManager spaceManager, RandomGenerator random)
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
                    RadiusOffset = random.Value(0.10f, 0.25f),
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

    private static void GeneratePlanet(PlanetSettings settings, GameObject prefab, SpaceManager spaceManager, RandomGenerator random)
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


        //float angle1 = Radian();
        //float angle2 = Radian();

        //x = Mathf.Sin(angle1) * Mathf.Cos(angle2);
        //y = Mathf.Sin(angle1) * Mathf.Sin(angle2);
        //z = Mathf.Cos(angle1);


        var axisAngle = random.Value(0, Mathf.PI * 2f);

        rotateBehaviour.Axis = new Vector3(Mathf.Sin(axisAngle), Mathf.Cos(axisAngle), 0f);
        rotateBehaviour.Speed = settings.RotateSpeed;



        var _moonRootPrefab = Resources.Load<GameObject>(_moonRootPrefabName);

        GenerateMoons(planetGameObject, planet, random.Value(1, 5), _moonRootPrefab, spaceManager, random);



        var _asteroidRootPrefab = Resources.Load<GameObject>(_asteroidRootPrefabName);

        GenerateAsteroids(planetGameObject, planet, random.Value(4, 16), _asteroidRootPrefab, spaceManager, random);



        random.Pop();
    }

    private static void GenerateMoons(
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

            GenerateIcosahedron(
                new IcosahedronSettings
                {
                    Parent = parent,
                    Offset =  random.Value(planet.Radius * 2.25f, planet.Radius * 3.75f),
                    Radius = random.Value(planet.Radius * 0.15f, planet.Radius * 0.30f),
                    RadiusOffset = random.Value(0.05f, 0.15f),
                    Colors = _moonColors,
                    SubdivideSteps = 0,
                },
                prefab, 
                spaceManager, 
                random);

            random.Pop();
        }
    }

    private static void GenerateAsteroids(
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

            GenerateAsteroid(
                new IcosahedronSettings
                {
                    Parent = parent,
                    Offset = random.Value(planet.Radius * 1.15f, planet.Radius * 1.75f),
                    Radius = random.Value(planet.Radius * 0.05f, planet.Radius * 0.15f),
                    RadiusOffset = random.Value(0.10f, 0.25f),
                    Colors = _asteroidColors,
                    SubdivideSteps = 0,
                },
                prefab,
                spaceManager,
                random);

            random.Pop();
        }
    }

    private static void GenerateAsteroid(IcosahedronSettings settings, GameObject prefab, SpaceManager spaceManager, RandomGenerator random)
    {
        var planetGameObject = GenerateIcosahedron(settings, prefab, spaceManager, random);

        var parentRotateBehaviour = settings.Parent.GetComponent<RotateBehaviour>();

        var rotateBehaviour = planetGameObject.GetComponent<RotateBehaviour>();

        rotateBehaviour.enabled = true;

        var axisAngle = random.Value(0, Mathf.PI * 2f);

        rotateBehaviour.Point = parentRotateBehaviour.Point;
        rotateBehaviour.Axis = new Vector3(Mathf.Sin(axisAngle), Mathf.Cos(axisAngle), 0f);
        rotateBehaviour.Speed = settings.RotateSpeed;
    }

    private static GameObject GenerateIcosahedron(IcosahedronSettings settings, GameObject prefab, SpaceManager spaceManager, RandomGenerator random)
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

        return planetGameObject;
    }

    private static void AssignMaterialColor(IcosahedronSettings settings, GameObject gameObject, RandomGenerator random)
    {
        var material = new Material(gameObject.GetComponent<MeshRenderer>().material);

        var color = settings.Colors[random.Value(0, settings.Colors.Length)];

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

            var t = o.normalized * (settings.Radius - (settings.Radius * random.Value(settings.RadiusOffset)));

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
