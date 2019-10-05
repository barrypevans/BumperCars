// GENERATED AUTOMATICALLY FROM 'Assets/PlayerInput.inputactions'

using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class PlayerInput : IInputActionCollection
{
    private InputActionAsset asset;
    public PlayerInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInput"",
    ""maps"": [
        {
            ""name"": ""Movement"",
            ""id"": ""259606d1-3ea2-4448-8211-ad7baa1fe686"",
            ""actions"": [
                {
                    ""name"": ""LeftStick"",
                    ""type"": ""Button"",
                    ""id"": ""2460bc8b-450b-414a-8bc1-6ceab97c81d7"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RightStick"",
                    ""type"": ""Button"",
                    ""id"": ""5d324821-b0d9-4229-8b10-699d006f4014"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Accelerator"",
                    ""type"": ""Button"",
                    ""id"": ""91785921-a93b-4de0-b7d8-3288a853efb7"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""c85a6a12-cd20-427e-ac09-e5c579c9249a"",
                    ""path"": ""<DualShockGamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": ""AxisDeadzone(min=0.1)"",
                    ""groups"": """",
                    ""action"": ""LeftStick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3d44bee9-9b3d-48af-812f-e2d6ecb77939"",
                    ""path"": ""<DualShockGamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": ""AxisDeadzone(min=0.1)"",
                    ""groups"": """",
                    ""action"": ""RightStick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""12e27682-a7c3-4bd7-8c7c-d99534568063"",
                    ""path"": ""<DualShockGamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Accelerator"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Movement
        m_Movement = asset.FindActionMap("Movement", throwIfNotFound: true);
        m_Movement_LeftStick = m_Movement.FindAction("LeftStick", throwIfNotFound: true);
        m_Movement_RightStick = m_Movement.FindAction("RightStick", throwIfNotFound: true);
        m_Movement_Accelerator = m_Movement.FindAction("Accelerator", throwIfNotFound: true);
    }

    ~PlayerInput()
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

    // Movement
    private readonly InputActionMap m_Movement;
    private IMovementActions m_MovementActionsCallbackInterface;
    private readonly InputAction m_Movement_LeftStick;
    private readonly InputAction m_Movement_RightStick;
    private readonly InputAction m_Movement_Accelerator;
    public struct MovementActions
    {
        private PlayerInput m_Wrapper;
        public MovementActions(PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @LeftStick => m_Wrapper.m_Movement_LeftStick;
        public InputAction @RightStick => m_Wrapper.m_Movement_RightStick;
        public InputAction @Accelerator => m_Wrapper.m_Movement_Accelerator;
        public InputActionMap Get() { return m_Wrapper.m_Movement; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MovementActions set) { return set.Get(); }
        public void SetCallbacks(IMovementActions instance)
        {
            if (m_Wrapper.m_MovementActionsCallbackInterface != null)
            {
                LeftStick.started -= m_Wrapper.m_MovementActionsCallbackInterface.OnLeftStick;
                LeftStick.performed -= m_Wrapper.m_MovementActionsCallbackInterface.OnLeftStick;
                LeftStick.canceled -= m_Wrapper.m_MovementActionsCallbackInterface.OnLeftStick;
                RightStick.started -= m_Wrapper.m_MovementActionsCallbackInterface.OnRightStick;
                RightStick.performed -= m_Wrapper.m_MovementActionsCallbackInterface.OnRightStick;
                RightStick.canceled -= m_Wrapper.m_MovementActionsCallbackInterface.OnRightStick;
                Accelerator.started -= m_Wrapper.m_MovementActionsCallbackInterface.OnAccelerator;
                Accelerator.performed -= m_Wrapper.m_MovementActionsCallbackInterface.OnAccelerator;
                Accelerator.canceled -= m_Wrapper.m_MovementActionsCallbackInterface.OnAccelerator;
            }
            m_Wrapper.m_MovementActionsCallbackInterface = instance;
            if (instance != null)
            {
                LeftStick.started += instance.OnLeftStick;
                LeftStick.performed += instance.OnLeftStick;
                LeftStick.canceled += instance.OnLeftStick;
                RightStick.started += instance.OnRightStick;
                RightStick.performed += instance.OnRightStick;
                RightStick.canceled += instance.OnRightStick;
                Accelerator.started += instance.OnAccelerator;
                Accelerator.performed += instance.OnAccelerator;
                Accelerator.canceled += instance.OnAccelerator;
            }
        }
    }
    public MovementActions @Movement => new MovementActions(this);
    public interface IMovementActions
    {
        void OnLeftStick(InputAction.CallbackContext context);
        void OnRightStick(InputAction.CallbackContext context);
        void OnAccelerator(InputAction.CallbackContext context);
    }
}
