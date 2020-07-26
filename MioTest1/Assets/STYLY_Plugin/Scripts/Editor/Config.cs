using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace STYLY.Uploader
{
    public class Config
    {
        public const string CurrentVersion = "1.1.0";

        public static string[] UNITY_VERSIONS = { "2019.3" };

        /// <summary>
        /// STYLYアセット対象プラットフォームリスト
        /// アセットバンドルのビルド対象として利用
        /// </summary>
        public static RuntimePlatform[] PlatformList = new RuntimePlatform[] {
            RuntimePlatform.Android,
            RuntimePlatform.IPhonePlayer,
            RuntimePlatform.OSXPlayer,
            RuntimePlatform.WebGLPlayer,
            RuntimePlatform.WindowsPlayer,
//			RuntimePlatform.WSAPlayerX86, // UWP用
		};

        /// <summary>
        /// RuntimePlatformとBuildTargetの対応Dictionary
        /// </summary>
        public static Dictionary<RuntimePlatform, BuildTarget> PlatformBuildTargetDic = new Dictionary<RuntimePlatform, BuildTarget>()
        {
            {RuntimePlatform.Android, BuildTarget.Android },
            {RuntimePlatform.IPhonePlayer, BuildTarget.iOS},
            {RuntimePlatform.OSXPlayer, BuildTarget.StandaloneOSX },
            {RuntimePlatform.WebGLPlayer, BuildTarget.WebGL },
            {RuntimePlatform.WindowsPlayer, BuildTarget.StandaloneWindows64 },
            {RuntimePlatform.WSAPlayerX86, BuildTarget.WSAPlayer },
        };

        /// <summary>
        /// STYLYアセットに変換可能な拡張子一覧
        /// </summary>
        public static string[] AcceptableExtentions = new string[] {
            ".prefab",
            ".obj",
            ".fbx",
            ".skp",
            ".unity"
        };

        //禁止タグ
        public static string[] ProhibitedTags = new string[] {
            "MainCamera",
            "sphere",
            "FxTemporaire",
            "TeleportIgnore",
            "Fire",
            "projectile",
            "GameController",
            "EditorOnly",
            "Finish",
            "Respawn"
        };




        /// <summary>upload prefab</summary>
        public static string STYLY_TEMP_DIR = "styly_temp";
        public static string UploadPrefabName = "Assets/" + STYLY_TEMP_DIR + "/{0}.prefab";

        /// <summary>一時出力フォルダ</summary>
        public static string OutputPath = "_Output/";
        //		public static string AzureSignedUrls = "http://localhost:8999/api/v1/azure/unity_plugin/signed-url";
        //		public static string StylyAssetUploadCompleteUrl = "http://localhost:8999/api/v1/scene/unity_plugin/complete";
        public static string AzureSignedUrls = "https://api.styly.cc/api/v1/azure/unity_plugin/signed-url";
        public static string StylyAssetUploadCompleteUrl = "https://api.styly.cc/api/v1/scene/unity_plugin/complete";

        // StudioのAPI Key取得用ページのURL
        public static string GetAPIKeyUrl = "https://gallery.styly.cc/me/account";

        // TutorialのURL
        public static string TutorialUrl = "http://docs.styly.cc/category/unity-uploader/";

        public static int ThumbnailWidth = 640;
        public static int ThumbnailHeight = 480;

        #region PluginUpdate関連

        public const string VersionInformationJsonUrl = "https://build.styly.cc/unity-plugin/version.json";
        public const string LatestVersionKey = "latestVersion";
        public const string DownloadUrlKey = "downloadUrl";

        #endregion

        public const int LIMIT_PACKAGE_FILE_SIZE_MB = 256;
        public const int LIMIT_PACKAGE_FILE_SIZE_MB_AZCOPY = 512;

        // <summary>azcopyに与えるブロックサイズ値。アップロードの並列数に影響する。</summary>
        public const int AZCOPY_BLOCK_SIZE_MB = 4;
    }
}
