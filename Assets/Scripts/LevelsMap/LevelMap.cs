using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMap : MonoBehaviour
{
    public static LevelMap singleton;

    public Level LevelNow;
    public static Transform LevelPrefab;
    public static Transform ConnectionPrefab;
    public Dictionary<Vector2Int, Level> LevelsOnCoords;

    public bool RegenerateTest;

    public GameActions inputActions;

    bool IsLeftButtonPressed;
    bool IsRightButtonPressed;
    Vector2 MousePos;
    float zoomValue = 1;
    float ZoomSpeed = 2f;

    private Vector2 Input_Move;
    bool isMovingCamera = false;

    public Transform MapTransform;
    public Transform MapZoomTransform;
    Vector2 startMapPos;
    Vector2 startPos;

    public Transform TopPos;
    public Transform RightPos;
    public Transform DownPos;
    public Transform LeftPos;

    public Transform TopTargetPos;
    public Transform RightTargetPos;
    public Transform DownTargetPos;
    public Transform LeftTargetPos;

    private void Awake()
    {
        singleton = this;

        inputActions = new GameActions();
        inputActions.Gameplay.MouseLeft.started += ctx => IsLeftButtonPressed = true;
        inputActions.Gameplay.MouseLeft.canceled += ctx => IsLeftButtonPressed = false;

        inputActions.Gameplay.MouseRight.started += ctx => IsRightButtonPressed = true;
        inputActions.Gameplay.MouseRight.canceled += ctx => IsRightButtonPressed = false;

        inputActions.Gameplay.MousePosition.performed += ctx => MousePos = ctx.ReadValue<Vector2>();

        inputActions.Gameplay.Move.performed += ctx => Input_Move = ctx.ReadValue<Vector2>();
        inputActions.Gameplay.Move.canceled += ctx => Input_Move = Vector2.zero;

        inputActions.Gameplay.MouseScroll.performed += ctx => RotateScroll(ctx.ReadValue<float>());
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();

    }

    private void Start()
    {
        LevelPrefab = PrefabManager.GetPrefab("LevelMap_Level");
        ConnectionPrefab = PrefabManager.GetPrefab("LevelMap_Connection");
        if (LevelMapManager.LevelsData == null)
        {
            LevelMapManager.CreateOrLoadMap();
        }
        SpawnMap();
        LevelMapManager.CheckForLevelEnded();
    }

    public void SpawnMap()
    {
        LevelsOnCoords = new Dictionary<Vector2Int, Level>();
        foreach (var connections in LevelMapManager.LevelsConnections)
        {
            foreach (var target in connections.Value)
            {

                var connection = Instantiate(ConnectionPrefab, transform);

                var pos1 = LevelMapManager.ConvertCoordsToPosition(connections.Key, true);
                var pos2 = LevelMapManager.ConvertCoordsToPosition(target, true);

                var doubledPos = pos1 + pos2;
                var middlePos = new Vector2(doubledPos.x / 2f, doubledPos.y / 2f);
                
                connection.localPosition = middlePos;
                connection.localScale = new Vector3(Vector2.Distance(pos1, pos2), 1, 1);

                var angle = Vector2.SignedAngle(Vector2.right, pos1 - pos2);
                connection.localRotation = Quaternion.Euler(0, 0, angle);
            }
        }

        TopPos = transform;
        RightPos = transform;
        DownPos = transform;
        LeftPos = transform;

        foreach (var e in LevelMapManager.LevelsDataPoses) {
            var levelObj = Instantiate(LevelPrefab, transform);
            var convertedPos = LevelMapManager.ConvertCoordsToPosition(e.Key, true);
            levelObj.localPosition = convertedPos;

            if (levelObj.position.x > RightPos.position.x)
                RightPos = levelObj;
            if (levelObj.position.y > TopPos.position.y)
                TopPos = levelObj;

            if (levelObj.position.x < LeftPos.position.x)
                LeftPos = levelObj;
            if (levelObj.position.y < DownPos.position.y)
                DownPos = levelObj;

            var level = levelObj.GetComponent<Level>();
            LevelsOnCoords[e.Key] = level;
            level.InitLevel(e.Value, e.Key);
            level.UpdatedLevelStatus(e.Value.LevelStatus);
        }
    }

    public void LevelSelected(Level level) {
        if (LevelNow == level)
            return;
        if (LevelNow != null) {
            LevelNow.Unselected();
        }
        LevelNow = level;
        LevelNow.Selected();
        //LevelMapManager.LoadLevel(level);
    }

    void RotateScroll(float value)
    {
        if (value == 0) return;

        zoomValue += ZoomSpeed * value * 0.5f * Time.deltaTime;
        zoomValue = Mathf.Clamp(zoomValue, 0.5f, 2f);
    }

    private void Update()
    {
        if (RegenerateTest) {
            for (var i = 0; i < transform.childCount; i++) {
                Destroy(transform.GetChild(i).gameObject);
            }
            LevelMapManager.LevelsData = null;
            Start();
            RegenerateTest = false;
        }

        if (IsLeftButtonPressed || IsRightButtonPressed) {
            if (!isMovingCamera) {
                isMovingCamera = true;
                startPos = MousePos;
                startMapPos = MapTransform.localPosition;
            }
            MapTransform.localPosition = startMapPos + (MousePos - startPos) / MapZoomTransform.localScale.x;
        }
        else {
            isMovingCamera = false;
        }
        MapZoomTransform.localScale = Vector3.Lerp(MapZoomTransform.localScale, Vector3.one * zoomValue, Time.deltaTime * 5);

        if (TopTargetPos.position.y < DownPos.position.y)
        {
            var delta = TopTargetPos.position.y - DownPos.position.y;
            MapTransform.position = new Vector3(MapTransform.position.x, MapTransform.position.y + delta, 0);
        }

        if (DownTargetPos.position.y > TopPos.position.y)
        {
            var delta = DownTargetPos.position.y - TopPos.position.y;
            MapTransform.position = new Vector3(MapTransform.position.x, MapTransform.position.y + delta, 0);
        }

        if (LeftTargetPos.position.x > RightPos.position.x)
        {
            var delta = LeftTargetPos.position.x - RightPos.position.x;
            MapTransform.position = new Vector3(MapTransform.position.x + delta, MapTransform.position.y, 0);
        }

        if (RightTargetPos.position.x < LeftPos.position.x)
        {
            var delta = RightTargetPos.position.x - LeftPos.position.x;
            MapTransform.position = new Vector3(MapTransform.position.x + delta, MapTransform.position.y, 0);
        }
    }
}
