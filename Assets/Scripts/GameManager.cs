using UnityEngine;

public class GameManager
{
    private static readonly GameManager _instance = new GameManager();
    
    public static GameManager Instance { get { return _instance; } }

    public SpaceManager SpaceManager { get; set; }
}
