using ArbazAminKhan.Coroutines.Components;
using ArbazAminKhan.Coroutines.Datas;
using ArbazAminKhan.Editor.Presets;
using ArbazAminKhan.Enums.Datas;
using ArbazAminKhan.Transforms.Datas;
using ArbazAminKhan.Transforms.Presets;
using UnityEngine;

namespace ArbazAminKhan.Transforms.Presets
{
    public static class Transform_Presets
    {
        #region Global

        public static Vector3 GetGlobalPosition(Vector3 localPosition, Transform parent)
            => parent != null ? parent.TransformPoint(localPosition) : localPosition;

        public static Quaternion GetGlobalRotation(Quaternion localRotation, Transform parent)
            => parent != null ? parent.rotation * localRotation : localRotation;
        public static Vector3 GetGlobalEulerAngles(Vector3 localEulerAngles, Transform parent)
            => GetGlobalRotation(Quaternion.Euler(localEulerAngles), parent).eulerAngles;

        public static Vector3 GetGlobalLossyScale(Vector3 localScale, Transform parent)
            => parent != null ? GetGlobalLossyScale(localScale, parent.lossyScale) : localScale;
        public static Vector3 GetGlobalLossyScale(Vector3 localScale, Vector3 parentGlobalLossyScale)
            => new Vector3(localScale.x * parentGlobalLossyScale.x, localScale.y * parentGlobalLossyScale.y, localScale.z * parentGlobalLossyScale.z);

        #endregion

        #region Local

        public static Quaternion GetLocalRotation(Quaternion worldRotation, Transform parent = null) => parent != null ? Quaternion.Inverse(parent.rotation) * worldRotation : worldRotation;
        public static Vector3 GetLocalScale(Vector3 lossyScale, Transform parent)
        {
            if (parent != null)
            {
                var parentGlobalScale = parent.lossyScale;

                //Convert to divide to multiler and set, divide by zero Error (value != 0 ? (1 / value) : 0)
                lossyScale.x *= (parentGlobalScale.x != 0) ? (1.0f / parentGlobalScale.x) : 0;
                lossyScale.y *= (parentGlobalScale.y != 0) ? (1.0f / parentGlobalScale.y) : 0;
                lossyScale.z *= (parentGlobalScale.z != 0) ? (1.0f / parentGlobalScale.z) : 0;
            }

            return lossyScale;
        }

        #endregion

        public static void SetSpeedMotion(Coroutine_Behaviour_Data<Transform_MotionBehaviour_Container> routineData, System.Action<Transform_Data> OnComplete = null, System.Action<Transform_Data> OnIndexComplete = null, System.Action OnChange = null)
        {
            var behaviour = routineData._behaviour;

            if (behaviour._startData == null)
            {
                behaviour._startData = new();
                behaviour._startData.SetData(behaviour._component, behaviour._behaviours[behaviour._index]._mask, behaviour._behaviours[behaviour._index]._data._parent);
                behaviour._lastData = behaviour._startData;
            }

            Transform_Data targetData = null;
            behaviour._index = (byte)Mathf.Repeat(behaviour._index, behaviour._behaviours.Length);
            behaviour._targetIndex = Mathf.Clamp(behaviour._targetIndex, 0, behaviour._behaviours.Length - 1);

            routineData.StopRoutine();
            routineData._coroutine = Coroutine_Manager.Instance.TimeChange(t =>
            {
                bool isCompleted = true;

                var targetData = behaviour._behaviours[behaviour._index];

                if (targetData._mask.HasFlag(TransformAxes3_Flags.position))
                {
                    var targetValue = targetData.GetGlobalPosition();
                    var currentValue = Vector3.MoveTowards(behaviour._component.position, targetValue, behaviour._speed * Time.deltaTime); 

                    behaviour._component.position = currentValue;
                    if (currentValue != targetValue) isCompleted = false;
                }

                if (targetData._mask.HasFlag(TransformAxes3_Flags.rotation))
                {
                    var targetValue = targetData.GetGlobalRotation();
                    var currentValue = Quaternion.RotateTowards(behaviour._component.rotation, targetValue, behaviour._speed * Time.deltaTime); 

                    behaviour._component.rotation = currentValue;
                    if (currentValue != targetValue) isCompleted = false;
                }

                if (targetData._mask.HasFlag(TransformAxes3_Flags.scale))
                {
                    var targetValue = targetData.GetGlobalLossyScale();
                    var currentValue = Vector3.MoveTowards(behaviour._component.lossyScale, targetValue, behaviour._speed * Time.deltaTime);

                    behaviour._component.localScale = Transform_Presets.GetLocalScale(currentValue, behaviour._component.parent);
                    if (currentValue != targetValue) isCompleted = false;
                }

                OnChange?.Invoke();
                behaviour.OnChange?.Invoke();

                if (isCompleted)
                {
                    OnIndexComplete?.Invoke(targetData);

                    if (behaviour._index == behaviour._targetIndex)
                    {
                        routineData.StopRoutine();
                        OnComplete?.Invoke(targetData);
                        behaviour.OnComplete?.Invoke(targetData);
                    }
                    else
                    {
                        behaviour._lastData = behaviour._behaviours[behaviour._index];
                        behaviour._index = (int)Mathf.MoveTowards(behaviour._index, behaviour._targetIndex, 1);
                    }
                }

            }, (t) => behaviour.OnComplete?.Invoke(targetData));
        }

    }
}

