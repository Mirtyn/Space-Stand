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

    public class Settings
    {

    }

    public class Result
    {
        public SpaceManager Space { get; private set; } = new SpaceManager();
    }

    public static Result Generate(Settings settings)
    {
        _rootGameObject = GameObject.Find(_rootGameObjectName);

        _planetRootPrefab = Resources.Load<GameObject>(_planetRootPrefabName);

        var position = new Vector3(0, 0, 0);

        var planetGameObject = UnityEngine.Object.Instantiate(_planetRootPrefab, position, Quaternion.identity);

        var meshFilter = planetGameObject.GetComponent<MeshFilter>();

        var vertices = new Vector3[]
        {
            new Vector3(0.0f, 0.0f, 0.0f),
            new Vector3(0.0f, 1.0f, 0.0f),
            new Vector3(1.0f, 0.0f, 0.0f),
        };

        var triangles = new int[]
        {
            0, 1, 2,
        };

        var mesh = meshFilter.mesh;

        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        planetGameObject.name = "Planet";
        planetGameObject.transform.SetParent(_rootGameObject.transform, false);

        return new Result();
    }
}
