using UnityEngine;

namespace ArbazAminKhan.Transforms.Components
{
    public class Transform_Grabber_Container : MonoBehaviour, IGrabber
    {
        [SerializeField] Transform_Grabber _componenet;

        public Transform_Grabber GetGrabber() => _componenet;
    }
}
