using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = nameof(GameConfig), menuName = "B.T.A/" + nameof(GameConfig))]
    public class GameConfig : ScriptableObject
    {
        [field: SerializeField] public string PlayFabTitleId { get; private set; }
        [field: SerializeField] public ItemsInfoConfig ItemsInfoConfig { get; private set; }
    }
}
