using Unity.Scripting.LifecycleManagement;
using UnityEngine;

public partial class Audio_Manager : MonoBehaviour
{
    [AutoStaticsCleanup] public static Audio_Manager Instance { get; private set; }

    [SerializeField] internal AudioSource _click;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
