using ArbazAminKhan.Editor.Presets;
using ArbazAminKhan.Frequentlys.Components;
using ArbazAminKhan.Inputs.Datas;
using System;
using System.Linq;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ArbazAminKhan.Inputs.Components
{
    public partial class Touch_Manager : MonoBehaviour
    {
        [AutoStaticsCleanup] public static System.Collections.Generic.List<Touch_Data> _datas = new();

        [AutoStaticsCleanup] public static Touch_Data[] _noneUI_TouchsDown_Data = Array.Empty<Touch_Data>();

        [AutoStaticsCleanup] public static System.Action<Touch_Data> OnPreDown, OnPreMove, OnPreUp;
        [AutoStaticsCleanup] public static System.Action<Touch_Data> OnDown, OnMove, OnUp;

        Touch_Data[] _touchesData = Array.Empty<Touch_Data>();
        Touch_Data _mouseData = new() { _touchControl = null };

        bool _isInitialize_Awake = false;

        void Awake()
        {
            Awake_Initialize();

            InputSystem.onDeviceChange += DeviceChange;
        }

        void OnEnable()
        {
            Frequently_Manager.OnPreUpdate -= PreUpdate_Behaviour;
            Frequently_Manager.OnPreUpdate += PreUpdate_Behaviour;
        }

        void OnDisable()
        {
            Frequently_Manager.OnPreUpdate -= PreUpdate_Behaviour;
        }

        void OnDestroy()
        {
            Frequently_Manager.OnPreUpdate -= PreUpdate_Behaviour;

            InputSystem.onDeviceChange -= DeviceChange;
        }

        public void Awake_Initialize()
        {
            if (_isInitialize_Awake) return;
            _isInitialize_Awake = true;

            RecatchTouchData();
            _mouseData = new();
        }

        void DeviceChange(InputDevice device, InputDeviceChange change)
        {
            RecatchTouchData();
        }

        void RecatchTouchData()
        {
            var touchscreen = Touchscreen.current;
            if (touchscreen == null)
            {
                _touchesData = Array.Empty<Touch_Data>();
                return;
            }

            if (_touchesData.Length != touchscreen.touches.Count)
            {
                System.Collections.Generic.List<Touch_Data> touchsData = new();
                for (int i = 0; i < touchscreen.touches.Count; i++)
                {
                    if (i < _touchesData.Length && _touchesData[i] != null) touchsData.Add(_touchesData[i]);
                    else touchsData.Add(new());

                    touchsData[i]._touchControl = touchscreen.touches[i];
                }

                _touchesData = touchsData.ToArray();
            }
            else
            {
                for (byte i = 0; i < _touchesData.Length; i++) _touchesData[i]._touchControl = touchscreen.touches[i];
            }
        }

        void PreUpdate_Behaviour()
        {
            Update_Touch();
            Update_Mouse();
        }

        void Update_Touch()
        {
            if (_touchesData == null) return;

            foreach (var data in _touchesData)
            {
                if (data._touchControl.press.wasPressedThisFrame) DownBehaviour(data);
                if (data._touchControl.press.isPressed) MoveBehaviour(data);
                if (data._touchControl.press.wasReleasedThisFrame) UpBehaviour(data);
            }
        }

        void Update_Mouse()
        {
            var mouse = Mouse.current;
            if (mouse == null) return;

            if (mouse.leftButton.wasPressedThisFrame) DownBehaviour(_mouseData);
            if (mouse.leftButton.isPressed) MoveBehaviour(_mouseData);
            if (mouse.leftButton.wasReleasedThisFrame) UpBehaviour(_mouseData);
        }

        void DownBehaviour(Touch_Data data)
        {
            data._hitData._hasRaycastData = Physics.Raycast(Camera.main.ScreenPointToRay(data.position), out data._hitData._data);
            data._hitData._visualElements = UIToolkit_Inputs.GetUIElements(data.position);

            if (!_datas.Contains(data)) _datas.Add(data);

            RemoveNoneUseTouch_DownBehaviour();
            RefreshAndSet_NoneRayTouchData();

            OnPreDown?.Invoke(data);
            OnDown?.Invoke(data);
        }

        void MoveBehaviour(Touch_Data data)
        {
            OnPreMove?.Invoke(data);
            OnMove?.Invoke(data);
        }

        void UpBehaviour(Touch_Data data)
        {
            _datas.Remove(data);
            RefreshAndSet_NoneRayTouchData();

            OnPreUp?.Invoke(data);
            OnUp?.Invoke(data);
        }

        void RefreshAndSet_NoneRayTouchData() => _noneUI_TouchsDown_Data = _datas.FindAll(d => d._hitData._visualElements.Count == 0).ToArray();

        void RemoveNoneUseTouch_DownBehaviour()
        {
            var removeData = _datas.Where(d => d == null || !d.IsDown);
            _datas.RemoveAll(d => removeData.Contains(d));
            
            RefreshAndSet_NoneRayTouchData();

            foreach (var data in removeData) OnUp?.Invoke(data);
        }
    }
}
