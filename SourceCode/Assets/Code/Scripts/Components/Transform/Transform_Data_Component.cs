using ArbazAminKhan.Transforms.Datas;
using UnityEngine;

namespace ArbazAminKhan.Transforms.Components
{
    public class Transform_Data_Component : MonoBehaviour
    {
        [SerializeField] internal Transform_Data_Component_Data[] _behaviours = new Transform_Data_Component_Data[default];

        void Awake()
        {
            foreach (var behaviour in _behaviours) behaviour.Initialize();
        }
    }

    [System.Serializable]
    public class Transform_Data_Component_Data : Transform_Data { }
}