using ArbazAminKhan.Cameras.Components;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    [SerializeField] Input_Manager _input;
    [SerializeField] Character_Controller _controller;

    void Start()
    {
        if (_input == null) _input = Input_Manager.Instance;
        if (_input != null) _input.OnJumpedBegan += JumpInputBegan;
    }

    void Update()
    {
        if(_controller != null)
        {
            if(_input != null)
            {
                _controller._move = _input.Move;
                Camera_Manager.Instance.SetRotation(_input.Look / Mathf.Min(Screen.width, Screen.height));
                //_controller._look = _input.Look;
                _controller._sprint = _input.Sprint;
            }

            _controller.UpdateBehaviour();
        }
    }

    void JumpInputBegan() => _controller.JumpInputBegan();

    void OnDestroy()
    {
        if (_input != null) _input.OnJumpedBegan -= JumpInputBegan;
    }
}
