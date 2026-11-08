using System.Linq;
using UnityEngine;

namespace ArbazAminKhan.Transforms.Components
{
    public class Transform_Data_Component_Container : MonoBehaviour
    {
        [SerializeField] internal Transform_Data_Component[] _components = new Transform_Data_Component[default];

        public Transform_Data_Component_Data[] GetAllBehaviour() => _components.SelectMany(comp => comp._behaviours).ToArray();
    }
}