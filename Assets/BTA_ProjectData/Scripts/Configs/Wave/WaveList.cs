using Configs;
using System.Collections.Generic;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = nameof(WaveList), menuName = "B.T.A/" + nameof(WaveList))]
    public class WaveList : ScriptableObject
    {
        [field: SerializeField] public List<WaveConfig> Collection { get; private set; }
    }
}
