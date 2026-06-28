using UnityEngine;

namespace Toge.Core
{
    public static class SystemsBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void EnsureSystems()
        {
            if (Object.FindFirstObjectByType<PersistentRoot>() != null) return;

            GameObject prefab = Resources.Load<GameObject>("_GameSystems");
            if (prefab != null) Object.Instantiate(prefab);
        }
    }
}
