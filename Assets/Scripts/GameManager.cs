using UnityEngine;

public class GameManager : ProjectBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
}
