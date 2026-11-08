using ArbazAminKhan.Presets;
using System;
using UnityEngine;
using Unity.Scripting.LifecycleManagement;
using ArbazAminKhan.Coroutines.Components;

namespace ArbazAminKhan.Coroutines.Components
{
    public partial class Coroutine_Manager : MonoBehaviour
    {
        [AutoStaticsCleanup] public static Coroutine_Manager Instance { get; private set; }

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

        public Coroutine Delay(float delay, Action onComplete)
        {
            return StartCoroutine(Coroutine_Presets.Delay_Routine(delay, onComplete));
        }

        public Coroutine TimeChange(Action<float> onChange, Action<float> onComplete = null, float targetTime = short.MaxValue, float startTime = .0f)
        {
            return StartCoroutine(Coroutine_Presets.ValueChange_Routine(startTime, targetTime, 1, onChange, onComplete));
        }

        public Coroutine RunWhen(Func<bool> condition, Action callback)
        {
            return StartCoroutine(Coroutine_Presets.RunWhen_Routine(condition, callback));
        }

        public void StopRoutine(Coroutine coroutine)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }
    }
}

namespace ArbazAminKhan.Coroutines.Datas
{
    public class Coroutine_Behaviour_Data<T>
    {
        public Coroutine _coroutine;
        public T _behaviour;

        public void StopRoutine()
        {
            if (_coroutine != null)
            {
                Coroutine_Manager.Instance.StopCoroutine(_coroutine);
                _coroutine = null;
            }
        }
    }
}
