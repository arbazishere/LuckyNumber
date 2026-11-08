using UnityEngine;
using Unity.Scripting.LifecycleManagement;

namespace ArbazAminKhan.Frequentlys.Components
{
    public partial class Frequently_Manager : MonoBehaviour
    {
        [AutoStaticsCleanup] public static System.Action OnPreUpdate, OnUpdate;

        void Update()
        {
            OnPreUpdate?.Invoke();
            OnUpdate?.Invoke();
        }
    }
}
