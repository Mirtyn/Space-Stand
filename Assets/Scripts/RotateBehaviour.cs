using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class RotateBehaviour : ObjectBehaviour
{
    private Vector3? _point = null;

    public Vector3 Point { get { return _point ?? transform.position; } set { _point = value; } }
    public float Speed { get; set; } = 8f;
    public Vector3 Axis { get; set; } = new Vector3(0, 1, 0);

    void Update()
    {
        transform.RotateAround(Point, Axis, Speed * Time.deltaTime);
    }
}
