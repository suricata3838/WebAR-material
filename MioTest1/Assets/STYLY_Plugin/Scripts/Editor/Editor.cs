using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEditor;

namespace STYLY.Uploader
{
    [InitializeOnLoad]
    public class Editor : MonoBehaviour
    {
        static Editor()
        {
            EditorApplication.update += Startup;
        }


        static void Startup()
        {
            EditorApplication.update -= Startup;

            bool requirementSatisfied = true;
            string email = EditorPrefs.GetString(Settings.SETTING_KEY_STYLY_EMAIL);
            string api_key = EditorPrefs.GetString(Settings.SETTING_KEY_STYLY_API_KEY);
            if (email.Length == 0 || api_key.Length == 0)
            {
                requirementSatisfied = false;
            }
            // http://answers.unity3d.com/questions/1324195/detect-if-build-target-is-installed.html
            var moduleManager = System.Type.GetType("UnityEditor.Modules.ModuleManager,UnityEditor.dll");
            var isPlatformSupportLoaded = moduleManager.GetMethod("IsPlatformSupportLoaded", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            var getTargetStringFromBuildTarget = moduleManager.GetMethod("GetTargetStringFromBuildTarget", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            GUIStyle platformStyle = new GUIStyle();
            GUIStyleState platformStyleState = new GUIStyleState();
            platformStyleState.textColor = Color.red;
            platformStyle.normal = platformStyleState;
            if (!(bool)isPlatformSupportLoaded.Invoke(null, new object[] { (string)getTargetStringFromBuildTarget.Invoke(null, new object[] { BuildTarget.StandaloneWindows64 }) }))
            {
                requirementSatisfied = false;
            }
            if (!(bool)isPlatformSupportLoaded.Invoke(null, new object[] { (string)getTargetStringFromBuildTarget.Invoke(null, new object[] { BuildTarget.Android }) }))
            {
                requirementSatisfied = false;
            }
            if (!(bool)isPlatformSupportLoaded.Invoke(null, new object[] { (string)getTargetStringFromBuildTarget.Invoke(null, new object[] { BuildTarget.iOS }) }))
            {
                requirementSatisfied = false;
            }
            if (!(bool)isPlatformSupportLoaded.Invoke(null, new object[] { (string)getTargetStringFromBuildTarget.Invoke(null, new object[] { BuildTarget.StandaloneOSX }) }))
            {
                requirementSatisfied = false;
            }
            if (!(bool)isPlatformSupportLoaded.Invoke(null, new object[] { (string)getTargetStringFromBuildTarget.Invoke(null, new object[] { BuildTarget.WebGL }) }))
            {
                requirementSatisfied = false;
            }
            if (requirementSatisfied == false)
            {
                OpenSettings();
            }
        }


        private static bool isUploading = false;
        public static bool IsUploading { get { return isUploading; } }
        //http://baba-s.hatenablog.com/entry/2014/05/13/213143
        /// <summary>
        /// 選択中のPrefabアセットをアセットバンドルとしてビルドしてアップロードする。
        /// </summary>
        [MenuItem(@"Assets/STYLY/Upload prefab or scene to STYLY", false, 10000)]
        private static void BuildAndUpload()
        {
            // アセットのアップローディング中であることを表すフラグ
            // SceneProcessorがビルド対象シーンに処理を施していいかどうか判断する基準となる
            isUploading = true;

            try
            {
                BuildAndUploadImplement();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }

            // アップロードがおわったらフラグをオフ
            isUploading = false;
        }

        private static void BuildAndUploadImplement()
        {
            if (CheckSelectedObjectIsPrefab())
            {
                bool isError = false;
                // 選択中のPrefabアセットパス, アセット名を取得
                var assetList = new List<UnityEngine.Object>();
                assetList.AddRange(Selection.objects);
                var unprocessedAssetList = new List<UnityEngine.Object>();
                unprocessedAssetList.AddRange(Selection.objects);
                string errorMessages = "";

                int count = 0;
                int selectLength = Selection.objects.Length;
                for (count = 0; count < selectLength; count++)
                {
                    var selectObject = assetList[count];
                    Converter converter = new Converter(selectObject);

                    if (!converter.BuildAsset() || converter.error != null)
                    {
                        isError = true;
                        errorMessages += "Failed to upload object <" + selectObject.name + ">";
                        if (converter.error != null)
                        {
                            errorMessages += " : " + converter.error.message;
                        }
                        errorMessages += "\r\n";
                    }
                    else
                    {
                        Debug.Log(selectObject.name + " Upload Success!");
                        unprocessedAssetList.Remove(selectObject);
                    }
                }

                if (isError)
                {
                    // エラーが発生した場合、処理されていないオブジェクトを選択する。
                    Selection.objects = unprocessedAssetList.ToArray();
                    Editor.ShowErrorDialog(errorMessages);
                }
                else
                {
                    Editor.ShowUploadSucessDialog();
                }
            }
        }

        /// <summary>
        /// 選択中のPrefab他アセットのサイズを調べます
        /// </summary>
        [MenuItem(@"Assets/STYLY/Check File Size", false, 10001)]
        private static void CheckFileSize()
        {
            if (CheckSelectedObjectIsPrefab())
            {
                var asset = Selection.objects[0];
                var assetPath = AssetDatabase.GetAssetPath(asset);
                string AssetBundlesOutputPath = Config.OutputPath + "STYLY_ASSET";
                //フォルダのクリーンアップ
                Converter.Delete(AssetBundlesOutputPath);
                //パッケージ形式でExport
                if (!Directory.Exists(AssetBundlesOutputPath + "/Packages/"))
                    Directory.CreateDirectory(AssetBundlesOutputPath + "/Packages/");
                var exportPackageFile = AssetBundlesOutputPath + "/Packages/" + "temp_for_for_filesize_check" + ".unitypackage";
                AssetDatabase.ExportPackage(assetPath, exportPackageFile, ExportPackageOptions.IncludeDependencies);
                //ファイルサイズチェック
                System.IO.FileInfo fi = new System.IO.FileInfo(exportPackageFile);
                long fileSize = fi.Length;
                //Debug.Log("File Size: " + fileSize.ToString("#,0") + " Byte");
                if (fileSize < 1024 * 1024)
                {
                    Editor.ShowFileSizeDialog(((fileSize / 1024).ToString("#,0") + " KByte"));
                }
                else
                {
                    Editor.ShowFileSizeDialog(((fileSize / (1024 * 1024)).ToString("#,0") + " MByte"));
                }
            }
        }

        [MenuItem(@"Assets/STYLY/Settings", false, 10002)]
        private static void OpenSettings()
        {
            var settings = ScriptableObject.CreateInstance<Settings>();
            settings.Show();
        }

        [MenuItem("STYLY/Asset Uploader Settings")]
        static void ShowSettingView()
        {
            var settings = ScriptableObject.CreateInstance<Settings>();
            settings.Show();
        }

        static bool CheckSelectedObjectIsPrefab()
        {
            // check only one prefab selected
            Error error = null;
            if (Selection.objects.Length == 0)
            {
                error = new Error("There is no prefab selected.");
            }
            if (error != null)
            {
                error.ShowDialog();
                return false;
            }
            return true;
        }

        public static void ShowUploadProgress(string description, float t)
        {
            //			EditorUtility.DisplayProgressBar ("STYLY Asset Uploader", description, t);
            //			if (t >= 1f) {
            //				EditorUtility.ClearProgressBar ();
            //			}
        }

        public static void ShowWaringDialog(string description)
        {
            ShowDialog("Warning", description, "OK");
        }

        public static void ShowErrorDialog(string description)
        {
            ShowDialog("Asset Upload failed", description, "OK");
        }

        public static void ShowFileSizeDialog(string size)
        {
            ShowDialog("Asset File Size", size, "OK");
        }

        public static void ShowUploadSucessDialog()
        {
            ShowDialog("Asset Upload", "Upload succeeded.", "OK");
        }

        private static void ShowDialog(string title, string description, string buttonName)
        {
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog(title, description, buttonName);
        }
    }

    public class Settings : EditorWindow
    {
        private enum CacheServerMode { Local, Remote, Disabled }
        private enum CacheServer2Mode { Enabled, Disabled }

        const string CacheServerModeKey = "CacheServerMode";
        const string CacheServerEnabledKey = "CacheServerEnabled";
        const string CacheServer2ModeKey = "CacheServer2Mode";

        public const string SETTING_KEY_STYLY_EMAIL = "SUITE.STYLY.CC.ASSET_UPLOADER_EMAIL";
        public const string SETTING_KEY_STYLY_API_KEY = "SUITE.STYLY.CC.API_KEY";
        public const string SETTING_KEY_STYLY_AZCOPY_PATH = "SUITE.STYLY.CC.AZCOPY_PATH";

        public string email;
        public string api_key;
        public string azcopyPath;

        private System.Reflection.MethodInfo isPlatformSupportLoaded;
        private System.Reflection.MethodInfo getTargetStringFromBuildTarget;

        bool IsPlatformSupportLoaded(string platform)
        {
            return (bool)isPlatformSupportLoaded.Invoke(null, new object[] { platform });
        }

        string GetTargetStringFromBuildTarget(BuildTarget target)
        {
            return (string)getTargetStringFromBuildTarget.Invoke(null, new object[] { target });
        }

        /// <summary>
        /// Check if cache server is enabled or not, including cache server for AssetPipeline-v2.
        /// </summary>
        private static bool IsCacheServerEnabled() 
        {
            if (IsCacheServerV1Enabled()) 
            {
                return true;
            }

            if (IsCacheServerV2Enabled()) 
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// returns true if the cache server is enabled.
        /// </summary>
        private static bool IsCacheServerV1Enabled() 
        {
            var defaultValue = EditorPrefs.GetBool(CacheServerEnabledKey) ? CacheServerMode.Remote : CacheServerMode.Disabled;
            var cacheServerMode = (CacheServerMode)EditorPrefs.GetInt(CacheServerModeKey, (int)defaultValue);
            // cache server v1 is enabled when CacheServerMode.Local or CacheServerMode.Remote.
            return cacheServerMode == CacheServerMode.Local || cacheServerMode == CacheServerMode.Remote;
        }

        /// <summary>
        /// returns true if the cache server for AssetPipelineV2 is enabled.
        /// </summary>
        private static bool IsCacheServerV2Enabled() 
        {
#if UNITY_2019_1_OR_NEWER
            return (CacheServer2Mode)EditorPrefs.GetInt(CacheServer2ModeKey, (int)CacheServer2Mode.Disabled) == CacheServer2Mode.Enabled;
#else
            return false;
#endif
        }

        void Awake()
        {
            this.minSize = new Vector2(400, 300);
            email = EditorPrefs.GetString(SETTING_KEY_STYLY_EMAIL);
            api_key = EditorPrefs.GetString(SETTING_KEY_STYLY_API_KEY);
            azcopyPath = EditorPrefs.GetString(SETTING_KEY_STYLY_AZCOPY_PATH);

            // http://answers.unity3d.com/questions/1324195/detect-if-build-target-is-installed.html
            var moduleManager = System.Type.GetType("UnityEditor.Modules.ModuleManager,UnityEditor.dll");
            isPlatformSupportLoaded = moduleManager.GetMethod("IsPlatformSupportLoaded", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            getTargetStringFromBuildTarget = moduleManager.GetMethod("GetTargetStringFromBuildTarget", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        }

        void OnGUI()
        {
            GUILayout.Label("Unity plugin for STYLY", EditorStyles.boldLabel);
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.wordWrap = true;
            EditorGUILayout.LabelField("Right click on a prefab and select \"STYLY\"-\"Upload prefab or scene to STYLY\". Your prefab will appear in \"3D Model\"-\"My Models\" section in STYLY. ", style);

            GUILayout.BeginHorizontal("box");
            if (GUILayout.Button("Get STYLY API Key", GUILayout.Width(120)))
            {
                Application.OpenURL(Config.GetAPIKeyUrl);
            }
            if (GUILayout.Button("Open Tutorial", GUILayout.Width(120)))
            {
                Application.OpenURL(Config.TutorialUrl);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUIStyle platformStyle = new GUIStyle();
            GUIStyleState platformStyleState = new GUIStyleState();
            platformStyleState.textColor = Color.red;
            platformStyle.normal = platformStyleState;
            bool allPlatformInstalled = true;

            foreach (var platform in Config.PlatformList)
            {
                var target = Config.PlatformBuildTargetDic[platform];
                if (!IsPlatformSupportLoaded(GetTargetStringFromBuildTarget(target)))
                {
                    EditorGUILayout.LabelField(target.ToString() + " module not installed.", platformStyle);
                    allPlatformInstalled = false;
                }
            }
            if (allPlatformInstalled == false)
            {
                EditorGUILayout.LabelField("STYLY Uploader requires above modules. Please install these modules.", platformStyle);
            }

            if (!IsCacheServerEnabled())
            {
                EditorGUILayout.LabelField("Cache Server is strongly recommended.\n Change the setting at Edit-Preferences-CacheServer", platformStyle);
            }

            GUILayout.Space(20);
            EditorGUI.BeginChangeCheck();

            email = EditorGUILayout.TextField("Email", email);
            GUILayout.Space(5);
            api_key = EditorGUILayout.TextField("API Key", api_key);
            GUILayout.Space(10);

            // azcopy setting
            EditorGUILayout.LabelField("azcopy can be used for faster upload (optional)");
            EditorGUILayout.BeginHorizontal();
            azcopyPath = EditorGUILayout.TextField("azcopy executable path", azcopyPath);
            if (GUILayout.Button("Select File", new GUILayoutOption[] { GUILayout.Width(80) }))
            {
                var path = EditorUtility.OpenFilePanel("Select azcopy executable", string.Empty, null);
                if (path.Length != 0)
                {
                    azcopyPath = path;
                    GUI.FocusControl(null);
                    Repaint(); // 値を反映させる
                }
            }
            EditorGUILayout.EndHorizontal();
            if (!string.IsNullOrEmpty(azcopyPath.Trim()) && !File.Exists(azcopyPath.Trim()))
            {
                EditorGUILayout.HelpBox("The file is not exist!", MessageType.Info);
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetString(SETTING_KEY_STYLY_EMAIL, email.Trim());
                EditorPrefs.SetString(SETTING_KEY_STYLY_API_KEY, api_key.Trim());
                EditorPrefs.SetString(SETTING_KEY_STYLY_AZCOPY_PATH, azcopyPath.Trim());
            }

            GUILayout.Space(20);
            this.Repaint();
        }
    }
}