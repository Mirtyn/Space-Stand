using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class UiPanelBehaviour : ObjectBehaviour
{
    public static readonly Vector2 ScreenReferenceResolution = new Vector2(1920, 1080);
    private static Vector2 ScreenResolution = new Vector2();

    void Update()
    {
        if(ScreenResolution.x != Screen.width || ScreenResolution.y != Screen.height)
        {
            ScreenResolution.x = Screen.width;
            ScreenResolution.y = Screen.height;

            var aspect = ScreenResolution.x / ScreenReferenceResolution.x;

            var t = (RectTransform)transform;

            var panelLeft = (t.offsetMin.x * aspect);
            var panelRight = (Mathf.Abs(t.offsetMax.x) * aspect);
            var panelWidth = (int)(ScreenResolution.x - panelLeft - panelRight);

            var panelTop = (t.offsetMin.y * aspect);
            var panelBottom = (Mathf.Abs(t.offsetMax.y) * aspect);
            var panelHeight = (int)(ScreenResolution.y - panelTop - panelBottom);

            Debug.Log($"panelLeft: {panelLeft} | panelWidth: {panelWidth} | panelTop: {panelTop} | panelHeight: {panelHeight}");
        }
    }
}