namespace ArbazAminKhan.Transforms.Datas
{
    #region Data

    public class Transform_MotionBehaviour_Container
    {
        public Transform _component;

        [Space(Value_Editor_Presets.m_8ByteInspectorSpace)]
        public Transform_Data _startData;
        public Transform_Data _lastData;

        public int _index;
        public int _targetIndex = int.MaxValue;

        public Transform_Data[] _behaviours = new Transform_Data[default];

        public float _time;
        public float _speed;

        public System.Action OnChange;
        public System.Action<Transform_Data> OnComplete;

        public ValueMotionData _durationData;

        //public float GetTime()
        //{
        //    var index = _index;
        //    float time = Vector3.Distance(_component.transform.position, _behaviours[index].GetGlobalPosition()) / _speed;
        //    while (_index != _targetIndex)
        //    {
        //        var nextIndex = (int)Mathf.MoveTowards(_index, _targetIndex, 1);
        //        time += Vector3.Distance(_behaviours[index].GetGlobalPosition(), _behaviours[nextIndex].GetGlobalPosition()) / _speed;
        //        index = nextIndex;
        //    }

        //    return time;
        //}
    }

    public class ValueMotionData
    {
        public float _value;
        public Transform3ValueAxes_Mask_Data _overrideValue = new();

        public float position => _overrideValue._mask.HasFlag(TransformAxes3_Flags.position) ? _overrideValue.position : _value;
        public float rotation => _overrideValue._mask.HasFlag(TransformAxes3_Flags.rotation) ? _overrideValue.rotation : _value;
        public float scale => _overrideValue._mask.HasFlag(TransformAxes3_Flags.scale) ? _overrideValue.scale : _value;
    }

    [System.Serializable]
    public class Transform_Data
    {
        public Transform _trackingTarget;
        public TransformAxes_Parent_Data _data = new();

        [Space(Value_Editor_Presets.m_8ByteInspectorSpace)]
        public TransformAxes3_Flags _mask;

        public void Initialize() /*=> _data.Initialize();*/{ }

        public Vector3 GetGlobalPosition() => (_trackingTarget != null ? _trackingTarget.position : _data.GetGlobalPosition())/* + _data._initializedExtents.position + _data._offset.position*/;
        public Vector3 GetGlobalEulerAngles() => _trackingTarget != null ? _trackingTarget.eulerAngles : _data.GetGlobalEulerAngles()/* + _data._initializedExtents.eulerAngles + _data._offset.eulerAngles*/;
        public Quaternion GetGlobalRotation() => Quaternion.Euler(GetGlobalEulerAngles());
        public Vector3 GetGlobalLossyScale() => _trackingTarget != null ? _trackingTarget.lossyScale : _data.GetGlobalScale()/* + _data._initializedExtents.scale + _data._offset.scale*/;


        public void SetComponent(Transform component) => SetComponent(component, _mask);
        public void SetComponent(Transform component, TransformAxes3_Flags mask)
        {
            if (mask.HasFlag(TransformAxes3_Flags.position)) component.position = GetGlobalPosition();
            if (mask.HasFlag(TransformAxes3_Flags.rotation)) component.rotation = GetGlobalRotation();
            if (mask.HasFlag(TransformAxes3_Flags.scale)) component.localScale = Transform_Presets.GetLocalScale(GetGlobalLossyScale(), component.parent);
        }

        public void SetData(Transform component) => SetData(component, _mask, _data._parent);

