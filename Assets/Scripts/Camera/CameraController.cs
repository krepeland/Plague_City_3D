using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] public Transform cameraTransform;
    [SerializeField] private Transform cameraXRotor;
    [SerializeField] private GraphicRaycasterUI raycasterUI;

    private Vector3 newPosition;
    private Quaternion newRotation;
    private float rotationXValue = 50;
    public float zoomValue;

    private float movementTime = 5f;
    private float Speed = 15;
    private float ZoomSpeed = 30f;

    private float RotationSpeed = -90;

    private Vector3 dragStartPosition;
    private Vector3 dragCurrentPosition;

    private Vector2 rotateStartPosition;
    private Vector2 rotateCurrentPosition;

    private Vector2 zoomStartPosition;
    private Vector2 zoomCurrentPosition;

    public int BuildingRotation;

    //Input
    GameActions inputActions;
    private Vector2 Input_Move;
    private float Input_Rotation;
    private float Input_UpDown;
    private Vector2 Input_MouseDelta;
    public Vector2 Input_MousePosition;
    public bool Input_MouseButtonLeft;
    private float Input_MouseButtonLeftTime;
    private bool Input_MouseButtonRight;
    private bool Input_MouseButtonMiddle;
    public bool IsMouseOnGUI;
    //

    [SerializeField] public bool Input_GameCameraMode;
    [SerializeField] private float Input_GameCameraModeTime;

    float UnselectingTime = 0.75f;

    public static CameraController singleton;

    public static bool IsUpdatingByUnit;

    private void Awake()
    {
        singleton = this;

        inputActions = new GameActions();
        inputActions.Gameplay.Move.performed += ctx => Input_Move = ctx.ReadValue<Vector2>();
        inputActions.Gameplay.Move.canceled += ctx => Input_Move = Vector2.zero;

        inputActions.Gameplay.Rotation.performed += ctx => Input_Rotation = ctx.ReadValue<float>();
        inputActions.Gameplay.Rotation.canceled += ctx =>
        {
            Input_Rotation = 0;
        };

        inputActions.Gameplay.UpDown.performed += ctx => Input_UpDown = ctx.ReadValue<float>();
        inputActions.Gameplay.UpDown.canceled += ctx => Input_UpDown = 0;

        inputActions.Gameplay.MouseScroll.performed += ctx => RotateScroll(ctx.ReadValue<float>());

        inputActions.Gameplay.MouseDelta.performed += ctx => Input_MouseDelta = ctx.ReadValue<Vector2>();
        inputActions.Gameplay.MouseDelta.canceled += ctx => Input_MouseDelta = Vector2.zero;

        inputActions.Gameplay.MousePosition.performed += ctx =>
        {
            Input_MousePosition = ctx.ReadValue<Vector2>();
            if ((!Input_MouseButtonRight && !Input_MouseButtonLeft && !Input_MouseButtonMiddle))
            {
                IsMouseOnGUI = raycasterUI.RayCastToResults(Input_MousePosition);
            }
        };


        inputActions.Gameplay.Boost.started += ctx =>
        {
            RotationSpeed = -180;
            Speed = 45;
        };

        inputActions.Gameplay.Boost.canceled += ctx =>
        {
            RotationSpeed = -90;
            Speed = 15;
        };


        inputActions.Gameplay.MouseLeft.started += ctx =>
        {
            Input_MouseButtonLeft = true;
            Input_MousePressed(0);
        };
        inputActions.Gameplay.MouseLeft.canceled += ctx =>
        {
            Input_MouseButtonLeft = false;
            Input_MouseCanceled(0);
        };

        inputActions.Gameplay.MouseRight.started += ctx =>
        {
            Input_MouseButtonRight = true;
            Input_MousePressed(1);
        };
        inputActions.Gameplay.MouseRight.canceled += ctx =>
        {
            Input_MouseButtonRight = false;
            Input_MouseCanceled(1);
        };

        inputActions.Gameplay.MouseMiddle.started += ctx =>
        {
            Input_MouseButtonMiddle = true;
            Input_MousePressed(2);
        };
        inputActions.Gameplay.MouseMiddle.canceled += ctx =>
        {
            Input_MouseButtonMiddle = false;
            Input_MouseCanceled(2);
        };


        inputActions.Gameplay.GameCameraMode.started += ctx =>
        {
            Input_GameCameraMode = true;
            Input_GameCameraModeTime = 0;
        };

        inputActions.Gameplay.GameCameraMode.canceled += ctx =>
        {
            Input_GameCameraMode = false;
            if (Input_GameCameraModeTime < 0.2f)
            {
                NextGameCameraMode();
            }
            else {
                SelectedGameCameraMode();
            }
        };

        inputActions.Gameplay.Pause.canceled += ctx =>
        {
            UIManager.singleton.SwitchMenu();
        };

    }

    public void CheckIsStillOnGUI() {
        IsMouseOnGUI = raycasterUI.RayCastToResults(Input_MousePosition);
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }


    void Start()
    {
        newPosition = transform.position;
        newRotation = transform.rotation;
        zoomValue = cameraTransform.GetComponent<Camera>().fieldOfView;
    }

    public void SetCameraTargetHeight(float height) {
        newPosition.y = height;
    }


    void NextGameCameraMode() {
        GameCameraMode.singleton.NextGameCameraMode();
    }

    void SelectedGameCameraMode()
    {
        GameCameraMode.singleton.SelectedGameCameraMode();
    }

    void Input_MouseCanceled(int mouseButton) {
        if (Ticker.IsControllPaused)
            return;

        IsUpdatingByUnit = false;
        UIManager.singleton.MouseUp(mouseButton);
        switch (mouseButton)
        {
            case 0:
                Input_MouseButtonLeftTime = 0;
                Input_Move = Vector2.zero;

                if (UnitManager.singleton.IsUnitSelected && UnitManager.singleton.SelectedUnit != null)
                {
                    UnitManager.singleton.SelectedTarget();
                }
                break;

            case 1:
                Input_Rotation = 0;

                break;

            case 2:

                break;
        }
    }

    void Input_MousePressed(int mouseButton)
    {
        if (Ticker.IsControllPaused)
            return;
        UIManager.singleton.MouseDown(mouseButton);
        switch (mouseButton) {
            case 0:
                Input_MouseButtonLeftTime = 0;

                var plane = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0));

                var ray = Camera.main.ScreenPointToRay(Input_MousePosition);

                float entery;

                if (plane.Raycast(ray, out entery))
                {
                    dragStartPosition = ray.GetPoint(entery);
                }

                break;

            case 1:
                rotateStartPosition = Input_MousePosition;

                break;

            case 2:
                zoomStartPosition = Input_MousePosition;

                break;
        }
    }

    void UpdateMouseInput()
    {
        if (IsMouseOnGUI) return;

        if (Input_MouseButtonLeft)
        {
            Input_MouseButtonLeftTime += Time.deltaTime;

            if (!UnitManager.singleton.IsUnitSelected)
            {
                Plane plane = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0));

                Ray ray = Camera.main.ScreenPointToRay(Input_MousePosition);

                float entery;

                if (plane.Raycast(ray, out entery))
                {
                    dragCurrentPosition = ray.GetPoint(entery);
                    var temp = transform.position + dragStartPosition - dragCurrentPosition;
                    newPosition = new Vector3(temp.x, newPosition.y, temp.z);
                }
            }
            else { 
                
            }
        }
        if (Input_MouseButtonRight) {
            rotateCurrentPosition = Input_MousePosition;

            Vector3 difference = rotateStartPosition - rotateCurrentPosition;

            rotateStartPosition = rotateCurrentPosition;

            newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / 8f));

            rotationXValue += difference.y / 8f;
            rotationXValue = Mathf.Clamp(rotationXValue, 45, 85);
        }

        if (Input_MouseButtonMiddle) {
            zoomCurrentPosition = Input_MousePosition;

            float difference = zoomStartPosition.y - zoomCurrentPosition.y;
            zoomStartPosition = zoomCurrentPosition;

            zoomValue += ZoomSpeed * difference * Time.deltaTime;
        }
    }

    void RotateScroll(float value)
    {
        if (value == 0 || Ticker.IsControllPaused) return;

        if (CardsManager.singleton.IsCardInHand && CardsManager.singleton.SelectedCard.CardClassType == CardClassType.Building)
        {
            BuildingRotation += (value > 0 ? 1 : -1);
            BuildingRotation = (BuildingRotation + 4) % 4;
            CardsManager.singleton.SetBuildingRotation(BuildingRotation);
        }
        else
        {
            zoomValue -= ZoomSpeed * value * 0.5f * Time.deltaTime;
        }
    }

    void UpdateCameraInput()
    {
        if (Ticker.IsControllPaused)
            return;
        //Rotation
        newRotation *= Quaternion.Euler(0, Input_Rotation * RotationSpeed * Time.deltaTime, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);

        cameraXRotor.localRotation = Quaternion.Lerp(
            cameraXRotor.localRotation, 
            Quaternion.Euler(rotationXValue, 0, 0), 
            Time.deltaTime * movementTime);

        //Move

        newPosition.x = Mathf.Clamp(newPosition.x, -5, 85);
        newPosition.z = Mathf.Clamp(newPosition.z, -5, 85);

        newPosition += newRotation * new Vector3(Input_Move.x, 0, Input_Move.y) * Speed * Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);

        //Zoom
        zoomValue += 45 * Time.deltaTime * Input_UpDown;
        UpdateZoom(movementTime);
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
    }

    public void UpdateZoom(float zoomSpeed)
    {
        zoomValue = Mathf.Clamp(zoomValue, 20, 100);
        cameraTransform.GetComponent<Camera>().fieldOfView = Mathf.Lerp(
            cameraTransform.GetComponent<Camera>().fieldOfView,
            zoomValue,
            Time.deltaTime * zoomSpeed);
    }

    void UpdateSelectingCameraMode() {
        if (Input_GameCameraMode) {
            Input_GameCameraModeTime += Time.deltaTime;
            if (Input_GameCameraModeTime > 0.2f) {
                CameraModeCircleSelector.singleton.SetIsVisible(true);
            }
        }
    }

    void Update()
    {
        if (Ticker.IsControllPaused)
            return;

        UpdateSelectingCameraMode();

        if (IsUpdatingByUnit)
        {
            CheckIsStillOnGUI();
        }

        if (UnitManager.singleton.IsUnitSelected && Selector.singleton.SelectedType == SelectedType.None)
        {
            UnselectingTime -= Time.deltaTime;
            UnitManager.singleton.RemoveWay();
            if (UnselectingTime <= 0)
            {
                UnitManager.singleton.SetTarget(null);

                var plane = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0));
                var ray = Camera.main.ScreenPointToRay(Input_MousePosition);
                float entery;
                if (plane.Raycast(ray, out entery))
                    dragStartPosition = ray.GetPoint(entery);

            }
        }
        else {
            UnselectingTime = 0.75f;
        }

        UpdateMouseInput();
        UpdateCameraInput();

        Selector.singleton.UpdateSelector(Input_MousePosition);
    }

    public void ForceSetNewPosition(Vector3 pos) {
        newPosition = pos;
        transform.position = pos;
    }
}
