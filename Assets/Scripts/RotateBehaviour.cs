using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class RotateBehaviour : ObjectBehaviour
{
    public Vector3? Point = null;
    public float Speed { get; set; } = 8f;
    public Vector3 Axis { get; set; } = new Vector3(0, 1, 0);

    void Update()
    {
        transform.RotateAround(Point ?? transform.position, Axis, Speed * Time.deltaTime);
    }
}
