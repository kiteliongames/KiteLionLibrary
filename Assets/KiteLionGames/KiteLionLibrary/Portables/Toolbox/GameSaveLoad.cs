#region FileHeader

// test
// Project: Assembly-CSharp
// File:    GameSaveLoad.cs
// Author:  Eliot CS
// Created: 2024.09.18.03.09.10
// Edited: 2024.09.19.01.09.03
//
// Copyright (c) 2024 SomeGameDevs, LLC. All rights reserved.
//
// This source code is the property of SomeGameDevs, LLC and may not be
// copied, distributed, modified, or used in any way without prior written
// permission from SomeGameDevs, LLC.
//
// Description:
// [Provide a brief description of what this file/class does.]
//
// Previous Header (if any):
//
// License:
// This code is provided "as is," without warranty of any kind, express or
// implied, including but not limited to the warranties of merchantability,
// fitness for a particular purpose, and noninfringement. In no event shall
// the authors or copyright holders be liable for any claim, damages, or
// other liability, whether in an action of contract, tort, or otherwise,
// arising from, out of, or in connection with the software or the use or
// other dealings in the software.

#endregion

/* Copyright (C) KiteLion Games, LLC - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 *
 * Written by Eliot Carney-Seim <support@kiteliongames.com>, January 2023
 */

using System.IO;
using System.Linq;
using KiteLionGames.KiteLionLibrary.Portables.BetterDebug;
using KiteLionGames.KiteLionLibrary.Portables.Legal.Legal;
using KiteLionGames.KiteLionLibrary.Portables.Utilities;
using Newtonsoft.Json;
using UnityEngine;

//NOTICE A: REMOVAL OR MODIFICATION OF THE LINES ABOVE 'NOTICE B' VOIDS ANY AND ALL RESPONSIBLITY AND SUPPORT OF THIS SOFTWARE BY KITELION GAMES, LLC AND IT'S PARTNERS.
namespace KiteLionGames.KiteLionLibrary.Portables.Toolbox
{
    public class GameSaveLoad : ILegal
    {
        public static bool SafeToMakeChanges = true;
        private static bool s_isInitialized;
        /// <summary>
        ///     Game data does NOT load automatically. "LoadGameDataFromDisk()" must be called.
        /// </summary>
        public static bool IsGameDataLoaded { get; private set; }
        public static Tools.ByteArrayExtensions.ByteArrayPool SAVE_DATA { get; private set; } = new Tools.ByteArrayExtensions.ByteArrayPool();
        public static string SaveFilePath { get; private set; }
        public static string SaveDirectoryPath { get => Path.GetDirectoryName(SaveFilePath); }
        public string KiteLionGamesSoftwareName { get => typeof(GameSaveLoad).Name; }

        /// <summary>
        ///     If not called previously, will be called automatically by SaveGameDataToDisk() and LoadGameDataFromDisk() methods.
        ///     Call this on load time to prevent io errors during runtime.
        /// </summary>
        public static void Initialize()
        {
            Copyright.RecordUsage(new GameSaveLoad());
            s_isInitialized = true;

            // Construct the save file path using the persistent data path and a custom file name
            SaveFilePath = Path.Combine(Application.persistentDataPath, "savedGameData.dat");
            if (File.Exists(SaveFilePath))
            {
                LoadGameDataFromDisk();
            }
            else
            {
                SaveGameDataToDisk();//Create file if it doesn't exist.
                LoadGameDataFromDisk();
            }
        }

        /// <summary>
        ///     Saves in scope of current session. Use SaveGameDataToDisk() to save to disk. AFTER calling this method.
        ///     TODO: Handle case where data is larger than original data.
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

            if (SAVE_DATA.Keys.Contains(key))
            {
                SAVE_DATA.UpdateItem(key, data);
            }
            else
            {
                SAVE_DATA.AddItem(key, data);
            }
        }

        /// <summary>
        ///     Grabs data from memory. Did you call LoadGameDataFromDisk()?
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] GetData(string key)
        {
            if (IsGameDataLoaded)
            {
                if (SAVE_DATA.Keys.Contains(key))
                {
                    return SAVE_DATA.GetItem(key).Data;
                }
                CBUG.Do("No data found for key: " + key);
                return null;
            }
            CBUG.Do($"Game data not loaded! Call {typeof(GameSaveLoad).Name}.Initialize()");
            return null;
        }

        /// <summary>
        ///     Deletes data from the save file. MUST CALL SAVEGAMEDATATODISK() AFTER CALLING THIS METHOD. (Or it will be reloaded
        ///     from disk.)
        /// </summary>
        /// <returns>True if successful, false if not.</returns>
        public static bool DeleteData(string key)
        {
            if (IsGameDataLoaded == false)
            {
                CBUG.Do("Game data not loaded!");
                return false;
            }
            return SAVE_DATA.RemoveItem(key);
        }

        /// <summary>
        ///     Saves to disk. See LoadGameDataFromDisk() to load from disk.
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
            var json = JsonConvert.SerializeObject(SAVE_DATA);

            // Write the JSON data to the save file
            File.WriteAllText(SaveFilePath, json);

            Debug.Log("Game data saved at: " + SaveFilePath);
            SafeToMakeChanges = true;
        }

        /// <summary>
        ///     Loads from memory. See SaveGameDataToDisk() to save to disk.
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
            if (File.Exists(SaveFilePath))
            {
                // Read the JSON data from the save file
                var json = File.ReadAllText(SaveFilePath);

                // Deserialize the JSON data to GameData object
                SAVE_DATA = JsonConvert.DeserializeObject<Tools.ByteArrayExtensions.ByteArrayPool>(json);

                CBUG.Do("Game data loaded from: " + SaveFilePath);
            }
            else
            {
                CBUG.LogWarning("Save file not found: " + SaveFilePath);
            }
            SafeToMakeChanges = true;
            IsGameDataLoaded = true;
        }

        /// <summary>
        ///     Will nuke your save file. Use with caution.
        /// </summary>
        public static void WipeSaveFile()
        {
            SAVE_DATA = new Tools.ByteArrayExtensions.ByteArrayPool();
            SaveGameDataToDisk();
        }
    }
}
//NOTICE B: REMOVAL OR MODIFICATION OF THE LINES BELOW 'NOTICE A' VOIDS ANY AND ALL RESPONSIBLITY AND SUPPORT OF THIS SOFTWARE BY KITELION GAMES, LLC AND IT'S PARTNERS.
