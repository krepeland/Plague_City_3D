//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.3.0
//     from Assets/GameActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @GameActions : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @GameActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""GameActions"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""092bd37f-6962-4447-8e6f-20f80e45b99e"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""fc72e27e-47af-4036-a25a-9705ab0ca0b9"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Rotation"",
                    ""type"": ""Value"",
                    ""id"": ""48316f93-f78e-4af5-afc5-6838c6a224c6"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""UpDown"",
                    ""type"": ""Value"",
                    ""id"": ""b0016018-1a37-40bc-a2df-11b84167f208"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""MouseDelta"",
                    ""type"": ""Value"",
                    ""id"": ""6b884d65-40fd-401d-9d54-1817c0d24e9f"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""MouseLeft"",
                    ""type"": ""Button"",
                    ""id"": ""327aae59-d00e-49e0-92be-9c5d8fae9786"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""MouseRight"",
                    ""type"": ""Button"",
                    ""id"": ""8b815725-8783-4f44-9dde-a80fff4ad96e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""MousePosition"",
                    ""type"": ""Value"",
                    ""id"": ""a325de5b-c94c-4b19-a487-ea3e4aeef57a"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""MouseScroll"",
                    ""type"": ""Value"",
                    ""id"": ""325fd305-6045-4e1b-b988-7cdbe306b6eb"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Boost"",
                    ""type"": ""Button"",
                    ""id"": ""d8f03375-4df3-4cb2-a277-81054c258417"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""MouseMiddle"",
                    ""type"": ""Button"",
                    ""id"": ""47b480e4-85f4-4998-989a-0830e1f9fec6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""GameCameraMode"",
                    ""type"": ""Button"",
                    ""id"": ""0a8c5bde-ede5-403c-bb9a-8d0889c4c019"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""a45f7555-2bb9-4ea5-8c04-edb659737dda"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""ca460d36-2026-4331-a8e5-a76e4a655d31"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotation"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""66b1905f-0fb2-4e62-a20f-23fcad4ca65d"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""07590832-4dd2-4c3b-95be-499aef246bbe"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""2b0ce713-7c3b-42b5-87c1-7fc3a24572de"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""UpDown"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""ed3e1ec3-416e-40be-8554-ee76d22fe31a"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""UpDown"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""fc5c11f4-efdd-4ba3-8a6d-3f2663092741"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""UpDown"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""0606657d-2731-4a14-a139-3bfd61568159"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""fddcc6ad-d712-40ee-8def-83baf0c97549"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""c939617b-da6d-4ae1-9698-47f4567ef039"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""709809be-e263-48fd-87ce-3fbab863f7cf"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""ae21ccaa-b680-4638-a001-d39504f3c169"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""f1f4beeb-f270-44e4-9e7f-99f234f17fb8"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""54c66c40-aa65-443d-9a3e-a119f1bc8479"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""817fdee8-dff9-48f9-9760-24c29ab29f05"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""458bab6f-b1e5-4d67-902d-a75b6575dc80"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""dea7f6e6-3f99-40a0-98b9-f85ab9f450d9"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""3f510505-8bc8-4c06-bb2d-d415190760fe"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MouseDelta"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3954594d-95fe-4905-b348-5eaaf1153903"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MouseLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""95c907c0-c3dd-42fe-9501-dab35059f5cc"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MouseRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""69fb8106-fb37-4b05-a353-dd092caff2d1"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MousePosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""517316ab-b31d-47f9-a71d-478d76d49536"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MouseScroll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3a143a6f-6f87-4efa-809b-e4e28260348f"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Boost"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""79e41701-8528-4746-b57a-468647805a32"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MouseMiddle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""183f0927-83a2-476b-a1f9-d18f4062dde3"",
                    ""path"": ""<Keyboard>/z"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""GameCameraMode"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""da9549c7-d3b4-448c-87e3-baf65ee448a1"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""GameCameraMode"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""20397916-e39e-4904-83e3-d537736b8467"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Gameplay
        m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
        m_Gameplay_Move = m_Gameplay.FindAction("Move", throwIfNotFound: true);
        m_Gameplay_Rotation = m_Gameplay.FindAction("Rotation", throwIfNotFound: true);
        m_Gameplay_UpDown = m_Gameplay.FindAction("UpDown", throwIfNotFound: true);
        m_Gameplay_MouseDelta = m_Gameplay.FindAction("MouseDelta", throwIfNotFound: true);
        m_Gameplay_MouseLeft = m_Gameplay.FindAction("MouseLeft", throwIfNotFound: true);
        m_Gameplay_MouseRight = m_Gameplay.FindAction("MouseRight", throwIfNotFound: true);
        m_Gameplay_MousePosition = m_Gameplay.FindAction("MousePosition", throwIfNotFound: true);
        m_Gameplay_MouseScroll = m_Gameplay.FindAction("MouseScroll", throwIfNotFound: true);
        m_Gameplay_Boost = m_Gameplay.FindAction("Boost", throwIfNotFound: true);
        m_Gameplay_MouseMiddle = m_Gameplay.FindAction("MouseMiddle", throwIfNotFound: true);
        m_Gameplay_GameCameraMode = m_Gameplay.FindAction("GameCameraMode", throwIfNotFound: true);
        m_Gameplay_Pause = m_Gameplay.FindAction("Pause", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Gameplay
    private readonly InputActionMap m_Gameplay;
    private IGameplayActions m_GameplayActionsCallbackInterface;
    private readonly InputAction m_Gameplay_Move;
    private readonly InputAction m_Gameplay_Rotation;
    private readonly InputAction m_Gameplay_UpDown;
    private readonly InputAction m_Gameplay_MouseDelta;
    private readonly InputAction m_Gameplay_MouseLeft;
    private readonly InputAction m_Gameplay_MouseRight;
    private readonly InputAction m_Gameplay_MousePosition;
    private readonly InputAction m_Gameplay_MouseScroll;
    private readonly InputAction m_Gameplay_Boost;
    private readonly InputAction m_Gameplay_MouseMiddle;
    private readonly InputAction m_Gameplay_GameCameraMode;
    private readonly InputAction m_Gameplay_Pause;
    public struct GameplayActions
    {
        private @GameActions m_Wrapper;
        public GameplayActions(@GameActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Gameplay_Move;
        public InputAction @Rotation => m_Wrapper.m_Gameplay_Rotation;
        public InputAction @UpDown => m_Wrapper.m_Gameplay_UpDown;
        public InputAction @MouseDelta => m_Wrapper.m_Gameplay_MouseDelta;
        public InputAction @MouseLeft => m_Wrapper.m_Gameplay_MouseLeft;
        public InputAction @MouseRight => m_Wrapper.m_Gameplay_MouseRight;
        public InputAction @MousePosition => m_Wrapper.m_Gameplay_MousePosition;
        public InputAction @MouseScroll => m_Wrapper.m_Gameplay_MouseScroll;
        public InputAction @Boost => m_Wrapper.m_Gameplay_Boost;
        public InputAction @MouseMiddle => m_Wrapper.m_Gameplay_MouseMiddle;
        public InputAction @GameCameraMode => m_Wrapper.m_Gameplay_GameCameraMode;
        public InputAction @Pause => m_Wrapper.m_Gameplay_Pause;
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void SetCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                @Rotation.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRotation;
                @Rotation.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRotation;
                @Rotation.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRotation;
                @UpDown.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnUpDown;
                @UpDown.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnUpDown;
                @UpDown.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnUpDown;
                @MouseDelta.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMouseDelta;
                @MouseDelta.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMouseDelta;
                @MouseDelta.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMouseDelta;
                @MouseLeft.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMouseLeft;
                @MouseLeft.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMouseLeft;
                @MouseLeft.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMouseLeft;
                @MouseRight.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMouseRight;
                @MouseRight.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMouseRight;
                @MouseRight.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMouseRight;
                @MousePosition.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMousePosition;
                @MousePosition.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMousePosition;
                @MousePosition.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMousePosition;
                @MouseScroll.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMouseScroll;
                @MouseScroll.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMouseScroll;
                @MouseScroll.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMouseScroll;
                @Boost.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnBoost;
                @Boost.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnBoost;
                @Boost.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnBoost;
                @MouseMiddle.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMouseMiddle;
                @MouseMiddle.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMouseMiddle;
                @MouseMiddle.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMouseMiddle;
                @GameCameraMode.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnGameCameraMode;
                @GameCameraMode.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnGameCameraMode;
                @GameCameraMode.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnGameCameraMode;
                @Pause.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPause;
                @Pause.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPause;
                @Pause.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPause;
            }
            m_Wrapper.m_GameplayActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Rotation.started += instance.OnRotation;
                @Rotation.performed += instance.OnRotation;
                @Rotation.canceled += instance.OnRotation;
                @UpDown.started += instance.OnUpDown;
                @UpDown.performed += instance.OnUpDown;
                @UpDown.canceled += instance.OnUpDown;
                @MouseDelta.started += instance.OnMouseDelta;
                @MouseDelta.performed += instance.OnMouseDelta;
                @MouseDelta.canceled += instance.OnMouseDelta;
                @MouseLeft.started += instance.OnMouseLeft;
                @MouseLeft.performed += instance.OnMouseLeft;
                @MouseLeft.canceled += instance.OnMouseLeft;
                @MouseRight.started += instance.OnMouseRight;
                @MouseRight.performed += instance.OnMouseRight;
                @MouseRight.canceled += instance.OnMouseRight;
                @MousePosition.started += instance.OnMousePosition;
                @MousePosition.performed += instance.OnMousePosition;
                @MousePosition.canceled += instance.OnMousePosition;
                @MouseScroll.started += instance.OnMouseScroll;
                @MouseScroll.performed += instance.OnMouseScroll;
                @MouseScroll.canceled += instance.OnMouseScroll;
                @Boost.started += instance.OnBoost;
                @Boost.performed += instance.OnBoost;
                @Boost.canceled += instance.OnBoost;
                @MouseMiddle.started += instance.OnMouseMiddle;
                @MouseMiddle.performed += instance.OnMouseMiddle;
                @MouseMiddle.canceled += instance.OnMouseMiddle;
                @GameCameraMode.started += instance.OnGameCameraMode;
                @GameCameraMode.performed += instance.OnGameCameraMode;
                @GameCameraMode.canceled += instance.OnGameCameraMode;
                @Pause.started += instance.OnPause;
                @Pause.performed += instance.OnPause;
                @Pause.canceled += instance.OnPause;
            }
        }
    }
    public GameplayActions @Gameplay => new GameplayActions(this);
    public interface IGameplayActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnRotation(InputAction.CallbackContext context);
        void OnUpDown(InputAction.CallbackContext context);
        void OnMouseDelta(InputAction.CallbackContext context);
        void OnMouseLeft(InputAction.CallbackContext context);
        void OnMouseRight(InputAction.CallbackContext context);
        void OnMousePosition(InputAction.CallbackContext context);
        void OnMouseScroll(InputAction.CallbackContext context);
        void OnBoost(InputAction.CallbackContext context);
        void OnMouseMiddle(InputAction.CallbackContext context);
        void OnGameCameraMode(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
    }
}
