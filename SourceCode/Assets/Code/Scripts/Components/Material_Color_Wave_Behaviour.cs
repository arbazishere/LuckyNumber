using ArbazAminKhan.Enums.Datas;
using UnityEngine;

namespace ArbazAminKhan.Colors.Components
{
    public class Material_Color_Wave_Behaviour : Color_Wave_Behaviour_Base
    {
        [SerializeField] Material_Color_Wave_Behaviour_Data[] _behaviours = new Material_Color_Wave_Behaviour_Data[default];

        public override void SetTime(float time)
        {
            base.SetTime(time);
            foreach (var behaviour in _behaviours) behaviour.SetColor(_time);
        }
    }

    [System.Serializable]
    public class Material_Color_Wave_Behaviour_Data
    {
        public Material[] _componenets;
        public Color_Wave_Behaviour_Base_Data _behaviour;

        Color _color;

        public void SetColor(float time)
        {
            if (_behaviour._mask.HasFlag(Axis4_Flags.x)) _color.r = _behaviour._baseValue.r + Mathf.Sin(time * _behaviour._frequency.x) * _behaviour._amplitude.x;
            if (_behaviour._mask.HasFlag(Axis4_Flags.y)) _color.g = _behaviour._baseValue.g + Mathf.Sin(time * _behaviour._frequency.y) * _behaviour._amplitude.y;
            if (_behaviour._mask.HasFlag(Axis4_Flags.z)) _color.b = _behaviour._baseValue.b + Mathf.Sin(time * _behaviour._frequency.z) * _behaviour._amplitude.z;
            if (_behaviour._mask.HasFlag(Axis4_Flags.w)) _color.a = _behaviour._baseValue.a + Mathf.Sin(time * _behaviour._frequency.w) * _behaviour._amplitude.w;

            foreach (var comp in _componenets)
            {
                if (!_behaviour._mask.HasFlag(Axis4_Flags.x)) _color.r = comp.color.r;
                if (!_behaviour._mask.HasFlag(Axis4_Flags.y)) _color.g = comp.color.g;
                if (!_behaviour._mask.HasFlag(Axis4_Flags.z)) _color.b = comp.color.b;
                if (!_behaviour._mask.HasFlag(Axis4_Flags.w)) _color.a = comp.color.a;

                comp.color = _color;
            }
        }
    }
}