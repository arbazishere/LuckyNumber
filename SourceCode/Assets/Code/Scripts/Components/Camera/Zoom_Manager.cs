using ArbazAminKhan.Cameras.Components;
using ArbazAminKhan.Datas;
using ArbazAminKhan.Inputs.Components;
using UnityEngine;
using UnityEngine.InputSystem;

public class Zoom_Manager : MonoBehaviour
{
    public float SpeedMultiplier = 1.0f;
    public sbyte PinchFixedBalancedMultiplier = 100;

    public Float_MinMax_Data _fieldOfViewClamp = new() { _min = 8, _max = 96 };

    public void UpdateBehaviour()
    {
        if (GetDelta_ScrollOrPinch(out float delta, out Vector2 center)) PassValue(delta, center);
    }

    public void PassValue(float delta, Vector2 center)
    {
        if (Mathf.Approximately(delta, 0f))
            return;

        delta *= SpeedMultiplier;

        float oldValue = Camera_Manager.Instance.FieldOfView;

        float newValue = oldValue - delta;

        if (Mathf.Approximately(oldValue, newValue))
            return;

        Camera_Manager.Instance.FieldOfView = Mathf.Clamp(newValue, _fieldOfViewClamp._min, _fieldOfViewClamp._max);
    }

    public bool GetDelta_ScrollOrPinch(out float delta, out Vector2 center)
    {
        delta = default;
        center = default;

        if (Touch_Manager._noneUI_TouchsDown_Data.Length > 1)
        {
            var touch1 = Touch_Manager._noneUI_TouchsDown_Data[0];
            var touch2 = Touch_Manager._noneUI_TouchsDown_Data[1];

            if (touch1 != null && touch2 != null)
            {
                float screenScale = Mathf.Min(Screen.width, Screen.height);

                Vector2 touchPos1 = touch1.position;
                Vector2 touchPos2 = touch2.position;

                Vector2 curPos1 = touchPos1 / screenScale;
                Vector2 curPos2 = touchPos2 / screenScale;

                Vector2 prePos1 = (touchPos1 - touch1.delta) / screenScale;
                Vector2 prePos2 = (touchPos2 - touch2.delta) / screenScale;

                float previousDistance = Vector2.Distance(prePos1, prePos2);
                float currentDistance = Vector2.Distance(curPos1, curPos2);

                delta = (currentDistance - previousDistance) * PinchFixedBalancedMultiplier;
                center = Vector2.Lerp(touch1.position, touch2.position, 0.5f);

                return !Mathf.Approximately(delta, 0f);
            }
        }

        if (Mouse.current != null)
        {
            center = Mouse.current.position.value;

            delta = Mathf.Clamp(
                Mouse.current.scroll.value.y,
                -1f,
                1f
            );

            return !Mathf.Approximately(delta, 0f);
        }

        return false;
    }
}
