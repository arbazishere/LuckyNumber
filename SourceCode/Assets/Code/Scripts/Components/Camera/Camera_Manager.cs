using ArbazAminKhan.Coroutines.Components;
using ArbazAminKhan.Coroutines.Datas;
using ArbazAminKhan.Datas;
using ArbazAminKhan.Frequentlys.Components;
using ArbazAminKhan.Inputs.Components;
using ArbazAminKhan.Inputs.Datas;
using ArbazAminKhan.Presets;
using ArbazAminKhan.Transforms.Components;
using ArbazAminKhan.Transforms.Datas;
using ArbazAminKhan.Transforms.Presets;
using System;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;

namespace ArbazAminKhan.Cameras.Components
{
    public partial class Camera_Manager : MonoBehaviour
    {
        public Transform _root;
        public Camera _camera;
        [SerializeField] internal Focus_Camera _focusCamera;
        [SerializeField] internal Zoom_Manager _zoomManager;

        const float _speed = 1 * Value_Presets.m_256Value;

        Touch_Data _touchData = null;

        public bool RotationLocked { get; set; }

        [SerializeField]
        Vector3MinMax_Data _rotationClamp = new()
        {
            _min = new(-90, -180, 0),
            _max = new(90, 180, 0),
        };

        //--------
        internal readonly byte _transformRoutineSpeed = 180;
        Coroutine_Behaviour_Data<Transform_MotionBehaviour_Container> _transformRoutineData = new() { _behaviour = new() };
        Coroutine _cameraDistanceRoutine;
        Coroutine _fieldOfViewRoutine;
        //--------

        [AutoStaticsCleanup] public static Camera_Manager Instance { get; private set; }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            _touchData = null;
        }

        void OnEnable()
        {
            Subscribe();
        }

        void OnDisable()
        {
            Unsubscribe();
        }

        void Subscribe()
        {
            Unsubscribe();

            Frequently_Manager.OnUpdate += UpdateBehaviour;

            //Touch_Manager.OnDown += TouchDown;
            //Touch_Manager.OnUp += TouchUp;
        }

        void Unsubscribe()
        {
            Frequently_Manager.OnUpdate -= UpdateBehaviour;
            //Frequently_Manager.OnUpdate -= Touch_UpdateBehaviour;
            
            //Touch_Manager.OnDown -= TouchDown;
            //Touch_Manager.OnUp -= TouchUp;
        }

        void UpdateBehaviour()
        {
            _zoomManager.UpdateBehaviour();
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        //void TouchDown(Touch_Data data)
        //{
        //    Frequently_Manager.OnUpdate -= Touch_UpdateBehaviour;
        //
        //    _touchData = Touch_Manager._noneUI_TouchsDown_Data.Length == 1 ? Touch_Manager._noneUI_TouchsDown_Data[0] : null;
        //
        //    if (_touchData != null && _touchData._hitData._visualElements.Count == 0 && !Grabber_Transform_Manager.Instance._datas.ContainsKey(_touchData))
        //    {
        //        Frequently_Manager.OnUpdate += Touch_UpdateBehaviour;
        //    }
        //    else _touchData = null;
        //}
        //
        //void Touch_UpdateBehaviour()
        //{
        //    if (_touchData == null) Frequently_Manager.OnUpdate -= Touch_UpdateBehaviour; 
        //    if (RotationLocked) return;
        //    
        //    SetRotation(_touchData.delta_ScreenModified);
        //}
        //
        //void TouchUp(Touch_Data data)
        //{
        //    if (_touchData != data) return;
        //
        //    Frequently_Manager.OnUpdate -= Touch_UpdateBehaviour;
        //    _touchData = null;
        //}

        public void SetRotation(Vector2 delta)
        {
            if (RotationLocked) return;

            delta *= _speed;

            var euler = _root.localEulerAngles + new Vector3(-delta.y, delta.x, 0);
            euler.z = 0;

            var max = _rotationClamp._max.x;
            var min = _rotationClamp._min.x;
            if (min < 0) min = 360 + min;

            //Manually correct x clamp
            if (euler.x > max && euler.x < 180) euler.x = max;
            else if ((euler.x > 180 && euler.x < min) || (min >= 0 && min <= max && euler.x < min)) euler.x = min;

            _root.localEulerAngles = euler;
        }

        public void ReFocus() => _focusCamera.FitAllRenderers();

        public float FieldOfView
        {
            get => _camera.fieldOfView;
            set => _camera.fieldOfView = value;
        }

        public float CameraDistance
        {
            get => Mathf.Abs(_camera.transform.localPosition.z);
            set => _camera.transform.localPosition = new Vector3(_camera.transform.localPosition.x, _camera.transform.localPosition.y, -value);
        }

        public void SetCameraDistanceData(float target, Action complete = null)
        {
            var current = CameraDistance;
            var speed = Mathf.Abs(current - target) * 4;

            Coroutine_Manager.Instance.StopRoutine(_cameraDistanceRoutine);
            _cameraDistanceRoutine = Coroutine_Manager.Instance.TimeChange((v) =>
            {
                current = Mathf.MoveTowards(current, target, speed * Time.deltaTime);
                CameraDistance = current;

                if (current == target)
                {
                    Coroutine_Manager.Instance.StopRoutine(_cameraDistanceRoutine);
                    complete?.Invoke();
                }
            });
        }

        public void SetFieldOfViewData(float target, Action complete = null)
        {
            var current = FieldOfView;
            var speed = Mathf.Abs(current - target) * 4;

            Coroutine_Manager.Instance.StopRoutine(_fieldOfViewRoutine);
            _fieldOfViewRoutine = Coroutine_Manager.Instance.TimeChange((v) =>
            {
                current = Mathf.MoveTowards(current, target, speed * Time.deltaTime);
                FieldOfView = current;

                if (current == target)
                {
                    Coroutine_Manager.Instance.StopRoutine(_fieldOfViewRoutine);
                    complete?.Invoke();
                }
            });
        }

        public ref Coroutine_Behaviour_Data<Transform_MotionBehaviour_Container> SetObjectData(Transform_Data data, Action<Transform_Data> complete)
        {
            var speed = Quaternion.Angle(Camera_Manager.Instance._root.transform.rotation, data.GetGlobalRotation()) * 4;
            SetSpeedMotion(_transformRoutineData, _root, data, 0, 0, speed, complete);
            return ref _transformRoutineData;
        }

        void SetSpeedMotion(Coroutine_Behaviour_Data<Transform_MotionBehaviour_Container> routineData, Transform component, Transform_Data behaviour, int currentIndex, int targetIndex, float speed = 1, System.Action<Transform_Data> OnComplete = null, System.Action<Transform_Data> OnIndexComplete = null, System.Action OnChange = null)
        {
            SetSpeedMotion(routineData, component, new Transform_Data[] { behaviour }, 0, 0, speed, OnComplete, OnIndexComplete, OnChange);
        }

        void SetSpeedMotion(Coroutine_Behaviour_Data<Transform_MotionBehaviour_Container> routineData, Transform component, Transform_Data[] behaviours, int currentIndex, int targetIndex, float speed = 1, System.Action<Transform_Data> OnComplete = null, System.Action<Transform_Data> OnIndexComplete = null, System.Action OnChange = null)
        {
            routineData._behaviour._component = component;
            routineData._behaviour._behaviours = behaviours;
            routineData._behaviour._index = currentIndex;
            routineData._behaviour._targetIndex = targetIndex;
            routineData._behaviour._speed = speed;

            Transform_Presets.SetSpeedMotion(routineData, OnComplete, OnIndexComplete, OnChange);
        }
    }
}
