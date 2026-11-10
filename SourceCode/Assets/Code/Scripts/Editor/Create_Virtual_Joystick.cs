#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

// GameObject > UI > Virtual Joystick
// Builds a joystick that uses Unity's On-Screen Stick, bound to <Gamepad>/leftStick,
// which is what the "Move" action in InputSystem_Actions already listens to.
public static class Create_Virtual_Joystick
{
    [MenuItem("GameObject/UI/Virtual Joystick", false, 2100)]
    static void Create(MenuCommand command)
    {
        var parent = FindCanvas(command.context as GameObject);
        var knob = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");

        var baseGO = new GameObject("Virtual Joystick", typeof(RectTransform), typeof(Image));
        Undo.RegisterCreatedObjectUndo(baseGO, "Create Virtual Joystick");
        baseGO.transform.SetParent(parent, false);

        var baseRT = (RectTransform)baseGO.transform;
        baseRT.anchorMin = baseRT.anchorMax = Vector2.zero;
        baseRT.pivot = new Vector2(.5f, .5f);
        baseRT.sizeDelta = new Vector2(260, 260);
        baseRT.anchoredPosition = new Vector2(250, 220);

        var baseImg = baseGO.GetComponent<Image>();
        baseImg.sprite = knob;
        baseImg.color = new Color(1, 1, 1, .25f);
        baseImg.raycastTarget = false;

        var handleGO = new GameObject("Handle", typeof(RectTransform), typeof(Image), typeof(OnScreenStick));
        handleGO.transform.SetParent(baseGO.transform, false);

        var handleRT = (RectTransform)handleGO.transform;
        handleRT.anchorMin = handleRT.anchorMax = new Vector2(.5f, .5f);
        handleRT.sizeDelta = new Vector2(110, 110);
        handleRT.anchoredPosition = Vector2.zero;

        var handleImg = handleGO.GetComponent<Image>();
        handleImg.sprite = knob;
        handleImg.color = new Color(1, 1, 1, .6f);
        handleImg.raycastTarget = true;

        var stick = handleGO.GetComponent<OnScreenStick>();
        stick.controlPath = "<Gamepad>/leftStick";
        stick.movementRange = 75;
        EditorUtility.SetDirty(stick);

        Selection.activeGameObject = handleGO;
    }

    static Transform FindCanvas(GameObject context)
    {
        Canvas canvas = context != null ? context.GetComponentInParent<Canvas>() : null;
        if (canvas == null) canvas = Object.FindFirstObjectByType<Canvas>();

        if (canvas == null)
        {
            var go = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Undo.RegisterCreatedObjectUndo(go, "Create Canvas");
            canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = .5f;
        }

        if (Object.FindFirstObjectByType<EventSystem>() == null)
        {
            var es = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            Undo.RegisterCreatedObjectUndo(es, "Create EventSystem");
        }

        return canvas.transform;
    }
}
#endif
