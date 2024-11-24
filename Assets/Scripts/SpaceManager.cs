using System;
using System.Collections.Generic;
using UnityEngine;

public class SpaceManager
{
    public Space Space { get; set; }

    private const string _rootGameObjectName = "Root";

    public void DestroyRootChildren()
    {
        var root = GameObject.Find(_rootGameObjectName);

        for (var i = root.transform.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(root.transform.GetChild(i).gameObject);
        }
    }
}