        public void SetData(Transform component, TransformAxes3_Flags mask, Transform parent)
        {
            if (parent != null)
            {
                if (mask.HasFlag(TransformAxes3_Flags.position)) _data.SetLocalPosition(component.localPosition, parent);
                if (mask.HasFlag(TransformAxes3_Flags.rotation)) _data.SetLocalRotation(component.localRotation, parent);
                if (mask.HasFlag(TransformAxes3_Flags.scale)) _data.SetLocalScale(component.localScale, parent);
            }
            else
            {
                if (_data._parent != null)
                {
                    if (mask.HasFlag(TransformAxes3_Flags.position)) _data.SetLocalPosition(component.localPosition, _data._parent);
                    if (mask.HasFlag(TransformAxes3_Flags.rotation)) _data.SetLocalRotation(component.localRotation, _data._parent);
                    if (mask.HasFlag(TransformAxes3_Flags.scale)) _data.SetLocalScale(component.localScale, _data._parent);
                }
                else
                {
                    if (mask.HasFlag(TransformAxes3_Flags.position)) _data.SetGlobalPosition(component.position);
                    if (mask.HasFlag(TransformAxes3_Flags.rotation)) _data.SetGlobalRotation(component.rotation);
                    if (mask.HasFlag(TransformAxes3_Flags.scale)) _data.SetGlobalScale(component.lossyScale);
                }
            }
        }

        public Coroutine_Behaviour_Data<Transform_MotionBehaviour_Container> SetTimeMotion(Transform component, float duration = 1.0f, Transform_Data currentData = null, System.Action<Coroutine_Behaviour_Data<Transform_MotionBehaviour_Container>> OnChange = null, System.Action<Coroutine_Behaviour_Data<Transform_MotionBehaviour_Container>> OnComplete = null)
        {
            Coroutine_Behaviour_Data<Transform_MotionBehaviour_Container> routineData = new();

            if (currentData == null)
            {
                currentData = new();
                currentData.SetData(component, _mask, _data._parent);
            }

            routineData._behaviour = new()
            {
                _component = component,
                _startData = currentData,
                _behaviours = new Transform_Data[] { this },
                _durationData = new() { _value = duration },
            };

            if (routineData._coroutine != null) Coroutine_Manager.Instance.StopCoroutine(routineData._coroutine);
            routineData._coroutine = Coroutine_Manager.Instance.TimeChange(t =>
            {
                if (_mask.HasFlag(TransformAxes3_Flags.position)) component.position = Vector3.Lerp(currentData.GetGlobalPosition(), GetGlobalPosition(), t / routineData._behaviour._durationData.position);
                if (_mask.HasFlag(TransformAxes3_Flags.rotation)) component.rotation = Quaternion.Slerp(currentData.GetGlobalRotation(), GetGlobalRotation(), t / routineData._behaviour._durationData.rotation);
                if (_mask.HasFlag(TransformAxes3_Flags.scale)) component.localScale = Transform_Presets.GetLocalScale(Vector3.Lerp(currentData.GetGlobalLossyScale(), GetGlobalLossyScale(), t / routineData._behaviour._durationData.scale), component.parent);

                OnChange?.Invoke(routineData);

            }, (t) => OnComplete?.Invoke(routineData), routineData._behaviour._durationData._value);

            return routineData;
        }

        public Coroutine_Behaviour_Data<Transform_MotionBehaviour_Container> SetSpeedMotion(Transform component, float speed = 1.0f, Transform_Data currentData = null, System.Action<Coroutine_Behaviour_Data<Transform_MotionBehaviour_Container>> OnChange = null, System.Action<Coroutine_Behaviour_Data<Transform_MotionBehaviour_Container>> OnComplete = null)
        {
            Coroutine_Behaviour_Data<Transform_MotionBehaviour_Container> routineData = new();

            if (currentData == null)
            {
                currentData = new();
                currentData.SetData(component, _mask, _data._parent);
            }

            Transform3ValueAxes_Data duration = new()
            {
                position = _mask.HasFlag(TransformAxes3_Flags.position) ? Vector3.Distance(currentData.GetGlobalPosition(), GetGlobalPosition()) / speed : default,
                rotation = _mask.HasFlag(TransformAxes3_Flags.rotation) ? Quaternion.Angle(currentData.GetGlobalRotation(), GetGlobalRotation()) / speed : default,
                scale = _mask.HasFlag(TransformAxes3_Flags.scale) ? Vector3.Distance(currentData.GetGlobalLossyScale(), GetGlobalLossyScale()) / speed : default,
            };

            routineData._behaviour = new()
            {
                _component = component,
                _startData = currentData,
                _behaviours = new Transform_Data[] { this },
                _durationData = new() { _value = Mathf.Max(duration.position, duration.rotation, duration.scale) },
            };

            if (routineData._coroutine != null) Coroutine_Manager.Instance.StopCoroutine(routineData._coroutine);
            routineData._coroutine = Coroutine_Manager.Instance.TimeChange(t =>
            {
                if (_mask.HasFlag(TransformAxes3_Flags.position)) component.position = Vector3.Lerp(currentData.GetGlobalPosition(), GetGlobalPosition(), t / routineData._behaviour._durationData.position);
                if (_mask.HasFlag(TransformAxes3_Flags.rotation)) component.rotation = Quaternion.Slerp(currentData.GetGlobalRotation(), GetGlobalRotation(), t / routineData._behaviour._durationData.rotation);
                if (_mask.HasFlag(TransformAxes3_Flags.scale)) component.localScale = Transform_Presets.GetLocalScale(Vector3.Lerp(currentData.GetGlobalLossyScale(), GetGlobalLossyScale(), t / routineData._behaviour._durationData.scale), component.parent);

                OnChange?.Invoke(routineData);

            }, (t) => OnComplete?.Invoke(routineData), routineData._behaviour._durationData._value);

            return routineData;
        }
    }

