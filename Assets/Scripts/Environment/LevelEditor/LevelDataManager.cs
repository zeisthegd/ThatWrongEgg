using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

using TMPro;
using Penwyn.Tools;

namespace Penwyn.LevelEditor
{
    public class LevelDataManager : SingletonMonoBehaviour<LevelDataManager>
    {
#if UNITY_EDITOR
        [Header("--- Save and Load ---")]
        public TMP_InputField FileLoadName;
        public TMP_InputField FileSaveName;


        [Header("--- Scriptable Object Prefix ---")]//* These are the strings that indicate the type of the resouce. Used to manage files.
        public string LevelDataSfx;

        [Header("--- Game Settings ---")]
        public LevelDataList LevelDataList;
        public LevelData LevelData;

        public const string SaveFolderPath = "Assets/Resources/LevelDatas/";


        /// <summary>
        /// Save Button. Can be used if the input text is not null or empty.
        /// First, get the save file path.
        /// Then for each component of the GameEditor, call the SaveData function on that component.
        /// If the save directory doesn't exists, create a new directory.
        /// </summary>
        public void Save()
        {
            InputReader.Instance.DisableGameplayInput();
            if (string.IsNullOrEmpty(FileSaveName.text))
            {
                Debug.Log("Null input");
                return;
            }

            string levelName = FileSaveName.text;
            string folderPath = GetFolderPath(FileSaveName.text);

            if (!FolderExists(folderPath))
                AssetDatabase.CreateFolder(SaveFolderPath.TrimEnd('/'), FileSaveName.text);

            LevelData = ScriptableObject.CreateInstance<LevelData>();

            LevelData.LevelName = FileSaveName.text;
            LevelEditor.Instance.SaveData(GetFilePath(folderPath, levelName, LevelDataSfx));

            LevelDataList.Add(LevelData);

            AssetDatabase.CreateAsset(LevelData, GetFilePath(folderPath, levelName, LevelDataSfx));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Saved");
        }

        /// <summary>
        /// Load Button. Can be used if the input text is not null or empty.
        /// First, get the load file path.
        /// Then for each component of the GameEditor, call the LoadData function on that component.
        /// </summary>
        public void Load()
        {
            InputReader.Instance.DisableGameplayInput();
            if (string.IsNullOrEmpty(FileLoadName.text))
                return;
            FileSaveName.text = FileLoadName.text;
            string levelName = FileLoadName.text;
            string folderPath = GetFolderPath(FileLoadName.text);

            if (FolderExists(folderPath))
            {
                LevelData = (LevelData)AssetDatabase.LoadAssetAtPath(GetSettingsPath(folderPath, levelName), typeof(LevelData));
                LevelEditor.Instance.LoadData(GetFilePath(folderPath, levelName, LevelDataSfx));
            }
            else
            {
                Debug.LogWarning("Level not yet created.");
            }
        }

        public string GetSettingsPath(string folderPath, string levelName)
        {
            return GetFilePath(folderPath, levelName, LevelDataSfx);
        }

        /// <summary>
        /// Get FolderPath.
        /// </summary>
        /// <param name="levelName">Level name</param>
        private string GetFolderPath(string levelName)
        {
            return $"{SaveFolderPath}{levelName}";
        }

        /// <summary>
        /// Get FilePath
        /// </summary>
        /// <param name="folderPath">Folder path</param>
        /// <param name="levelName">Level name</param>
        /// <param name="prefix">Type of resource.</param>
        /// <returns></returns>
        public string GetFilePath(string folderPath, string levelName, string suffix)
        {
            return $"{folderPath}/{levelName}_{suffix}.asset";
        }

        /// <summary>
        /// Check if folder directory exists.
        /// </summary>
        /// <param name="folderPath">Folder path</param>
        private bool FolderExists(string folderPath)
        {
            return System.IO.Directory.Exists(Application.dataPath + "/" + folderPath.Remove(0, 7));
        }
#endif
    }
}