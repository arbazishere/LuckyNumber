using System.Collections.Generic;
using UnityEngine;
using Unity.Scripting.LifecycleManagement;
using ArbazAminKhan.Inputs.Components;
using ArbazAminKhan.Inputs.Datas;
using System;

namespace ArbazAminKhan.Transforms.Components
{
    [System.Serializable]
    public class Grabber_Transform_Data
    {
        internal Transform_Grabber _component;
        internal Touch_Data _touchData;
        public float _screenDepth;
        internal Vector3 _offset;
        internal Vector3 _screenPoint;
    }

    public partial class Grabber_Transform_Manager : MonoBehaviour
    {
        [NonSerialized] public Dictionary<Touch_Data, Grabber_Transform_Data> _datas = new();

        [AutoStaticsCleanup] public static Grabber_Transform_Manager Instance { get; private set; }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        void OnEnable()
        {
            Touch_Manager.OnPreDown -= OnTouchDown;
            Touch_Manager.OnPreDown += OnTouchDown;

            Touch_Manager.OnPreMove -= OnTouchMove;
            Touch_Manager.OnPreMove += OnTouchMove;

            Touch_Manager.OnPreUp -= OnTouchUp;
            Touch_Manager.OnPreUp += OnTouchUp;
        }

        void OnDisable()
        {
            Touch_Manager.OnPreDown -= OnTouchDown;
            Touch_Manager.OnPreMove -= OnTouchMove;
            Touch_Manager.OnPreUp -= OnTouchUp;
        }

        void OnDestroy()
        {
            Touch_Manager.OnPreDown -= OnTouchDown;
            Touch_Manager.OnPreMove -= OnTouchMove;
            Touch_Manager.OnPreUp -= OnTouchUp;

            if (Instance == this) Instance = null;
        }

        void OnTouchDown(Touch_Data touchData)
        {
            if (touchData._hitData._hasRaycastData && touchData._hitData._data.transform.TryGetComponent(out IGrabber iGrabber) && iGrabber.GetGrabber().Interactable)
            {
                var data = _datas.ContainsKey(touchData) ? _datas[touchData] : new();

                data._touchData = touchData;

                data._component = iGrabber.GetGrabber();

                data._screenPoint = Camera.main.WorldToScreenPoint(data._component.Main.position);
                data._screenDepth = Camera.main.WorldToScreenPoint(data._component.Main.position).z;
                data._offset = data._component.Main.position - ConvertScreenToWorldPos(data);

                if(!_datas.ContainsKey(touchData)) _datas.Add(touchData, data);

                data._component.DownBehaviour(data);
            }
        }

        void OnTouchMove(Touch_Data touchData)
        {
            if (!_datas.ContainsKey(touchData)) return;

            var data = _datas[touchData];
            data._component.Main.position = ConvertScreenToWorldPos(data) + data._offset;

            data._component.MoveBehaviour(data);
        }

        void OnTouchUp(Touch_Data touchData) => RemoveBehaviour(touchData);

        public void RemoveBehaviour(Touch_Data touchData)
        {
            if (_datas.ContainsKey(touchData))
            {
                var data = _datas[touchData];
                data._component.UpBehaviour(data);
            }

            _datas.Remove(touchData);
        }

        Vector3 ConvertScreenToWorldPos(Grabber_Transform_Data data)
        {
            Vector2 position = default;

            position = data._touchData.position;

            Vector3 vector = Camera.main.ScreenToWorldPoint(new Vector3(position.x, position.y, data._screenPoint.z));
            return vector != Camera.main.transform.position ? vector : data._component.Main.position;
        }

        public bool IsGrabbed => _datas.Count > 0;
    }
}
