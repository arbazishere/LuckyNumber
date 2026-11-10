using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem;
using UnityEngine;
using ArbazAminKhan.Inputs.Presets;
using UnityEngine.UIElements;
using ArbazAminKhan.Inputs.Datas;

namespace ArbazAminKhan.Inputs.Presets
{
    public abstract class Touch_Presets
    {
        public static Vector2 Position(Touch_Data touchData) => touchData._touchControl != null ? touchData._touchControl.position.value : Mouse.current != null ? Mouse.current.position.value : default;
        public static Vector2 Delta(Touch_Data touchData) => touchData._touchControl != null ? touchData._touchControl.delta.value : Mouse.current != null ? Mouse.current.delta.value : default;

        public static Vector2 mousePosition => Mouse.current != null ? Mouse.current.position.value : default;
        public static Vector2 mouseDelta => Mouse.current != null ? Mouse.current.delta.value : default;
    }
}

namespace ArbazAminKhan.Inputs.Datas
{
    [System.Serializable]
    public class Touch_Data
    {
        internal TouchControl _touchControl = null;
        internal RayData_Touch_Data _hitData = new();

        internal Vector2 position => Touch_Presets.Position(this);
        internal Vector2 delta => Touch_Presets.Delta(this);
        internal Vector2 delta_ScreenModified => delta / Mathf.Min(Screen.width, Screen.height);

        internal bool IsDown => _touchControl != null && _touchControl.device.added ? (_touchControl.press.isPressed || _touchControl.press.wasPressedThisFrame) : Mouse.current != null && (Mouse.current.leftButton.isPressed || Mouse.current.leftButton.wasPressedThisFrame);
    }

    [System.Serializable]
    public class RayData_Touch_Data
    {
        internal bool _hasRaycastData;
        internal RaycastHit _data;

        internal System.Collections.Generic.List<VisualElement> _visualElements = new();
    }
}