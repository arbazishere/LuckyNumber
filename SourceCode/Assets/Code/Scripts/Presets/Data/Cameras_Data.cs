using ArbazAminKhan.Editor.Presets;
using ArbazAminKhan.Enums.Datas;
using ArbazAminKhan.Transforms.Datas;
using UnityEngine;

namespace ArbazAminKhan.Cameras.Datas
{
    public abstract class Cameras_Data { }

    [System.Serializable]
    public class Camera_Data
    {
        public Object_Camera_Data _object;
        public Lens_Camera_Data _lens;
        public ThirdPerson_Camera_Data _thirdPerson;
    }

    [System.Serializable]
    public class Lens_Camera_Data
    {
        public float _fieldOfView = 60;

        [Space(Value_Editor_Presets.m_8ByteInspectorSpace)]
        public Lens_Camera_Data_Flags _mask;
    }

    [System.Serializable]
    public class ThirdPerson_Camera_Data
    {
        public float _cameraDistance;

        [Space(Value_Editor_Presets.m_8ByteInspectorSpace)]
        public ThirdPerson_Camera_Data_Flags _mask;
    }

    [System.Serializable]
    public class Object_Camera_Data : Transform_Data { }
}