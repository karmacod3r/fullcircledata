using UnityEngine;

namespace FullCircleData
{
    [DefaultExecutionOrder(-1)]
    public class PrefabListBase : BestBehaviour
    {
#if UNITY_EDITOR
        protected override void OnEnable()
        {
            base.OnEnable();
            UnityEditor.SceneManagement.PrefabStage.prefabSaving += OnPrefabSaving;
            UnityEditor.SceneManagement.PrefabStage.prefabStageClosing += OnPrefabStageClosing;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnityEditor.SceneManagement.PrefabStage.prefabSaving -= OnPrefabSaving;
            UnityEditor.SceneManagement.PrefabStage.prefabStageClosing -= OnPrefabStageClosing;
        }

        private void OnPrefabStageClosing(UnityEditor.SceneManagement.PrefabStage obj)
        {
            base.OnDisable();
            base.OnEnable();
        }

        private void OnPrefabSaving(GameObject obj)
        {
            CleanupPrefab();
        }
#endif

        protected virtual void CleanupPrefab()
        {
            // Destroy children in prefab edit mode
            DestroyChildren();
        }
   }
}