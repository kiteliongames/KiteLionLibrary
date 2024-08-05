/* Copyright (C) KiteLion Games, LLC - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * 
 * Written by Eliot Carney-Seim <support@kiteliongames.com>, January 2023
 */

using KiteLionGames.BetterDebug;
using KiteLionGames.Common;
using KiteLionGames.Legal;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

//NOTICE A: REMOVAL OR MODIFICATION OF THE LINES ABOVE 'NOTICE B' VOIDS ANY AND ALL RESPONSIBLITY AND SUPPORT OF THIS SOFTWARE BY KITELION GAMES, LLC AND IT'S PARTNERS.
namespace KiteLionGames.Managers
{
    public class GameSaveLoad : ILegal
    {
        public string KiteLionGamesSoftwareName { get => typeof(GameSaveLoad).Name; }
        /// <summary>
        /// Game data does NOT load automatically. "LoadGameDataFromDisk()" must be called.
        /// </summary>
        public static bool IsGameDataLoaded { get; private set; }
        public static bool SafeToMakeChanges = true;
        public static Tools.ByteArrayExtensions.ByteArrayPool SAVE_DATA { get { return _saveData; } }
        public static string SaveFilePath { get => s_saveFilePath; }
        public static string SaveDirectoryPath { get => Path.GetDirectoryName(s_saveFilePath); }
        private static Tools.ByteArrayExtensions.ByteArrayPool _saveData = new ();
        private static bool s_isInitialized = false;
        private static string s_saveFilePath;

        /// <summary>
        /// If not called previously, will be called automatically by SaveGameDataToDisk() and LoadGameDataFromDisk() methods.
        /// Call this on load time to prevent io errors during runtime.
        /// </summary>
        public static void Initialize()
        {
            Copyright.RecordUsage(new GameSaveLoad());
            s_isInitialized = true;

            // Construct the save file path using the persistent data path and a custom file name
            s_saveFilePath = Path.Combine(Application.persistentDataPath, "savedGameData.dat");
            if (File.Exists(s_saveFilePath))
            {
                LoadGameDataFromDisk();
            }
            else
            {
                SaveGameDataToDisk(); //Create file if it doesn't exist.
                LoadGameDataFromDisk();
            }
        }

        /// <summary>
        /// Saves in scope of current session. Use SaveGameDataToDisk() to save to disk. AFTER calling this method.
        /// TODO: Handle case where data is larger than original data.
        /// </summary>
        /// <param name="key">If not exists, is made. No checks necessary for you.</param>
        /// <param name="data">Must be in bytes. Sorry.</param>
        public static void Save(string key, byte[] data)
        {
            if (IsGameDataLoaded == false)
            {
                CBUG.Do("Game data not loaded! Call GameSaveLoad.Initialize()");
                return;
            }

            if (_saveData.Keys.Contains(key))
            {
                _saveData.UpdateItem(key, data);
            }
            else
            {
                _saveData.AddItem(key, data);
            }
        }

        /// <summary>
        /// Grabs data from memory. Did you call LoadGameDataFromDisk()?
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] GetData(string key)
        {
            if (IsGameDataLoaded)
            {
                if (_saveData.Keys.Contains(key))
                {
                    return _saveData.GetItem(key).Data;
                }
                else
                {
                    CBUG.Do("No data found for key: " + key);
                    return null;
                }
            }
            else
            {
                CBUG.Do($"Game data not loaded! Call {typeof(GameSaveLoad).Name}.Initialize()");
                return null;
            }
        }

        /// <summary>
        /// Deletes data from the save file. MUST CALL SAVEGAMEDATATODISK() AFTER CALLING THIS METHOD. (Or it will be reloaded from disk.)
        /// </summary>
        /// <returns>True if successful, false if not.</returns>
        public static bool DeleteData(string key)
        {
            if (IsGameDataLoaded == false)
            {
                CBUG.Do("Game data not loaded!");
                return false;
            }
            else
            {
                return _saveData.RemoveItem(key);
            }
        }

        /// <summary>
        /// Saves to disk. See LoadGameDataFromDisk() to load from disk.
        /// </summary>
        public static void SaveGameDataToDisk()
        {
            if (s_isInitialized == false)
                Initialize();

            if (!SafeToMakeChanges)
            {
                CBUG.Do("Not safe to make file changes!");
                return;
            }

            SafeToMakeChanges = false;
            // Serialize the game data to JSON
            string json = JsonConvert.SerializeObject(_saveData);

            // Write the JSON data to the save file
            File.WriteAllText(s_saveFilePath, json);

            Debug.Log("Game data saved at: " + s_saveFilePath);
            SafeToMakeChanges = true;
        }

        /// <summary>
        /// Loads from memory. See SaveGameDataToDisk() to save to disk.
        /// </summary>
        public static void LoadGameDataFromDisk()
        {
            if (s_isInitialized == false)
                Initialize();

            if (!SafeToMakeChanges)
            {
                CBUG.Do("Not safe to make file changes!");
                return;
            }

            SafeToMakeChanges = false;
            if (File.Exists(s_saveFilePath))
            {
                // Read the JSON data from the save file
                string json = File.ReadAllText(s_saveFilePath);

                // Deserialize the JSON data to GameData object
                _saveData = JsonConvert.DeserializeObject<Tools.ByteArrayExtensions.ByteArrayPool>(json);

                CBUG.Do("Game data loaded from: " + s_saveFilePath);
            }
            else
            {
                CBUG.LogWarning("Save file not found: " + s_saveFilePath);
            }
            SafeToMakeChanges = true;
            IsGameDataLoaded = true;
        }

        /// <summary>
        /// Will nuke your save file. Use with caution.
        /// </summary>
        public static void WipeSaveFile()
        {
            _saveData = new Tools.ByteArrayExtensions.ByteArrayPool();
            SaveGameDataToDisk();
        }
    }
}
//NOTICE B: REMOVAL OR MODIFICATION OF THE LINES BELOW 'NOTICE A' VOIDS ANY AND ALL RESPONSIBLITY AND SUPPORT OF THIS SOFTWARE BY KITELION GAMES, LLC AND IT'S PARTNERS.
