using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpaceObjectInspector : ObjectBehaviour
{
    public static SpaceObjectInspector Instance {  get; private set; }

    private Transform thisTransform;

    [SerializeField] private Transform planetInspectorPanelStartPoint;
    [SerializeField] private Transform planetInspectorPanelEndPoint;
    [SerializeField] private AnimationCurve planetInspectorPanelSlideCurve;

    private bool inView;

    [SerializeField] private float inspectorMoveDeltaSpeed = 1f;
    private float inspectorMoveDelta = 0f;

    public bool InView 
    { 
        get
        {
            return inView; 
        }
        set
        {
            inspectorMoveDelta = 0;
            inView = value;
        }
    }

    [SerializeField] private TMP_Text spaceObjectNameText;
    [SerializeField] private TMP_Text spaceObjectDescriptionText;
    [SerializeField] private Button TravelButton;

    private void Awake()
    {
        Instance = this;
        thisTransform = transform;

        TravelButton.onClick.AddListener(() =>
        {
            OnTravelButtonPressed();
        });
    }

    private void Update()
    {
        HandleInspectorMovement();
    }

    private void HandleInspectorMovement()
    {
        inspectorMoveDelta += Time.deltaTime * inspectorMoveDeltaSpeed;

        inspectorMoveDelta = Mathf.Clamp01(inspectorMoveDelta);
        Vector3 start = inView ? planetInspectorPanelStartPoint.position : planetInspectorPanelEndPoint.position;
        Vector3 end = inView ? planetInspectorPanelEndPoint.position : planetInspectorPanelStartPoint.position;

        thisTransform.position = Vector3.LerpUnclamped(start, end, planetInspectorPanelSlideCurve.Evaluate(inspectorMoveDelta));
    }

    private void OnTravelButtonPressed()
    {
        if (!inView) return;

        Debug.Log("Travel!");
    }

    public void SetSpaceObject(string name, string description)
    {
        InView = true;

        spaceObjectNameText.text = name;
        spaceObjectDescriptionText.text = description;
    }
}
