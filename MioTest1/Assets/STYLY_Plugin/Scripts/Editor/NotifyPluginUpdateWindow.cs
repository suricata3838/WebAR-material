using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace STYLY.Uploader
{
    /// <summary>
    /// PluginのUpdate通知を行うウィンドウ。
    /// PluginUpdateNotifierSettingsとVersionInformationの内容に従って機能する。
    /// </summary>
    internal class NotifyPluginUpdateWindow : EditorWindow
    {
        private PluginUpdateNotifierSettings settings;
        private VersionInformation versionInformation;

        private Vector2 windowSize = new Vector2(380, 140);

        private bool showAtStartup;

        public static void Open()
        {
            var window = (NotifyPluginUpdateWindow)EditorWindow.GetWindow(typeof(NotifyPluginUpdateWindow), false, "Update Plugin", true);
            window.Show();
        }

        // Windowを開いた状態でコンパイルが走った際に初期化するためOnEnable（Awakeだと開いたままだと呼ばれない）
        private void OnEnable()
        {
            Initialize();

            VersionInformationProvider.Instance.OnUpdateVersionInformation += OnUpdateVersionInformation;
        }

        private void OnDestroy()
        {
            VersionInformationProvider.Instance.OnUpdateVersionInformation -= OnUpdateVersionInformation;
        }

        private void Initialize()
        {
            this.settings = PluginUpdateNotifierSettings.LoadOrCreateSettings();
            this.versionInformation = VersionInformationProvider.Instance.VersionInformation;
            this.minSize = windowSize;
            this.showAtStartup = settings.ShowAtStartup;
        }

        private void OnUpdateVersionInformation(VersionInformation versionInformation)
        {
            this.versionInformation = versionInformation;
            // VersionInformationが更新されたら再描画する。
            this.Repaint();
        }

        private void RenderHeader()
        {
            GUILayout.Label("Update Checker", EditorStyles.boldLabel);
        }

        private void RenderPage()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.wordWrap = true;

            switch (versionInformation.GetStatus())
            {
                case VersionInformation.Status.UpdateAvailable:
                    {
                        EditorGUILayout.LabelField("There is a new version of Unity Plugin for STYLY available for download.", style);

                        EditorGUILayout.LabelField("The latest version is " + versionInformation.LatestVersion, style);

                        GUILayout.BeginHorizontal("box");
                        if (GUILayout.Button("Download new version", GUILayout.Width(180)))
                        {
                            Application.OpenURL(versionInformation.DownloadUrl);
                        }
                        if (GUILayout.Button("Skip this version", GUILayout.Width(180)))
                        {
                            settings.SkipVersion = versionInformation.LatestVersion;
                            Close();
                        }
                        GUILayout.EndHorizontal();
                    }
                    break;
                case VersionInformation.Status.UpToDate:
                    {
                        EditorGUILayout.LabelField("Unity Plugin For STYLY is up to date.", style);
                    }
                    break;
                case VersionInformation.Status.Unavailable:
                    {
                        EditorGUILayout.LabelField("Failed to retrieve the data.", style);
                    }
                    break;
            }
        }

        private void RenderFooter()
        {
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            GUILayout.Label("Current Version:" + versionInformation.CurrentVersion);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            EditorGUI.BeginChangeCheck();

            showAtStartup = GUILayout.Toggle(showAtStartup, "Show At Startup When Update is Available");

            if (EditorGUI.EndChangeCheck())
            {
                settings.ShowAtStartup = showAtStartup;
            }

            GUILayout.EndHorizontal();
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical();

            RenderHeader();

            RenderPage();

            RenderFooter();

            GUILayout.EndVertical();
        }
    }
}
