using UnityEngine;

namespace AdaptiveOpponent.Game
{
    /// <summary>Creates the duel screen when play starts, so the game needs no scene or prefab edits.</summary>
    public static class Bootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CreateScreen()
        {
            if (Object.FindFirstObjectByType<DuelScreen>() != null)
            {
                return;
            }
            var root = new GameObject("AdaptiveOpponent");
            root.AddComponent<DuelScreen>();
            Object.DontDestroyOnLoad(root);
        }
    }
}
