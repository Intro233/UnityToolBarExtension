using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ToolbarExtension
{
    [InitializeOnLoad]
    internal static class ToolbarExtension
    {
        static int m_toolCount;
        static GUIStyle m_commandStyle = null;
        private static readonly List<(int, Action)> s_LeftToolbarGUI = new List<(int, Action)>();
        private static readonly List<(int, Action)> s_RightToolbarGUI = new List<(int, Action)>();

        static ToolbarExtension()
        {
            Type toolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");

#if UNITY_2019_1_OR_NEWER
            string fieldName = "k_ToolCount";
#else
			string fieldName = "s_ShownToolIcons";
#endif

            FieldInfo toolIcons = toolbarType.GetField(fieldName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

#if UNITY_2019_1_OR_NEWER
            m_toolCount = toolIcons != null ? ((int)toolIcons.GetValue(null)) : 7;
#elif UNITY_2018_1_OR_NEWER
			m_toolCount = toolIcons != null ? ((Array) toolIcons.GetValue(null)).Length : 6;
#else
			m_toolCount = toolIcons != null ? ((Array) toolIcons.GetValue(null)).Length : 5;
#endif

            ToolbarCallback.OnToolbarGUI -= OnGUI;
            ToolbarCallback.OnToolbarGUI += OnGUI;

            ToolbarCallback.OnToolbarGUILeft = GUILeft;
            ToolbarCallback.OnToolbarGUIRight = GUIRight;

            Type attributeType = typeof(ToolbarAttribute);

            foreach (var methodInfo in TypeCache.GetMethodsWithAttribute<ToolbarAttribute>())
            {
                var attributes = methodInfo.GetCustomAttributes(attributeType, false);
                if (attributes.Length > 0)
                {
                    ToolbarAttribute attribute = (ToolbarAttribute)attributes[0];
                    if (attribute.Side == OnGUISide.Left)
                    {
                        s_LeftToolbarGUI.Add((attribute.Priority, delegate { methodInfo.Invoke(null, null); }));
                        continue;
                    }

                    if (attribute.Side == OnGUISide.Right)
                    {
                        s_RightToolbarGUI.Add((attribute.Priority, delegate { methodInfo.Invoke(null, null); }));
                        continue;
                    }
                }
            }

            s_LeftToolbarGUI.Sort((tuple1, tuple2) => tuple1.Item1 - tuple2.Item1);
            s_RightToolbarGUI.Sort((tuple1, tuple2) => tuple2.Item1 - tuple1.Item1);
        }

        static void GUILeft()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            foreach (var handler in s_LeftToolbarGUI)
            {
                handler.Item2();
            }

            GUILayout.EndHorizontal();
        }

        static void GUIRight()
        {
            GUILayout.BeginHorizontal();
            foreach (var handler in s_RightToolbarGUI)
            {
                handler.Item2();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }


        static void OnGUI()
        {
            if (m_commandStyle == null)
            {
                m_commandStyle = new GUIStyle("CommandLeft");
            }

            var screenWidth = EditorGUIUtility.currentViewWidth;

            float playButtonsPosition = (screenWidth - 100) / 2;

            Rect leftRect = new Rect(0, 0, screenWidth, Screen.height);
            leftRect.xMin += 10; // Spacing left
            leftRect.xMin += 32 * m_toolCount; // Tool buttons
            leftRect.xMin += 20; // Spacing between tools and pivot
            leftRect.xMin += 64 * 2; // Pivot buttons
            leftRect.xMax = playButtonsPosition;

            Rect rightRect = new Rect(0, 0, screenWidth, Screen.height);
            rightRect.xMin = playButtonsPosition;
            rightRect.xMin += m_commandStyle.fixedWidth * 3; // Play buttons
            rightRect.xMax = screenWidth;
            rightRect.xMax -= 10; // Spacing right
            rightRect.xMax -= 80; // Layout
            rightRect.xMax -= 10; // Spacing between layout and layers
            rightRect.xMax -= 80; // Layers
            rightRect.xMax -= 20; // Spacing between layers and account
            rightRect.xMax -= 80; // Account
            rightRect.xMax -= 10; // Spacing between account and cloud
            rightRect.xMax -= 32; // Cloud
            rightRect.xMax -= 10; // Spacing between cloud and collab
            rightRect.xMax -= 78; // Colab

            // Add spacing around existing controls
            leftRect.xMin += 10;
            leftRect.xMax -= 10;
            rightRect.xMin += 10;
            rightRect.xMax -= 10;

            // Add top and bottom margins
            leftRect.y = 5;
            leftRect.height = 24;
            rightRect.y = 5;
            rightRect.height = 24;

            if (leftRect.width > 0)
            {
                GUILayout.BeginArea(leftRect);
                GUILayout.BeginHorizontal();
                foreach (var handler in s_LeftToolbarGUI)
                {
                    handler.Item2?.Invoke();
                }

                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }

            if (rightRect.width > 0)
            {
                GUILayout.BeginArea(rightRect);
                GUILayout.BeginHorizontal();
                foreach (var handler in s_RightToolbarGUI)
                {
                    handler.Item2?.Invoke();
                }

                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }
        }
    }
}