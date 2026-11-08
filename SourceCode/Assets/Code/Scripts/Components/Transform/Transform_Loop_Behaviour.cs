using ArbazAminKhan.Editor.Presets;
using ArbazAminKhan.Enums.Datas;
using ArbazAminKhan.Frequentlys.Components;
using ArbazAminKhan.Transforms.Datas;
using ArbazAminKhan.Transforms.Presets;
using UnityEngine;

namespace ArbazAminKhan.Transforms.Components
{
    public class Transform_Loop_Behaviour : MonoBehaviour
    {
        [SerializeField] Transform[] _components = new Transform[default];

        [Space(Value_Editor_Presets.m_8ByteInspectorSpace)]
        [InspectorName("Axes")] public TransformAxes_Parent_Mask_Data _axesData;

        [Space(Value_Editor_Presets.m_8ByteInspectorSpace)]
        public float _speed = 1.0f;
        public Transform3ValueAxes_Mask_Data _speedOverride;

        [Space(Value_Editor_Presets.m_8ByteInspectorSpace)]
        public bool _setOnUpdate = true;

        void OnEnable()
        {
            Frequently_Manager.OnUpdate -= Update_Behaviour;
            if (_setOnUpdate) Frequently_Manager.OnUpdate += Update_Behaviour;
        }

        void OnDisable()
        {
            Frequently_Manager.OnUpdate -= Update_Behaviour;
        }

        void OnDestroy()
        {
            Frequently_Manager.OnUpdate -= Update_Behaviour;
        }

        void Update_Behaviour()
        {
            bool hasPos = _axesData._mask.HasFlag(TransformAxes3_Flags.position);
            bool hasRot = _axesData._mask.HasFlag(TransformAxes3_Flags.rotation);
            bool hasScl = _axesData._mask.HasFlag(TransformAxes3_Flags.scale);
            Vector3 dir;

            foreach (var component in _components)
            {
                if (hasPos)
                {
                    dir = _axesData._parent != null ? _axesData._parent.TransformDirection(_axesData._axes._position) : _axesData._axes._position;
                    component.Translate(GetSpeed() * Time.deltaTime * dir, Space.World);
                }
                if (hasRot)
                {
                    dir = _axesData._parent != null ? _axesData._parent.TransformDirection(_axesData._axes._rotation) : _axesData._axes._rotation;
                    component.Rotate(dir, GetSpeed() * Time.deltaTime, Space.World);
                }
                if (hasScl)
                {
                    dir = _axesData._parent != null ? _axesData._parent.TransformDirection(_axesData._axes._scale) : _axesData._axes._scale;
                    var targetLossyScale = component.lossyScale + (GetSpeed() * Time.deltaTime * dir);
                    component.localScale = Transform_Presets.GetLocalScale(targetLossyScale, component.parent);
                }
            }
        }

        public float GetSpeed() => _speedOverride._mask switch
        {
            TransformAxes3_Flags.position => _speedOverride.position,
            TransformAxes3_Flags.rotation => _speedOverride.rotation,
            TransformAxes3_Flags.scale => _speedOverride.scale,
            _ => _speed
        };
    }

}