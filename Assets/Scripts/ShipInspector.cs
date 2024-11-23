using UnityEngine;
using UnityEngine.UI;

public class ShipInspector : ObjectBehaviour
{
    public static ShipInspector Instance {  get; private set; }

    private Transform thisTransform;

    [SerializeField] private Transform shipInspectorPanelStartPoint;
    [SerializeField] private Transform shipInspectorPanelEndPoint;
    [SerializeField] private AnimationCurve shipInspectorPanelSlideCurve;

    private bool inView;

    [SerializeField] private float shipMoveDeltaSpeed = 1f;
    private float shipMoveDelta = 1f;

    [SerializeField] private Button openShipButton;
    [SerializeField] private Button closeShipButton;

    public bool InView
    {
        get
        {
            return inView;
        }
        set
        {
            if (inView != value)
            {
                shipMoveDelta = 0;
                inView = value;
            }
        }
    }

    private void Awake()
    {
        Instance = this;
        thisTransform = transform;
        openShipButton.onClick.AddListener(OnShipOpened);
        closeShipButton.onClick.AddListener(OnShipClosed);
    }

    private void Update()
    {
        HandleInspectorMovement();
    }

    private void HandleInspectorMovement()
    {
        shipMoveDelta = Mathf.Lerp(shipMoveDelta, 1.1f, Time.deltaTime * shipMoveDeltaSpeed);

        shipMoveDelta = Mathf.Clamp01(shipMoveDelta);
        Vector3 start = inView ? shipInspectorPanelStartPoint.position : shipInspectorPanelEndPoint.position;
        Vector3 end = inView ? shipInspectorPanelEndPoint.position : shipInspectorPanelStartPoint.position;

        thisTransform.position = Vector3.LerpUnclamped(start, end, shipInspectorPanelSlideCurve.Evaluate(shipMoveDelta));
    }

    private void OnShipOpened()
    {
        InView = true;
    }

    private void OnShipClosed()
    {
        InView = false;
    }
}
