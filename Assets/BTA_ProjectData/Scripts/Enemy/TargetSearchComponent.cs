using Abstraction;
using UnityEngine;

namespace Enemy
{
    public class TargetSearchComponent : MonoBehaviour
    {

        public bool IsVisible(IFindable target, Transform from, float angle, float distance, LayerMask mask)
        {
            bool result = false;
            if (target != null)
            {
                foreach (Transform visiblePoint in target.VisiblePoints)
                {
                    if (IsVisibleObject(from, visiblePoint.position, target.GameObject, angle, distance, mask))
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        private bool IsVisibleObject(Transform from, Vector3 point, GameObject target, float angle, float distance, LayerMask mask)
        {
            bool result = false;
            if (IsAvailablePoint(from, point, angle, distance))
            {
                Vector3 direction = (point - from.position);
                Ray ray = new Ray(from.position, direction);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, distance, mask.value))
                {
                    if (hit.collider.gameObject == target)
                    {
                        result = true;
                    }
                }
            }
            return result;
        }


        private bool IsAvailablePoint(Transform from, Vector3 point, float angle, float distance)
        {
            bool result = false;

            if (from != null && Vector3.Distance(from.position, point) <= distance)
            {
                Vector3 direction = (point - from.position);
                float dot = Vector3.Dot(from.forward, direction.normalized);
                if (dot < 1)
                {
                    float angleRadians = Mathf.Acos(dot);
                    float angleDeg = angleRadians * Mathf.Rad2Deg;
                    result = (angleDeg <= angle);
                }
                else
                {
                    result = true;
                }
            }
            return result;
        }
    }
}
