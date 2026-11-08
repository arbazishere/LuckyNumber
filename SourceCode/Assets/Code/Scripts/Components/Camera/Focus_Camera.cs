using System.Linq;
using UnityEngine;

namespace ArbazAminKhan.Cameras.Components
{
    public class Focus_Camera : MonoBehaviour
    {
        [SerializeField] float _padding = 1.05f; // 5% padding (better than offset)
        [SerializeField] LayerMask _mask;

        public void FitAllRenderers()
        {
            var comps = FindObjectsByType<Renderer>().Where(r => (_mask.value & (1 << r.gameObject.layer)) != 0).ToArray();

            var cameraController = Camera_Manager.Instance;
            if (comps.Length == 0 || cameraController == null) return;

            // Convert all bounds into camera space
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            foreach (var r in comps)
            {
                Bounds b = r.bounds;

                // 8 corners of bounds
                Vector3[] corners = new Vector3[8]
                {
                new(b.min.x, b.min.y, b.min.z),
                new(b.min.x, b.min.y, b.max.z),
                new(b.min.x, b.max.y, b.min.z),
                new(b.min.x, b.max.y, b.max.z),
                new(b.max.x, b.min.y, b.min.z),
                new(b.max.x, b.min.y, b.max.z),
                new(b.max.x, b.max.y, b.min.z),
                new(b.max.x, b.max.y, b.max.z)
                };

                foreach (var corner in corners)
                {
                    Vector3 local = cameraController._root.InverseTransformPoint(corner);

                    min = Vector3.Min(min, local);
                    max = Vector3.Max(max, local);
                }
            }

            Vector3 size = max - min;
            Vector3 centerLocal = (min + max) * 0.5f;

            float fov = cameraController.FieldOfView * Mathf.Deg2Rad;
            float aspect = cameraController._camera.aspect;

            float distanceY = (size.y * 0.5f) / Mathf.Tan(fov / 2f);
            float distanceX = (size.x * 0.5f) / (Mathf.Tan(fov / 2f) * aspect);

            float distance = Mathf.Max(distanceX, distanceY) * _padding;

            // Move root to correct world position
            Vector3 centerWorld = cameraController._root.TransformPoint(centerLocal);
            cameraController._root.position = centerWorld;

            cameraController.CameraDistance = distance;
        }

        public float GetCameraDistance()
        {
            var comps = FindObjectsByType<Renderer>().Where(r => (_mask.value & (1 << r.gameObject.layer)) != 0).ToArray();

            var cameraController = Camera_Manager.Instance;
            if (comps.Length == 0 || cameraController == null) return cameraController.CameraDistance;

            // Convert all bounds into camera space
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            foreach (var r in comps)
            {
                Bounds b = r.bounds;

                // 8 corners of bounds
                Vector3[] corners = new Vector3[8]
                {
                new(b.min.x, b.min.y, b.min.z),
                new(b.min.x, b.min.y, b.max.z),
                new(b.min.x, b.max.y, b.min.z),
                new(b.min.x, b.max.y, b.max.z),
                new(b.max.x, b.min.y, b.min.z),
                new(b.max.x, b.min.y, b.max.z),
                new(b.max.x, b.max.y, b.min.z),
                new(b.max.x, b.max.y, b.max.z)
                };

                foreach (var corner in corners)
                {
                    Vector3 local = cameraController._root.InverseTransformPoint(corner);

                    min = Vector3.Min(min, local);
                    max = Vector3.Max(max, local);
                }
            }

            Vector3 size = max - min;
            Vector3 centerLocal = (min + max) * 0.5f;

            float fov = cameraController.FieldOfView * Mathf.Deg2Rad;
            float aspect = cameraController._camera.aspect;

            float distanceY = (size.y * 0.5f) / Mathf.Tan(fov / 2f);
            float distanceX = (size.x * 0.5f) / (Mathf.Tan(fov / 2f) * aspect);

            float distance = Mathf.Max(distanceX, distanceY) * _padding;

            // Move root to correct world position
            Vector3 centerWorld = cameraController._root.TransformPoint(centerLocal);
            return distance + Vector3.Distance(cameraController._root.position, centerWorld);
        }
    }
}