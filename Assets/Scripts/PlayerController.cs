using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerController : ObjectBehaviour
{
    [SerializeField] private float scrollSensitivity = 60f;
    [SerializeField] private float zoomSpeed = 5f;

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

    private void Update()
    {
        HandleCameraScroll();
        HandleCameraMovement();

        HandleMouseHover();
        HandlePointer();
    }

    private void HandleCameraScroll()
    {
        targetZoomLevel += -Game.PlayerInput.MouseScrollDelta * scrollSensitivity;

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
        Vector3 currentMouseWorldPos = Game.MainCamera.ScreenToWorldPoint(currentMouseScreenPos);
        Vector3 centerScreenWorldPos = Game.MainCamera.ScreenToWorldPoint(new Vector2(Screen.width / 2, Screen.height / 2));

        Vector3 difference = centerScreenWorldPos - currentMouseWorldPos;

        float prevSize = Game.MainCamera.orthographicSize;

        Game.MainCamera.orthographicSize = Mathf.Round(Mathf.Lerp(Game.MainCamera.orthographicSize, targetZoomLevel, Time.deltaTime * zoomSpeed));

        difference *= (Game.MainCamera.orthographicSize / prevSize) - 1;
        Game.MainCamera.transform.position += difference;

        //mainCamera.orthographicSize = targetZoomLevel;
    }

    private void HandleCameraMovement()
    {
        if (Game.PlayerInput.Mouse1Or2Pressed)
        {
            Vector2 currentMouseScreenPos = Input.mousePosition;
            Vector3 prevFrameMouseWorldPos = Game.MainCamera.ScreenToWorldPoint(prevFrameMouseScreenPos);
            Vector3 currentMouseWorldPos = Game.MainCamera.ScreenToWorldPoint(currentMouseScreenPos);

            Vector3 difference = prevFrameMouseWorldPos - currentMouseWorldPos;
            Game.MainCamera.transform.position += difference;
        }

        prevFrameMouseScreenPos = Input.mousePosition;

        CheckCameraWithinBoundsOfWorld();
    }

    private void CheckCameraWithinBoundsOfWorld()
    {
        int spaceSize = GameManager.Instance.SpaceManager.Space.Size / 2;
        //int spaceSize = Game.SpaceManager.Space.Size / 2;

        Vector3 cameraPos = Game.MainCamera.transform.position;

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

        Game.MainCamera.transform.position = cameraPos;
    }

    private void HandleMouseHover()
    {
        Vector2 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = Game.MainCamera.ScreenToWorldPoint(mouseScreenPos);
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

            if (Game.PlayerInput.Mouse0Down)
            {
                if (collider.gameObject.TryGetComponent<SpaceObjectVisual>(out SpaceObjectVisual visual))
                {
                    var e = new SpaceObjectClickEvent
                    {
                        ClickedGameObject = collider.gameObject,
                        MouseScreenPosition = mouseScreenPos,
                        MouseWorldPosition = mouseWorldPos,
                    };

                    visual.OnClick(e);
                }
            }

            return;
        }
        else if (Game.PlayerInput.Mouse0Down)
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
        var p = Vector2.LerpUnclamped(pointerStartPosition, pointerTargetPosition, pointerMoveCurve.Evaluate(pointerMoveDelta));
        pointer.position = new Vector3(p.x, p.y, pointer.position.z);
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
}
