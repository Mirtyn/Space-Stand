using UnityEngine;

public class UIPanelManager : ObjectBehaviour
{
    [SerializeField] private RectTransform[] uiPanels;

    private void Awake()
    {
        Game.UIPanelManager = this;
    }

    public bool CheckForPanelsOnScreenPos(Vector2 screenPosition)
    {
        bool panelInTheWay = false;

        //for (int i = 0; i < uiPanels.Length; ++i)
        //{
        //    Debug.Log(uiPanels[i].position);
        //    Debug.Log(uiPanels[i].localPosition);
        //}
        //Debug.Log(Screen.width + Screen.height);

        return panelInTheWay;


    }
}
