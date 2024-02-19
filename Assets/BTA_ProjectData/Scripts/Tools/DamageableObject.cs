using Abstraction;
using UnityEngine;

namespace Tools
{
    public class DamageableObject : MonoBehaviour, IDamageable
    {
        private Rigidbody _rigidBody;

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody>();    
        }

        public void TakeDamage(DamageData damage)
        {
            _rigidBody.AddForce(damage.Force);
        }
    }
}
