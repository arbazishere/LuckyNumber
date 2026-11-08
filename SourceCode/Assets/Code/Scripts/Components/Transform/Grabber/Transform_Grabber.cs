using ArbazAminKhan.Datas;
using System.Linq;
using UnityEngine;

namespace ArbazAminKhan.Transforms.Components
{
    interface IGrabber { public Transform_Grabber GetGrabber(); }

    public class Transform_Grabber : MonoBehaviour, IGrabber
    {
        [SerializeField]
        internal Vector3MinMax_Data _positionClamp = new()
        {
            _min = Vector3.one * sbyte.MinValue,
            _max = Vector3.one * sbyte.MaxValue,
        };

        public System.Action<Grabber_Transform_Data> OnDown, OnMove, OnUp;

        internal Rigidbody _rigidbody;
        bool _isGrabbed = false;
        public bool IsGrabbed => _isGrabbed;
        internal Transform Main => this.transform;

        bool _isInitialize_Awake = false;

        bool _interactable = true;
        internal bool Interactable 
        { 
            get => _interactable; 
            set
            {
                _interactable = value;
            } 
        }

        void Awake()
        {
            Awake_Initialize();
        }

        public void Awake_Initialize()
        {
            if (_isInitialize_Awake) return;
            _isInitialize_Awake = true;

            TryGetComponent(out _rigidbody);
        }

        public void DownBehaviour(Grabber_Transform_Data data)
        {
            _isGrabbed = true;

            if(_rigidbody != null) _rigidbody.isKinematic = true;
            data._component.OnDown?.Invoke(data);
        }

        public void MoveBehaviour(Grabber_Transform_Data data)
        {
            data._component.OnMove?.Invoke(data);

            var pos = transform.localPosition;
            pos.x = Mathf.Clamp(pos.x, _positionClamp._min.x, _positionClamp._max.x);
            pos.y = Mathf.Clamp(pos.y, _positionClamp._min.y, _positionClamp._max.y);
            pos.z = Mathf.Clamp(pos.z, _positionClamp._min.z, _positionClamp._max.z);

            transform.localPosition = pos;
        }

        public void UpBehaviour(Grabber_Transform_Data data)
        {
            _isGrabbed = false;

            if (_rigidbody != null) data._component._rigidbody.isKinematic = false;

            data._component.OnUp?.Invoke(data);
        }

        public Transform_Grabber GetGrabber() => this;

        public Collider[] GetCollider()
        {
            var colliders = transform.GetComponents<Collider>().ToList();

            var comps = transform.GetComponentsInChildren<IGrabber>();
            foreach (var comp in comps)
            {
                if (comp.GetGrabber() != this) continue;

                var list = comp.GetGrabber().transform.GetComponents<Collider>();
                colliders.AddRange(list);
            }

            return colliders.ToArray();
        }
    }
}