using UnityEngine;
using UnityEngine.Windows;

public class PlanetPointerBehaviour : ObjectBehaviour
{
    [SerializeField] private AnimationCurve pointerScaleCurve;
    [SerializeField] private AnimationCurve pointerMoveCurve;

    [SerializeField] private float ScaleDeltaSpeed = 1f;
    private float _scaleDelta = 0f;

    [SerializeField] private float MoveDeltaSpeed = 1f;
    private float _moveDelta = 0f;

    private float _pointerTargetScale = 1f;

    private float TargetScale
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
                _scaleDelta = 0f;
            }
        }
    }

    private Vector3 _startPosition;
    private Vector3 _targetPosition;

    private void Update()
    {
        UpdateMovement();

        UpdateScale();
    }

    private void HandleMouseHover()
    {
        Vector2 mouseScreenPos = UnityEngine.Input.mousePosition;
        Vector3 mouseWorldPos = Game.MainCamera.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0;

        var collider = Physics2D.OverlapPoint(mouseWorldPos);


        if (collider != null)
        {
            _startPosition = transform.position;
            _targetPosition = collider.transform.position;

            //increaseMoveDelta = true;
            if (collider is CircleCollider2D)
            {
                TargetScale = (collider as CircleCollider2D).radius;
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

        TargetScale = 0f;
        _moveDelta = 0f;
    }

    private void UpdateMovement()
    {
        _moveDelta += Time.deltaTime * MoveDeltaSpeed;

        _moveDelta = Mathf.Clamp01(_moveDelta);
        
        var p = Vector2.LerpUnclamped(_startPosition, _targetPosition, pointerMoveCurve.Evaluate(_moveDelta));
        
        transform.position = new Vector3(p.x, p.y, transform.position.z);
    }

    private void UpdateScale()
    {
        _scaleDelta += Time.deltaTime * ScaleDeltaSpeed;

        _scaleDelta = Mathf.Clamp01(_scaleDelta);

        float f = transform.localScale.x;
        f = Mathf.LerpUnclamped(f, TargetScale, pointerScaleCurve.Evaluate(_scaleDelta));
        Vector3 v3 = new Vector3(f, f, f);
        transform.localScale = v3;

        if (Mathf.Round(f) == 0f)
        {
            transform.gameObject.SetActive(false);
        }
        else
        {
            transform.gameObject.SetActive(true);
        }
    }
}