using UnityEngine;

namespace ArbazAminKhan.Rays.Presets
{
    public class Ray_Presets
    {
        public static bool Raycast(Vector2 screenPoint, out RaycastHit hit) => Physics.Raycast(Camera.main.ScreenPointToRay(screenPoint), out hit);
    }
}