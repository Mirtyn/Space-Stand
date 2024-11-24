using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput
{
    public float MouseScrollDelta { get; set; }

    public bool Mouse0Down { get; set; }

    public bool Mouse1Or2Pressed { get; set; }
    
    public Vector2 MoveInput { get; set; }
}
