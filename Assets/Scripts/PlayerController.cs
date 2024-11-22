using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerController : RootBehaviour
{
    private Camera camera;
    private PlayerInputs input;
    [SerializeField] private float scrollSensitivity = 10f;
    [SerializeField] private float moveSpeed = 5f;

    [SerializeField] private Transform pointer;
    [SerializeField] private AnimationCurve pointerScaleCurve;
    [SerializeField] private AnimationCurve pointerMoveCurve;

    [SerializeField] private float pointerScaleDeltaSpeed = 1f;
    private float pointerScaleDelta = 0f;
    private bool increaseScaleDelta = false;

    [SerializeField] private float pointerMoveDeltaSpeed = 1f;
    private float pointerMoveDelta = 0f;
    private bool increaseMoveDelta = false;

    private float pointerTargetScale = 1f;
    private Vector3 pointerStartPosition;
    private Vector3 pointerTargetPosition;

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    private void Update()
    {
        input = GetPlayerInput();

        HandleCameraScroll();
        HandleCameraMovement();

        HandleMouseHover();
        HandlePointer();
    }

    private void HandleCameraScroll()
    {
        camera.orthographicSize += -input.MouseScrollDelta * Time.deltaTime * scrollSensitivity;
    }

    private void HandleCameraMovement()
    {
        var v2 = input.MoveInput * Time.deltaTime * moveSpeed;
        Vector3 pos = new Vector3(v2.x, v2.y);
        camera.transform.position += pos;
    }

    private void HandleMouseHover()
    {
        Vector2 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = camera.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0;

        var collider = Physics2D.OverlapPoint(mouseWorldPos);

        if (collider != null)
        {
            pointerStartPosition = pointer.position;
            pointerTargetPosition = collider.transform.position;

            increaseMoveDelta = true;
            increaseScaleDelta = true;

            if (input.Mouse0Down)
            {
                collider.gameObject.GetComponent<SpaceObjectVisual>().OnClick();


            }
        }
        else if (input.Mouse0Down)
        {
            pointerMoveDelta = 0f;
            increaseMoveDelta = false;
            increaseScaleDelta = false;

            SpaceObjectInspector.Instance.InView = false;
        }
        else
        {
            pointerMoveDelta = 0f;
            increaseMoveDelta = false;
            increaseScaleDelta = false;
        }
    }

    private void HandlePointer()
    {
        MovePointer();
        ScalePointer();
    }

    private void MovePointer()
    {
        if (increaseMoveDelta)
        {
            pointerMoveDelta += Time.deltaTime * pointerMoveDeltaSpeed;
        }
        else
        {
            pointerMoveDelta -= Time.deltaTime * pointerMoveDeltaSpeed;
        }

        pointerMoveDelta = Mathf.Clamp01(pointerMoveDelta);
        pointer.position = Vector2.LerpUnclamped(pointerStartPosition, pointerTargetPosition, pointerMoveCurve.Evaluate(pointerMoveDelta));
    }

    private void ScalePointer()
    {
        if (increaseScaleDelta)
        {
            pointerScaleDelta += Time.deltaTime * pointerScaleDeltaSpeed;
        }
        else
        {
            pointerScaleDelta -= Time.deltaTime * pointerScaleDeltaSpeed;
        }

        pointerScaleDelta = Mathf.Clamp01(pointerScaleDelta);

        float f = Mathf.LerpUnclamped(0, pointerTargetScale, pointerScaleCurve.Evaluate(pointerScaleDelta));
        Vector3 v3 = new Vector3(f, f, f);
        pointer.localScale = v3;
    }

    private PlayerInputs GetPlayerInput()
    {
        PlayerInputs input = new PlayerInputs();

        input.MouseScrollDelta = Input.mouseScrollDelta.y;

        input.Mouse0Down = Input.GetMouseButtonDown(0);

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
    public Vector2 MoveInput;
}
