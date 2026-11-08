using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Unity.Scripting.LifecycleManagement;
using ArbazAminKhan.SettingsUtilities.Component;

namespace ArbazAminKhan.Inputs.Components
{
    public static partial class UIToolkit_Inputs
    {
        [AutoStaticsCleanup] static readonly List<VisualElement> _roots = new();
        [AutoStaticsCleanup] static readonly List<PanelRenderer> _panelRenderers = new();

        public static void RegisterRoot(VisualElement root)
        {
            if (root == null || _roots.Contains(root)) return;

            _roots.Add(root);
        }

        public static void UnregisterRoot(VisualElement root)
        {
            if (root == null) return;

            _roots.Remove(root);
        }

        public static void RegisterPanelRenderer(PanelRenderer panelRenderer)
        {
            if (panelRenderer == null || _panelRenderers.Contains(panelRenderer)) return;

            _panelRenderers.Add(panelRenderer);

            if(Settings.Instance != null) Settings.Instance.SetPanelSettingsMatch();
        }

        public static void UnregisterPanelRenderer(PanelRenderer panelRenderer)
        {
            if (panelRenderer == null) return;

            _panelRenderers.Remove(panelRenderer);
        }

        public static List<PanelRenderer> GetPanelRenderer() => _panelRenderers;

        public static bool IsPointerOverElement() => HasUIElement(GetPointerPosition());

        public static bool HasUIElement(Vector2 screenPosition) => GetUIElement(screenPosition) != null;

        public static VisualElement GetUIElement(Vector2 screenPosition)
        {
            // Convert from Unity/Input bottom-left screen coordinates
            // to UI Toolkit top-left screen coordinates.
            screenPosition.y = Screen.height - screenPosition.y;

            for (int i = 0; i < _roots.Count; i++)
            {
                var root = _roots[i];

                if (root == null || root.panel == null) continue;

                var panelPosition = RuntimePanelUtils.ScreenToPanel(root.panel, screenPosition);

                var pickedElement = root.panel.Pick(panelPosition);

                // panel.Pick already ignores PickingMode.Ignore elements.
                // It returns the top element at that position, or null.
                if (pickedElement != null && pickedElement != root) return pickedElement;
            }

            return null;
        }

        public static List<VisualElement> GetUIElements(Vector2 screenPosition)
        {
            // Convert from Unity/Input bottom-left screen coordinates
            // to UI Toolkit top-left screen coordinates.
            screenPosition.y = Screen.height - screenPosition.y;

            List<VisualElement> elements = new();

            for (int i = 0; i < _roots.Count; i++)
            {
                var root = _roots[i];

                if (root == null || root.panel == null) continue;

                var panelPosition = RuntimePanelUtils.ScreenToPanel(root.panel, screenPosition);

                root.panel.PickAll(panelPosition, elements);
            }

            return elements;
        }

        static Vector2 GetPointerPosition()
            => Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed
            ? Touchscreen.current.primaryTouch.position.value : (Mouse.current != null ? Mouse.current.position.value : default);
    }
}
