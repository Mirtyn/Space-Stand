using UnityEngine;

public class GameManager
{
    private static readonly GameManager _instance = new GameManager();

    private const string _mainCameraGameObjectName = "Main Camera";

    private GameObject _mainCameraGameObject = null;

    public static GameManager Instance { get { return _instance; } }

    public SpaceManager SpaceManager { get; set; }

    public Camera MainCamera { get; private set; }

    public PlayerInput PlayerInput { get; private set; }

    public void Initialize()
    {
        if(_mainCameraGameObject == null)
        {
            _mainCameraGameObject = GameObject.Find(_mainCameraGameObjectName);
            MainCamera = _mainCameraGameObject.GetComponent<Camera>();
        }

        if(PlayerInput == null)
        {
            PlayerInput = new PlayerInput();
        }

        if (SpaceManager == null)
        {
            var settings = new SpaceGenerator.SpaceSettings();

            //settings.MinPlanetCount = settings.MaxPlanetCount = 1;
            //settings.SpaceSize = 1;
            settings.Seed = 1024;

            SpaceManager = SpaceGenerator.Generate(settings);
        }
    }

    public void Update()
    {
        UpdatePlayerInput();
    }


    private void UpdatePlayerInput()
    {
        PlayerInput.MouseScrollDelta = Input.mouseScrollDelta.y;

        PlayerInput.Mouse0Down = Input.GetMouseButtonDown(0);

        PlayerInput.Mouse1Or2Pressed = Input.GetMouseButton(1) || Input.GetMouseButton(2);

        var moveInput = PlayerInput.MoveInput;

        if (Input.GetKey(KeyCode.W))
        {
            moveInput.y++;
        }

        if (Input.GetKey(KeyCode.S))
        {
            moveInput.y--;
        }

        if (Input.GetKey(KeyCode.D))
        {
            moveInput.x++;
        }

        if (Input.GetKey(KeyCode.A))
        {
            moveInput.x--;
        }
    }

}
