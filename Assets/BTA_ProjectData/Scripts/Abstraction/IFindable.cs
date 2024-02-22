using System.Collections.Generic;
using UnityEngine;

namespace Abstraction
{
    public interface IFindable
    {
        public bool IsAvailable { get; }
        public GameObject GameObject { get; }
        public List<Transform> VisiblePoints { get; }
        public Vector3 GetPosition();
    }
}
