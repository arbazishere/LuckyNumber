using UnityEngine;

namespace ArbazAminKhan.Inputs.Components
{
    public class Component_Clicker : MonoBehaviour
    {
        [SerializeField] bool _interactable = true;
        public System.Action clicked;

        void OnMouseUpAsButton()
        {
            Debug.Log("Call");
            if (!_interactable) return;
            if (UIToolkit_Inputs.IsPointerOverElement()) return;

            clicked?.Invoke();
        }

        public bool Interactable
        {
            get => _interactable;
            set => _interactable = value;
        }
    }
}
