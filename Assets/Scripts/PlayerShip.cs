using System;
using UnityEngine;

public class PlayerShip : MonoBehaviour
{
    public static PlayerShip PlayerShipInstance { get; private set; }
    private Transform thisTransform;

    //private ResourceHolder[] resources;

    private void Awake()
    {
        PlayerShipInstance = this;

        //var values = Enum.GetNames(typeof(ResourceType));
        //resources = new ResourceHolder[values.Length];
        //for (int i = 0; i < values.Length; i++)
        //{
        //    resources[i] = new ResourceHolder(0, (ResourceType)i);
        //}
    }
}
