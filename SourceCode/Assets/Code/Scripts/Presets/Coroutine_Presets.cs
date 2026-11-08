using System;
using System.Collections;
using UnityEngine;

namespace ArbazAminKhan.Presets
{
    public static class Coroutine_Presets
    {
        public static IEnumerator Delay_Routine(float delay, Action onComplete)
        {
            yield return new WaitForSeconds(delay);
            onComplete?.Invoke();
        }

        public static IEnumerator ValueChange_Routine(float startValue, float targetValue, float speed, Action<float> onChange = null, Action<float> onComplete = null)
        {
            while (startValue != targetValue)
            {
                startValue = Mathf.MoveTowards(startValue, targetValue, speed * Time.deltaTime);
                onChange?.Invoke(startValue);

                yield return null;
            }

            onComplete?.Invoke(startValue);
        }

        public static IEnumerator RunWhen_Routine(Func<bool> condition, Action callback)
        {
            yield return new WaitUntil(condition);

            callback?.Invoke();
        }
    }

}