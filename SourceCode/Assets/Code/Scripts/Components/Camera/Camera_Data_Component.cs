using ArbazAminKhan.Cameras.Datas;
using UnityEngine;

namespace ArbazAminKhan.Cameras.Components
{
    public class Camera_Data_Component : MonoBehaviour
    {
        [SerializeField] Camera_Data_Component_Data[] _behaviours = new Camera_Data_Component_Data[default];
    }

    [System.Serializable]
    public class Camera_Data_Component_Data : Camera_Data { }
}