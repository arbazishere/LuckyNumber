using ArbazAminKhan.Cameras.Components;
using UnityEngine;

public class Player : MonoBehaviour
{
    string _objectKey = "Object";

    Torch _torch;

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("other.name: " + other.name);
        Debug.Log("other.gameObject.tag == _objectKey: " + (other.gameObject.tag == _objectKey));
        Debug.Log("Input_Manager.Instance._interactionAction.WasPressedThisFrame(): " + (Input_Manager.Instance._interactionAction.WasPressedThisFrame()));

        if(other.gameObject.tag == _objectKey && Input_Manager.Instance._interactionAction.WasPressedThisFrame() && other.TryGetComponent(out _torch))
        {
            _torch._canvas.gameObject.SetActive(true);

            _torch.transform.parent = Camera_Manager.Instance._root;
            _torch.transform.SetLocalPositionAndRotation(new(.0625f, -.125f, .25f), default);

            other.enabled = false;
        }
    }
}
