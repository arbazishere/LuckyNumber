using ArbazAminKhan.Editor.Presets;
using ArbazAminKhan.Enums.Datas;
using ArbazAminKhan.Frequentlys.Components;
using UnityEngine;

namespace ArbazAminKhan.Colors.Components
{
    public class Componenet_Color_Wave_Behaviour : MonoBehaviour
    {
        [SerializeField] Color_Wave_Behaviour_Data[] _behaviours = new Color_Wave_Behaviour_Data[default];

        [SerializeField] float _time = .0f;

        [Space(Value_Editor_Presets.m_8ByteInspectorSpace)]
        public bool _setOnUpdate = true;

        void OnEnable()
        {
            Frequently_Manager.OnUpdate -= Update_Behaviour;
            if (_setOnUpdate) Frequently_Manager.OnUpdate += Update_Behaviour;
        }

        void Update_Behaviour()
        {
            _time += Time.deltaTime;

            foreach (var behaviour in _behaviours) behaviour.SetColor(_time);
        }

        void OnDisable()
        {
            Frequently_Manager.OnUpdate -= Update_Behaviour;
        }

        void OnDestroy()
        {
            Frequently_Manager.OnUpdate -= Update_Behaviour;
        }
    }

    [System.Serializable]
    public class Color_Wave_Behaviour_Data
    {
        [SerializeField] Component[] _components;
        [SerializeField] Color _baseValue;

        [Space(Value_Editor_Presets.m_8ByteInspectorSpace)]
        [SerializeField] Vector4 _amplitude = Vector3.one;
        [SerializeField] Vector4 _frequency = Vector3.one;

        [Space(Value_Editor_Presets.m_8ByteInspectorSpace)]
        [SerializeField] Axis4_Flags _mask;

        bool _isInitialized;

        MeshRenderer[] _meshRenderers;

        Color _color;

        public void SetColor(float time)
        {
            Intialized();

            if (_mask.HasFlag(Axis4_Flags.x))
            {
                _color.r = _baseValue.r + Mathf.Sin(time * _frequency.x) * _amplitude.x;
            }

            if (_mask.HasFlag(Axis4_Flags.y))
            {
                _color.g = _baseValue.g + Mathf.Sin(time * _frequency.y) * _amplitude.y;
            }

            if (_mask.HasFlag(Axis4_Flags.z))
            {
                _color.b = _baseValue.b + Mathf.Sin(time * _frequency.z) * _amplitude.z;
            }

            if (_mask.HasFlag(Axis4_Flags.w))
            {
                _color.a = _baseValue.a + Mathf.Sin(time * _frequency.w) * _amplitude.w;
            }

            SetColor(_color);
        }

        void Intialized()
        {
            if (_isInitialized) return;

            System.Collections.Generic.List<MeshRenderer> meshRenderers = new();

            foreach (var componenet in _components)
            {
                if (componenet.TryGetComponent(out MeshRenderer meshRenderer)) meshRenderers.Add(meshRenderer);
            }

            _meshRenderers = meshRenderers.ToArray();

            _isInitialized = true;
        }

        public void SetColor(Color color)
        {
            Intialized();

            foreach (var comp in _meshRenderers) comp.material.color = ModifyAndGetColor(comp.material.color, color);
        }

        Color ModifyAndGetColor(Color CompColor, Color target)
        {
            if (_mask.HasFlag(Axis4_Flags.x)) CompColor.r = target.r;
            if (_mask.HasFlag(Axis4_Flags.y)) CompColor.g = target.g;
            if (_mask.HasFlag(Axis4_Flags.z)) CompColor.b = target.b;
            if (_mask.HasFlag(Axis4_Flags.w)) CompColor.a = target.a;

            return CompColor;
        }
    }
}
