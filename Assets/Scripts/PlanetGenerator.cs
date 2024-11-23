using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public static class PlanetGenerator
{
    private const string _rootGameObjectName = "Root";

    private static GameObject _rootGameObject;

    private static string _planetRootPrefabName = "PlanetRootPrefab";

    private static GameObject _planetRootPrefab;

    //private static RandomGenerator _randomGenerator = new RandomGenerator();

    private static float[] _planetHues = new[]
    {
        107f,
        50f,
        10f,
        290f,
        90f,
    };

    public class SpaceSettings
    {

    }

    public class PlanetSettings
    {
        public float MinRadius { get; set; }
        public float MaxRadius { get; set; }
    }

    public class MoonSettings
    {
        public GameObject Planet { get; set; }
        public int Count { get; set; }
        public float MinRadius { get; set; }
        public float MaxRadius { get; set; }
        public float MinOffset { get; set; }
        public float MaxOffset { get; set; }

        public bool AsignRandomColor { get; set; }
    }

    public class AsteroidSettings : MoonSettings
    {
    }

    public class Result
    {
        public SpaceManager Space { get; private set; } = new SpaceManager();
    }

    public static Result Generate(PlanetSettings settings)
    {
        var random = new RandomGenerator();

        _rootGameObject = GameObject.Find(_rootGameObjectName);

        _planetRootPrefab = Resources.Load<GameObject>(_planetRootPrefabName);

        var position = new Vector3(0, 0, 0);

        var planetGameObject = UnityEngine.Object.Instantiate(_planetRootPrefab, position, Quaternion.identity);

        var spaceObjectVisual = planetGameObject.GetComponent<SpaceObjectVisual>();

        var planet = new Planet();

        planet.Radius = random.Value(0.5f, 1f);
        planet.Seed = random.Seed;
        planet.position = position;
        planet.visual = spaceObjectVisual;

        spaceObjectVisual.SetSpaceObject(planet);

        var meshFilter = planetGameObject.GetComponent<MeshFilter>();

        var icosahedronGenerator = new IcosahedronGenerator();
        icosahedronGenerator.Initialize();
        icosahedronGenerator.Subdivide(1);

        int vertexCount = icosahedronGenerator.Polygons.Count * 3;
        int[] triangles = new int[vertexCount];

        Vector3[] vertices = new Vector3[vertexCount];
        Color32[] colors = new Color32[vertexCount];

        for (int i = 0; i < icosahedronGenerator.Polygons.Count; i++)
        {
            var poly = icosahedronGenerator.Polygons[i];

            triangles[i * 3 + 0] = i * 3 + 0;
            triangles[i * 3 + 1] = i * 3 + 1;
            triangles[i * 3 + 2] = i * 3 + 2;

            vertices[i * 3 + 0] = icosahedronGenerator.Vertices[poly.vertices[0]];
            vertices[i * 3 + 1] = icosahedronGenerator.Vertices[poly.vertices[1]];
            vertices[i * 3 + 2] = icosahedronGenerator.Vertices[poly.vertices[2]];

            colors[i * 3 + 0] = new Color32(random.Byte(), random.Byte(), random.Byte(), 255);
            colors[i * 3 + 1] = new Color32(random.Byte(), random.Byte(), random.Byte(), 255);
            colors[i * 3 + 2] = new Color32(random.Byte(), random.Byte(), random.Byte(), 255);
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            var o = new Vector3(vertices[i].x, vertices[i].y, vertices[i].z);

            var t = new Vector3(random.Value(-0.1f, 0.1f), random.Value(-0.1f, 0.1f), random.Value(-0.1f, 0.1f));

            for (int j = 0; j < vertices.Length; j++)
            {
                if (o.x == vertices[j].x && o.y == vertices[j].y && o.z == vertices[j].z)
                {
                    vertices[j].x += t.x;
                    vertices[j].y += t.y;
                    vertices[j].z += t.z;
                }
            }
        }

        var mesh = meshFilter.mesh;

        mesh.Clear();

        mesh.vertices = vertices;
        mesh.SetTriangles(triangles, 0);
        mesh.colors32 = colors;

        //mesh.Optimize();
        mesh.RecalculateNormals();

        planetGameObject.name = "Planet";
        planetGameObject.transform.SetParent(_rootGameObject.transform, false);
        planetGameObject.transform.localScale = new Vector3(planet.Radius, planet.Radius, planet.Radius);

        //if (settings.AsignRandomColor)
        //{
            var material = new Material(planetGameObject.GetComponent<MeshRenderer>().material);

            var hue = _planetHues[random.Value(0, _planetHues.Length)];

            var color = Color.HSVToRGB(random.Value((hue - 10f) / 360f, (hue + 10f) / 360f), random.Value(0.8f, 1f), random.Value(0.2f, 0.5f));

            material.SetColor("_BaseColor", color);

            planetGameObject.GetComponent<MeshRenderer>().material = material;
        //}

        planetGameObject.GetComponent<MeshRenderer>().material = material;

        var rotateBehaviour = planetGameObject.GetComponent<RotateBehaviour>();

        var axisAngle = random.Value(0, Mathf.PI * 2f);

        rotateBehaviour.Axis = new Vector3(Mathf.Sin(axisAngle), Mathf.Cos(axisAngle), 0f);
        rotateBehaviour.Speed = random.Value(1f, 120f);

        //GenerateMoons(new MoonSettings
        //{
        //    Planet = planetGameObject,
        //    Count = random.Value(1, 5),
        //    MinOffset = planet.Radius * 2.50f,
        //    MaxOffset = planet.Radius * 3.75f,
        //    MinRadius = 0.125f,
        //    MaxRadius = 0.225f,
        //});

        GenerateMoons(new MoonSettings
        {
            Planet = planetGameObject,
            Count = random.Value(8, 25),
            MinOffset = planet.Radius * 2.55f,
            MaxOffset = planet.Radius * 3.25f,
            MinRadius = 0.025f,
            MaxRadius = 0.100f,
            AsignRandomColor = true,
        });

        return new Result();
    }

    public static Result GenerateMoons(MoonSettings settings)
    {
        for(var i = 0; i < settings.Count; i++)
        {
            GenerateMoon(settings);
        }
        
        return new Result();
    }

    public static Result GenerateMoon(MoonSettings settings)
    {
        var random = new RandomGenerator();

        var parentRotateBehaviour = settings.Planet.GetComponent<RotateBehaviour>();

        var a = Vector3.Cross(parentRotateBehaviour.Axis, Vector3.back);

        a = Quaternion.AngleAxis(random.Value(0, 360), parentRotateBehaviour.Axis) * a;

        _planetRootPrefab = Resources.Load<GameObject>(_planetRootPrefabName);

        var position = a * random.Value(settings.MinOffset, settings.MaxOffset);

        var planetGameObject = UnityEngine.Object.Instantiate(_planetRootPrefab, position, Quaternion.identity);

        var spaceObjectVisual = planetGameObject.GetComponent<SpaceObjectVisual>();

        var planet = new Planet();

        planet.Radius = random.Value(settings.MinRadius, settings.MaxRadius);
        planet.Seed = random.Seed;
        planet.position = position;
        planet.visual = spaceObjectVisual;

        spaceObjectVisual.SetSpaceObject(planet);

        var meshFilter = planetGameObject.GetComponent<MeshFilter>();

        var icosahedronGenerator = new IcosahedronGenerator();
        icosahedronGenerator.Initialize();

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

            var t = new Vector3(random.Value(-0.1f, 0.1f), random.Value(-0.1f, 0.1f), random.Value(-0.1f, 0.1f));

            for (int j = 0; j < vertices.Length; j++)
            {
                if (o.x == vertices[j].x && o.y == vertices[j].y && o.z == vertices[j].z)
                {
                    vertices[j].x += t.x;
                    vertices[j].y += t.y;
                    vertices[j].z += t.z;
                }
            }
        }

        var mesh = meshFilter.mesh;

        mesh.Clear();

        mesh.vertices = vertices;
        mesh.SetTriangles(triangles, 0);

        //mesh.Optimize();
        mesh.RecalculateNormals();

        planetGameObject.name = "Moon";
        planetGameObject.transform.SetParent(settings.Planet.transform, false);
        planetGameObject.transform.localScale = new Vector3(planet.Radius, planet.Radius, planet.Radius);

        if(settings.AsignRandomColor)
        {
            var material = new Material(planetGameObject.GetComponent<MeshRenderer>().material);

            var hue = _planetHues[random.Value(0, _planetHues.Length)];

            var color = Color.HSVToRGB(random.Value((hue - 10f) / 360f, (hue + 10f) / 360f), random.Value(0.8f, 1f), random.Value(0.2f, 0.5f));

            material.SetColor("_BaseColor", color);

            planetGameObject.GetComponent<MeshRenderer>().material = material;
        }

        var rotateBehaviour = planetGameObject.GetComponent<RotateBehaviour>();

        //rotateBehaviour.enabled = false;

        var axisAngle = random.Value(0, Mathf.PI * 2f);

        rotateBehaviour.Axis = new Vector3(Mathf.Sin(axisAngle), Mathf.Cos(axisAngle), 0f);
        rotateBehaviour.Speed = random.Value(1f, 120f);

        return new Result();
    }
}
