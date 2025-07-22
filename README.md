# UnityToolBarExtension
Unity顶部工具栏拓展

## 效果展示
<img width="878" height="512" alt="image" src="https://github.com/user-attachments/assets/8336902d-39a6-4c63-ab2f-6c39ba407338" />

## 导入
将ToolBar文件夹放到任意Editor目录下

## 自定义拓展
在Editor脚本中为一个静态无参方法添加特性[ToolBar] 例如：
```
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
```
