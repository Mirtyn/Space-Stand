using UnityEngine;

public class SpaceObjectVisual : ObjectBehaviour
{
    public Transform Transform { get; private set; }
    private SpaceObject spaceObject;

    private void Awake()
    {
        this.Transform = transform;
    }

    public void SetSpaceObject(SpaceObject spaceObject)
    {
        this.spaceObject = spaceObject;
    }

    public void OnClick()
    {
        spaceObject.Clicked();
    }
}
