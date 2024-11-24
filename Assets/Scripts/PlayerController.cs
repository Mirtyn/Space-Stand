using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerController : ObjectBehaviour
{
    private Camera mainCamera;
    private PlayerInputs input;
    [SerializeField] private float scrollSensitivity = 60f;
    [SerializeField] private float zoomSpeed = 5f;
    //[SerializeField] private float moveSpeed = 5f;

    [SerializeField] private int[] zoomSteps = new int[] { 50, 250, 500, 750, 1000 };
    [SerializeField] private float minDifferencReqForNextStep = 30f;
    private float maxDistanceFromEndSteps = 10f;
    private float targetZoomLevel = 250;
    private int targetZoomStep = 1;

    [SerializeField] private Transform pointer;
    [SerializeField] private AnimationCurve pointerScaleCurve;
    [SerializeField] private AnimationCurve pointerMoveCurve;

    [SerializeField] private float pointerScaleDeltaSpeed = 1f;
    private float pointerScaleDelta = 0f;

    [SerializeField] private float pointerMoveDeltaSpeed = 1f;
    private float pointerMoveDelta = 0f;

    private float _pointerTargetScale = 1f;
    private float pointerTargetScale
    {
        get
        {
            return _pointerTargetScale;
        }
        set
        {
            if (value != _pointerTargetScale)
            {
                _pointerTargetScale = value;
                pointerScaleDelta = 0f;
            }
        }
    }
    private Vector3 pointerStartPosition;
    private Vector3 pointerTargetPosition;

    private Vector2 prevFrameMouseScreenPos;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        input = GetPlayerInput();

        //Game.UIPanelManager.CheckForPanelsOnScreenPos(Input.mousePosition);

        HandleCameraScroll();
        HandleCameraMovement();

        HandleMouseHover();
        HandlePointer();
    }

    private void HandleCameraScroll()
    {
        var p = mainCamera.transform.position;

        //p.z += -input.MouseScrollDelta * Time.deltaTime * scrollSensitivity;

        //Debug.Log($"scroll: {-input.MouseScrollDelta} DeltaTime: {Time.deltaTime} sensitivity: {scrollSensitivity} result: {-input.MouseScrollDelta * Time.deltaTime * scrollSensitivity}");
        targetZoomLevel += -input.MouseScrollDelta * scrollSensitivity;

        if (targetZoomLevel > (zoomSteps[targetZoomStep] + minDifferencReqForNextStep))
        {
            targetZoomStep++;

            if (targetZoomStep >= zoomSteps.Length)
            {
                targetZoomStep = zoomSteps.Length - 1;
            }

            targetZoomLevel = zoomSteps[targetZoomStep];
        }
        else if (targetZoomLevel < (zoomSteps[targetZoomStep] - minDifferencReqForNextStep))
        {
            targetZoomStep--;

            if (targetZoomStep < 0)
            {
                targetZoomStep = 0;
            }

            targetZoomLevel = zoomSteps[targetZoomStep];
        }

        if (targetZoomLevel > zoomSteps[zoomSteps.Length - 1] + maxDistanceFromEndSteps)
        {
            targetZoomLevel = zoomSteps[zoomSteps.Length - 1] + maxDistanceFromEndSteps;
        }
        else if (targetZoomLevel < zoomSteps[0] - maxDistanceFromEndSteps)
        {
            targetZoomLevel = zoomSteps[0] - maxDistanceFromEndSteps;
        }

        //targetZoomLevel = Mathf.Lerp(targetZoomLevel, zoomSteps[targetZoomStep], Time.deltaTime * (zoomSpeed / 25));

        Vector2 currentMouseScreenPos = Input.mousePosition;
        Vector3 currentMouseWorldPos = mainCamera.ScreenToWorldPoint(currentMouseScreenPos);
        Vector3 centerScreenWorldPos = mainCamera.ScreenToWorldPoint(new Vector2(Screen.width / 2, Screen.height / 2));

        Vector3 difference = centerScreenWorldPos - currentMouseWorldPos;

        float prevSize = mainCamera.orthographicSize;

        mainCamera.orthographicSize = Mathf.Round(Mathf.Lerp(mainCamera.orthographicSize, targetZoomLevel, Time.deltaTime * zoomSpeed));

        difference *= (mainCamera.orthographicSize / prevSize) - 1;
        mainCamera.transform.position += difference;

        //mainCamera.orthographicSize = targetZoomLevel;
    }

    private void HandleCameraMovement()
    {
        if (input.Mouse1Or2Pressed)
        {
            Vector2 currentMouseScreenPos = Input.mousePosition;
            Vector3 prevFrameMouseWorldPos = mainCamera.ScreenToWorldPoint(prevFrameMouseScreenPos);
            Vector3 currentMouseWorldPos = mainCamera.ScreenToWorldPoint(currentMouseScreenPos);

            Vector3 difference = prevFrameMouseWorldPos - currentMouseWorldPos;
            mainCamera.transform.position += difference;
        }

        prevFrameMouseScreenPos = Input.mousePosition;

        CheckCameraWithinBoundsOfWorld();
    }

    private void CheckCameraWithinBoundsOfWorld()
    {
        int spaceSize = GameManager.Instance.SpaceManager.Space.Size / 2;

        Vector3 cameraPos = mainCamera.transform.position;

        if (cameraPos.x > spaceSize)
        {
            cameraPos.x = spaceSize;
        }
        else if (cameraPos.x < -spaceSize)
        {
            cameraPos.x = -spaceSize;
        }

        if (cameraPos.y > spaceSize)
        {
            cameraPos.y = spaceSize;
        }
        else if (cameraPos.y < -spaceSize)
        {
            cameraPos.y = -spaceSize;
        }

        mainCamera.transform.position = cameraPos;
    }

    private void HandleMouseHover()
    {
        Vector2 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0;

        var collider = Physics2D.OverlapPoint(mouseWorldPos);


        if (collider != null)
        {
            pointerStartPosition = pointer.position;
            pointerTargetPosition = collider.transform.position;

            //increaseMoveDelta = true;
            if (collider is CircleCollider2D)
            {
                pointerTargetScale = (collider as CircleCollider2D).radius;
            }

            if (input.Mouse0Down)
            {
                if (collider.gameObject.TryGetComponent<SpaceObjectVisual>(out SpaceObjectVisual visual))
                {
                    visual.OnClick();
                }
            }

            return;
        }
        else if (input.Mouse0Down)
        {
            SpaceObjectInspector.Instance.InView = false;
        }

        pointerTargetScale = 0f;
        pointerMoveDelta = 0f;
    }

    private void HandlePointer()
    {
        MovePointer();
        ScalePointer();
    }

    private void MovePointer()
    {
        pointerMoveDelta += Time.deltaTime * pointerMoveDeltaSpeed;

        pointerMoveDelta = Mathf.Clamp01(pointerMoveDelta);
        pointer.position = Vector2.LerpUnclamped(pointerStartPosition, pointerTargetPosition, pointerMoveCurve.Evaluate(pointerMoveDelta));
    }

    private void ScalePointer()
    {
        pointerScaleDelta += Time.deltaTime * pointerScaleDeltaSpeed;

        pointerScaleDelta = Mathf.Clamp01(pointerScaleDelta);

        float f = pointer.localScale.x;
        f = Mathf.LerpUnclamped(f, pointerTargetScale, pointerScaleCurve.Evaluate(pointerScaleDelta));
        Vector3 v3 = new Vector3(f, f, f);
        pointer.localScale = v3;

        if (Mathf.Round(f) == 0f)
        {
            pointer.gameObject.SetActive(false);
        }
        else
        {
            pointer.gameObject.SetActive(true);
        }
    }

    private PlayerInputs GetPlayerInput()
    {
        PlayerInputs input = new PlayerInputs();

        input.MouseScrollDelta = Input.mouseScrollDelta.y;

        input.Mouse0Down = Input.GetMouseButtonDown(0);

        input.Mouse1Or2Pressed = Input.GetMouseButton(1) || Input.GetMouseButton(2);
        //input.Mouse1Or2Up = Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2);

        if (Input.GetKey(KeyCode.W))
        {
            input.MoveInput.y++;
        }

        if (Input.GetKey(KeyCode.S))
        {
            input.MoveInput.y--;
        }

        if (Input.GetKey(KeyCode.D))
        {
            input.MoveInput.x++;
        }

        if (Input.GetKey(KeyCode.A))
        {
            input.MoveInput.x--;
        }

        return input;
    }
}


public struct PlayerInputs
{
    public float MouseScrollDelta;
    public bool Mouse0Down;
    public bool Mouse1Or2Pressed;
    public Vector2 MoveInput;
}