    [System.Serializable]
    public class TransformAxes_Parent_Mask_Data : TransformAxes_Parent_Data
    {
        //[Space(Value_Presets.m_8ByteInspectorSpace)]
        public TransformAxes3_Flags _mask;
    }

    [System.Serializable]
    public class TransformAxes_Data
    {
        [SerializeField] public Vector3 _position;
        [SerializeField] public Vector3 _rotation;
        [SerializeField] public Vector3 _scale;

        public Vector3 Position { get => _position; set => _position = value; }
        public Quaternion Rotation { get => Quaternion.Euler(_rotation); set => _rotation = value.eulerAngles; }
        public Vector3 EulerAngles { get => Rotation.eulerAngles; set => _rotation = Quaternion.Euler(value).eulerAngles; }
        public Vector3 Scale { get => _scale; set => _scale = value; }
    }

    [System.Serializable]
    public class Transform3ValueAxes_Mask_Data : Transform3ValueAxes_Data
    {
        //[Space(Value_Presets.m_8ByteInspectorSpace)]
        public TransformAxes3_Flags _mask;
    }

    [System.Serializable]
    public class TransformAxes_Parent_Data
    {
        public Transform _parent;
        public TransformAxes_Data _axes = new();

        public Vector3 GetGlobalPosition() => Transform_Presets.GetGlobalPosition(_axes.Position, _parent);
        public void SetGlobalPosition(Vector3 value) => SetLocalPosition(value, null);
        public void SetLocalPosition(Vector3 value, Transform parent)
        {
            _parent = parent;
            _axes.Position = value;
        }

        public Quaternion GetGlobalRotation() => Transform_Presets.GetGlobalRotation(_axes.Rotation, _parent);
        public void SetGlobalRotation(Quaternion value) => SetLocalRotation(value, null);
        public void SetLocalRotation(Quaternion value, Transform parent)
        {
            _parent = parent;
            _axes.Rotation = value;
        }

        public Vector3 GetGlobalEulerAngles() => Transform_Presets.GetGlobalEulerAngles(_axes.EulerAngles, _parent);
        public void SetGlobalEulerAngles(Vector3 value) => SetLocalEulerAngles(value, null);
        public void SetLocalEulerAngles(Vector3 value, Transform parent)
        {
            _parent = parent;
            _axes.EulerAngles = Quaternion.Euler(value).eulerAngles;
        }

        public Vector3 GetGlobalScale() => Transform_Presets.GetGlobalLossyScale(_axes.Scale, _parent);
        public void SetGlobalScale(Vector3 value) => SetLocalScale(value, null);
        public void SetLocalScale(Vector3 value, Transform parent)
        {
            _parent = parent;
            _axes.Scale = value;
        }
    }

    [System.Serializable]
    public class TransformAxes_Parent_SetFromSelf_Data : TransformAxes_Parent_Data
    {
        [Space(Value_Editor_Presets.m_8ByteInspectorSpace)]
        public TransformAxesSpace _autoSetMode = TransformAxesSpace.World;

        public void Initialize_AutoSetAxesFrom(Transform component)
        {
            if (_autoSetMode == TransformAxesSpace.World)
            {
                SetGlobalPosition(component.position);
                SetGlobalRotation(component.rotation);
                SetGlobalScale(component.lossyScale);
            }
            else if (_autoSetMode == TransformAxesSpace.Self)
            {
                _parent = component.parent;

                SetLocalPosition(component.localPosition, component.parent);
                SetLocalRotation(component.localRotation, component.parent);
                SetLocalScale(component.localScale, component.parent);
            }
        }
    }

    [System.Serializable]
    public class Transform3ValueAxes_Data
    {
        [SerializeField] float _position;
        [SerializeField] float _rotation;
        [SerializeField] float _scale;

        public float position { get => _position; set => _position = value; }
        public float rotation { get => _rotation; set => _rotation = value; }
        public float scale { get => _scale; set => _scale = value; }
    }

#endregion

}