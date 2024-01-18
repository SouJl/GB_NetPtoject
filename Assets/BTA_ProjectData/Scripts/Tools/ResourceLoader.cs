using UnityEngine;

namespace Tools
{
    internal static class ResourceLoader
    {
        public static Sprite LoadSprite(ResourcePath path) =>
            LoadObject<Sprite>(path);

        public static GameObject LoadPrefab(ResourcePath path) =>
            LoadObject<GameObject>(path);

        public static TObject LoadObject<TObject>(ResourcePath path) where TObject : Object =>
            Resources.Load<TObject>(path.PathResource);
    }
}
