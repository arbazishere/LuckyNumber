using ArbazAminKhan.Editor.Presets;
using ArbazAminKhan.Enums.Datas;
using ArbazAminKhan.Frequentlys.Components;
using UnityEngine;

namespace ArbazAminKhan.Colors.Components
{
    public class Color_Wave_Behaviour_Base : MonoBehaviour
    {
        [SerializeField] protected float _time = .0f;

        [Space(Value_Editor_Presets.m_8ByteInspectorSpace)]
        public bool _setOnUpdate = true;

        protected virtual void OnEnable()
        {
            Frequently_Manager.OnUpdate -= Update_Behaviour;
            if (_setOnUpdate) Frequently_Manager.OnUpdate += Update_Behaviour;
        }

        protected virtual void Update_Behaviour()
        {
            SetTime(_time + Time.deltaTime);
        }

        public virtual void SetTime(float time)
        {
            _time = time;
        }

        protected virtual void OnDisable()
        {
            Frequently_Manager.OnUpdate -= Update_Behaviour;
        }

        protected virtual void OnDestroy()
        {
            Frequently_Manager.OnUpdate -= Update_Behaviour;
        }
    }

    [System.Serializable]
    public class Color_Wave_Behaviour_Base_Data
    {
        public Color _baseValue;

        [Space(Value_Editor_Presets.m_8ByteInspectorSpace)]
        public Vector4 _amplitude = Vector3.one;
        public Vector4 _frequency = Vector3.one;

        [Space(Value_Editor_Presets.m_8ByteInspectorSpace)]
        public Axis4_Flags _mask;
    }
}