using Newtonsoft.Json;
using SpaceStand.Attributes;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SpaceObjectClickEvent
{
    public bool IsHandled { get; set; } = false;
    public GameObject ClickedGameObject { get; internal set; }
    public Vector3 MouseWorldPosition { get; internal set; }
    public Vector2 MouseScreenPosition { get; internal set; }
}
