using UnityEngine;

public class PlayerShip : MonoBehaviour
{
    public static PlayerShip Instance { get; private set; }
    private Transform thisTransform;

    private void Awake()
    {
        Instance = this;
    }


}
