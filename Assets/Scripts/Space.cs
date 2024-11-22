using UnityEngine;

public class Space : MonoBehaviour
{
    public static Space Instance { get; private set; }

    public int Width = 2000;
    public int Height = 2000;

    private void Awake()
    {
        Instance = this;
    }
}
