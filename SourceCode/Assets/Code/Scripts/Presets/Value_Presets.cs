using UnityEngine;

namespace ArbazAminKhan.Presets
{
    public class Value_Presets
    {
        public const short m_256Value = 256;
        public static float GetWaveYValue(float time, float amplitude = 1f, float frequency = 1f) => Mathf.Sin(time * frequency) * amplitude;
    }
}

namespace ArbazAminKhan.Editor.Presets
{
    public class Value_Editor_Presets
    {
        public const byte m_8ByteInspectorSpace = 8;
    }
}
 
namespace ArbazAminKhan.Datas
{
    [System.Serializable]
    public class Float_MinMax_Data
    {
        public float _min = 0, _max = 1;
    }

    [System.Serializable]
    public class Vector3MinMax_Data
    {
        public Vector3 _min = Vector3.zero;
        public Vector3 _max = Vector3.one;
    }
}