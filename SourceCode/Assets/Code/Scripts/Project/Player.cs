using ArbazAminKhan.Cameras.Components;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    string _objectKey = "Object";

    [SerializeField] Torch _torch;

    private void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame && _torch._interactionButton.gameObject.activeSelf)
        {
            _torch.GrabTorch();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == _objectKey && other.TryGetComponent(out _torch))
        {
            _torch._interactionButton.gameObject.SetActive(true);
            _torch._hardCollider.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == _objectKey && other.TryGetComponent(out _torch))
        {
            _torch._interactionButton.gameObject.SetActive(false);
            _torch._hardCollider.enabled = false;
        }
    }
}
