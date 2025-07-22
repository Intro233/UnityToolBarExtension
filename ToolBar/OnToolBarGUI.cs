using ToolbarExtension;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace AD.Editor.ToolBar
{
    public class OnToolBarGUI
    {
        [Toolbar(OnGUISide.Right, 100)]
        private static void OnDeletePlayerPrefs()
        {
#if UNITY_2021_1_OR_NEWER
                if (GUILayout.Button("删除PlayerPrefs数据"))
#else
            if (GUILayout.Button("删除PlayerPrefs数据", GUILayout.Width(130)))
#endif
            {
                PlayerPrefs.DeleteAll();
                Debug.Log($"成功删除{nameof(PlayerPrefs)}数据!");
            }
        }

        private static EditorBuildSettingsScene[] Scens => EditorBuildSettings.scenes;

        [Toolbar(OnGUISide.Right, 90)]
        private static void OnSwitchSceneGUI()
        {
#if UNITY_2021_1_OR_NEWER
            if (GUILayout.Button("切换场景"))
#else
            if (GUILayout.Button("切换场景", GUILayout.Width(80)))
#endif

            {
                var menu = new GenericMenu();
                for (int i = 0; i < Scens.Length; i++)
                {
                    if (Scens[i].enabled)
                    {
                        var path = Scens[i].path;

                        var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                        if (scene == null)
                            continue;
                        menu.AddItem(new GUIContent($"[{i}] {scene.name}"), false,
                            () => { EditorSceneManager.OpenScene(path); });
                    }
                }

                menu.ShowAsContext();
            }
        }
    }
}