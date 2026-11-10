using ArbazAminKhan.Frequentlys.Components;
using ArbazAminKhan.Inputs.Components;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Input_Manager : MonoBehaviour
{
    [SerializeField] InputActionAsset _inputActions;
    internal InputAction _moveAction, _lookAction, _jumpAction, _sprintAction, _interactionAction;

    Vector2 _move;
    Vector2 _look;
    bool _jump;
    bool _sprint;

    public Vector2 Move => _move;
    public Vector2 Look => _look;
    public bool Jump => _jump;
    public bool Sprint => _sprint;

    readonly string _playerActionMapId = "Player";

    readonly string _moveActionId = "Move";
    readonly string _lookActionId = "Look";
    readonly string _jumpActionId = "Jump";
    readonly string _sprintActionId = "Sprint";
    readonly string _interactionActionId = "Interact";

    public Action OnJumpedBegan;

    public static Input_Manager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(this.gameObject);
            return;
        }

        Instance = this;

        _moveAction = InputSystem.actions.FindAction(_moveActionId);
        _lookAction = InputSystem.actions.FindAction(_lookActionId);
        _jumpAction = InputSystem.actions.FindAction(_jumpActionId);
        _sprintAction = InputSystem.actions.FindAction(_sprintActionId);
        _interactionAction = InputSystem.actions.FindAction(_interactionActionId);

        Frequently_Manager.OnUpdate += UpdateBehaviour;
    }

    void OnEnable()
    {
        _inputActions.FindActionMap(_playerActionMapId).Enable();
    }

    void UpdateBehaviour()
    {
        _move = _moveAction.ReadValue<Vector2>();
        _look = GetTouchLook();

        _sprint = _sprintAction.IsPressed();
        _jump = _jumpAction.IsPressed();

        if (_jumpAction.WasPressedThisFrame()) OnJumpedBegan?.Invoke();
    }

    // Camera look comes from Touch_Manager: only presses that did not start on UI rotate the camera.
    static Vector2 GetTouchLook()
    {
        var datas = Touch_Manager._noneUI_TouchsDown_Data;
        for (int i = 0; i < datas.Length; i++)
            if (datas[i] != null && datas[i].IsDown) return datas[i].delta;

        return Vector2.zero;
    }

    void OnDisable()
    {
        _inputActions.FindActionMap(_playerActionMapId).Disable();
    }

    void OnDestroy()
    {
        Frequently_Manager.OnUpdate -= UpdateBehaviour;
    }
}
