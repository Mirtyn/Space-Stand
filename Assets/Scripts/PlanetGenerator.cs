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

        var spaceObjectVisual = planetGameObject.GetComponent<SpaceObjectVisual>();

        var planet = new Planet(position, spaceObjectVisual);

        spaceObjectVisual.SetSpaceObject(planet);

        var meshFilter = planetGameObject.GetComponent<MeshFilter>();

        const float width = 1f;
        const float height = 1f;
        const int widthSegments = 4;
        const int heightSegments = 4;

        const float width_half = width / 2;
        const float height_half = height / 2;

        const int gridX = widthSegments;
        const int gridY = heightSegments;

        const int gridX1 = gridX + 1;
        const int gridY1 = gridY + 1;

        const float segment_width = width / (float)gridX;
        const float segment_height = height / (float)gridY;

        var vertices = new List<Vector3>();
        var normals = new List<Vector3>();
        var uvs = new List<Vector2>();
        var triangles = new List<int>();

        for (var iy = 0; iy < gridY1; iy++)
        {
            float y = iy * segment_height - height_half;

            for (var ix = 0; ix < gridX1; ix++)
            {
                float x = ix * segment_width - width_half;

                float xx = x + width_half;

                float yy = y + height_half;

                float xCircle = xx + width_half * Mathf.Sqrt(1f - 0.5f * yy * yy);
                float yCircle = yy * Mathf.Sqrt(1f - 0.5f * xx * xx);

                //vertices.Add(new Vector3(x, -y, 0));
                vertices.Add(new Vector3(xCircle, -yCircle, 0));

                normals.Add(new Vector3(0, 0, -1));

                uvs.Add(new Vector2(ix / gridX, 1f - (iy / gridY)));
            }
        }

        for (var iy = 0; iy < gridY; iy++)
        {
            for (var ix = 0; ix < gridX; ix++)
            {
                int a = ix + gridX1 * iy;
                int b = ix + gridX1 * (iy + 1);
                int c = (ix + 1) + gridX1 * (iy + 1);
                int d = (ix + 1) + gridX1 * iy;

                triangles.Add(a);
                triangles.Add(d);
                triangles.Add(b);

                triangles.Add(b);
                triangles.Add(d);
                triangles.Add(c);
            }
        }


        //var vertices = new Vector3[]
        //{
        //    new Vector3(0.0f, 0.0f, 0.0f),
        //    new Vector3(0.0f, 1.0f, 0.0f),
        //    new Vector3(1.0f, 0.0f, 0.0f),
        //};

        //var colors = new Color32[]
        //{
        //    new Color32(255, 0, 0, 255),
        //    new Color32(255, 255, 0, 255),
        //    new Color32(0, 255, 255, 255),
        //};

        //var triangles = new int[]
        //{
        //    0, 1, 2,
        //};

        var mesh = meshFilter.mesh;

        mesh.Clear();

        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = triangles.ToArray();
        //mesh.colors32 = colors;

        //mesh.Optimize();
        //mesh.RecalculateNormals();

        planetGameObject.name = "Planet";
        planetGameObject.transform.SetParent(_rootGameObject.transform, false);

        return new Result();
    }
}
