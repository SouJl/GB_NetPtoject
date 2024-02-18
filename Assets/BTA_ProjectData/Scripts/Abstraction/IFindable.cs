using System.Collections.Generic;
using UnityEngine;

namespace Abstraction
{
    public interface IFindable
    {
        public Transform Transform { get; }
        public GameObject GameObject { get; }
        public List<Transform> VisiblePoints { get; }
    }
}
