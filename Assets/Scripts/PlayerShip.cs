using System;
using UnityEngine;

public class PlayerShip
{
    //public static PlayerShip PlayerShipInstance { get; private set; }
    public Vector3 Position { get; set; }

    public PlayerShip(Vector3 position)
    {
        Position = position;
    }

    public PlayerShip()
    {
        
    }
}
