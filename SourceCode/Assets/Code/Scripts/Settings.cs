using ArbazAminKhan.Frequentlys.Components;
using ArbazAminKhan.Inputs.Components;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;

namespace ArbazAminKhan.SettingsUtilities.Component
{
    public enum ScreenOrientation_State : byte { Portrait, Landscape }

    public partial class Settings : MonoBehaviour
    {
        [AutoStaticsCleanup] public static Settings Instance { get; private set; }

        #region LifeCycle

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        void OnEnable()
        {
            Frequently_Manager.OnPreUpdate += PreUpdateBehaviour;
        }

        void Start()
        {
            SetScreenSize_Start();
        }

        void OnDisable()
        {
            Frequently_Manager.OnPreUpdate -= PreUpdateBehaviour;
        }

        void OnDestroy()
        {
            Frequently_Manager.OnPreUpdate -= PreUpdateBehaviour;

            if (Instance == this) Instance = null;
        }

        #endregion

        void PreUpdateBehaviour()
        {
            CompareAndSetScreenSize();
        }

        #region Screen Size

        Vector2 _screenSize = Vector2.zero;
        [AutoStaticsCleanup] internal static System.Action<Vector2> ScreenSizeChanged;

        void SetScreenSize_Start()
        {
            SetScreenSize_WithoutNotify(GetScreenSize());
        }

        void CompareAndSetScreenSize()
        {
            var size = GetScreenSize();
            if (size != _screenSize)
            {
                SetScreenSize_WithoutNotify(size);
                ScreenSizeChanged?.Invoke(_screenSize);
            }
        }

        void SetScreenSize_WithoutNotify(Vector2 size)
        {
            _screenSize = size;
            CompareAndSetOrientation();
        }

        Vector2 GetScreenSize() => new(Screen.width, Screen.height);

        #endregion

        #region Screen Orientation

        ScreenOrientation_State _screenOrientation;
        [AutoStaticsCleanup] internal static System.Action<ScreenOrientation_State> ScreenOrientationChanged;

        void Orientation_State()
        {
            SetPanelSettingsMatch(GetOrientation());
        }

        void CompareAndSetOrientation()
        {
            var screenOrientation = GetOrientation();

            if (screenOrientation != _screenOrientation)
            {
                SetOrientation_WithoutNotify(screenOrientation);
                ScreenOrientationChanged?.Invoke(_screenOrientation);
            }
        }

        void SetOrientation_WithoutNotify(ScreenOrientation_State state)
        {
            _screenOrientation = state;
            SetPanelSettingsMatch();
        }

        public void SetPanelSettingsMatch() => SetPanelSettingsMatch(_screenOrientation);

        public void SetPanelSettingsMatch(ScreenOrientation_State screenOrientation)
        {
            var panelRenderers = UIToolkit_Inputs.GetPanelRenderer();
            foreach (var panelRenderer in panelRenderers) panelRenderer.panelSettings.match = .5f + (screenOrientation == ScreenOrientation_State.Portrait ? .0625f : -.0625f);
        }

        public ScreenOrientation_State GetOrientation() => _screenSize.x > _screenSize.y ? ScreenOrientation_State.Landscape : ScreenOrientation_State.Portrait;

        #endregion
    }
}