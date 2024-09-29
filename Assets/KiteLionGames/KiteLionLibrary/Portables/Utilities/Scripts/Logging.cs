#region FileHeader

// test
// Project: Assembly-CSharp
// File:    Logging.cs
// Author:  Eliot CS
// Created: 2024.09.18.01.09.15
// Edited: 2024.09.19.01.09.18
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

using System;
using System.IO;
using KiteLionGames.KiteLionLibrary.Portables.BetterDebug;
using UnityEngine;

namespace KiteLionGames.KiteLionLibrary.Portables.Utilities.Scripts
{
    /// <summary>
    ///     Writes a log "line"
    ///     COPIED FROM:
    ///     https://stackoverflow.com/questions/69147519/save-and-load-line-from-application-persistentdatapath-works-in-unity-but-not-o
    /// </summary>
    /// <typeparam name="string"></typeparam>
    public static class Logging
    {
        private static StreamWriter StreamWriter;

        /// <summary>
        ///     WriteOnce line to a file. DO.NOT.WRITE.A.LOT. This is for single, intermittent logging.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="line">not really a line but whatever</param>
        /// <param name="folder">
        ///     folder name inside Unity default Application.streamingAssetsPath if on OSX, otherwise
        ///     Application.persistentDataPath.
        /// </param>
        /// <param name="file"></param>
        public static void WriteOnce(string line, string folder, string file)
        {
            if (StreamWriter != null)
            {
                CBUG.Error($"WriteOnce called before closing previous write session. Call 'WriteEnd' first. line: {line}");
                return;
            }

            var isWebGLBuild = false;
#if UNITY_WEBGL
            isWebGLBuild = true;
#endif

            if (isWebGLBuild)
            {
                Debug.Log(
                    "\r\nLog Entry : \n" +
                    $"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}\n" +
                    "  :\n" +
                    $"  :{line}\n" +
                    "-------------------------------"
                );
                return;
            }


            // get the path of this save line
            var dataPath = GetFilePath(folder, file);

            // create the file in the path if it doesn't exist
            // if the file path or name does not exist, return the default SO
            if (!Directory.Exists(Path.GetDirectoryName(dataPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(dataPath));
            }

            // attempt to save here line
            try
            {
                // save data here
                //todo create a coded-in reminder to empty the logging location.
                // Debug.Log("WriteOnce line to: " + dataPath);
                using var w = File.AppendText(dataPath);
                w.Write("\r\nLog Entry : ");
                w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
                w.WriteLine("  :");
                w.WriteLine($"  :{line}");
                w.WriteLine("-------------------------------");
            }
            catch (Exception e)
            {
                // write out error here
                Debug.LogError("Failed to save line to: " + dataPath);
                Debug.LogError("Error " + e.Message);
            }
        }

        /// <summary>
        ///     WriteStart line to a file. DO.NOT.WRITE.A.LOT. This is for single, intermittent logging.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="line">not really a line but whatever</param>
        /// <param name="folder"></param>
        /// <param name="file"></param>
        public static void WriteStart(string line, string folder, string file)
        {
            if (StreamWriter != null)
            {
                CBUG.Error($"WriteStart called before closing previous write session. Call 'WriteEnd' first. line: {line}");
                return;
            }

            var isWebGLBuild = false;
#if UNITY_WEBGL
            isWebGLBuild = true;
#endif

            if (isWebGLBuild)
            {
                Debug.Log(
                    "\r\nLog Entry : \n" +
                    $"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}\n" +
                    "  :\n" +
                    $"  :{line}\n" +
                    "-------------------------------"
                );
                return;
            }

            // get the path of this save line
            var dataPath = GetFilePath(folder, file);

            // create the file in the path if it doesn't exist
            // if the file path or name does not exist, return the default SO
            if (!Directory.Exists(Path.GetDirectoryName(dataPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(dataPath));
            }

            // attempt to save here line
            try
            {
                // save data here
                Debug.Log("WriteStart line to: " + dataPath);
                StreamWriter = File.AppendText(dataPath);

                StreamWriter.Write("\r\nLog Entry : ");
                StreamWriter.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
                StreamWriter.WriteLine("  :");
                StreamWriter.WriteLine($"  :{line}");
                StreamWriter.WriteLine("-------------------------------");
            }
            catch (Exception e)
            {
                // write out error here
                Debug.LogError("Failed to save line to: " + dataPath);
                Debug.LogError("Error " + e.Message);
            }
        }

        public static void WriteEnd()
        {
            StreamWriter.Close();
            StreamWriter = null;
        }

        /// <summary>
        ///     Create file path for where a file is stored on the specific platform given a folder name and file name
        /// </summary>
        /// <param name="FolderName"></param>
        /// <param name="FileName"></param>
        /// <returns></returns>
        private static string GetFilePath(string FolderName, string FileName = "")
        {
            string filePath;
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            // mac
            filePath = Path.Combine(Application.streamingAssetsPath, ("data/" + FolderName));

            if (FileName != "")
                filePath = Path.Combine(filePath, (FileName + ".txt"));
#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            // windows
            filePath = Path.Combine(Application.persistentDataPath, ("data/" + FolderName));

            if (FileName != "")
                filePath = Path.Combine(filePath, (FileName + ".txt"));
#elif UNITY_ANDROID
            // android
            filePath = Path.Combine(Application.persistentDataPath, ("data/" + FolderName));

            if (FileName != "")
                filePath = Path.Combine(filePath, (FileName + ".txt"));
#elif UNITY_IOS
            // ios
            filePath = Path.Combine(Application.persistentDataPath, ("data/" + FolderName));

            if (FileName != "")
                filePath = Path.Combine(filePath, (FileName + ".txt"));
#elif UNITY_WEBGL
            // webgl
            filePath = "";
#elif UNITY_LINUX || UNITY_STANDALONE_LINUX
            // linux
            filePath = Path.Combine(Application.persistentDataPath, "data/" + FolderName);

            if (FileName != "")
                filePath = Path.Combine(filePath, FileName + ".txt");
#endif
            return filePath;
        }
    }
}
///// <summary>
///// Load all data at a specified file and folder location
///// </summary>
///// <param name="folder"></param>
///// <param name="file"></param>
///// <returns></returns>
//public static string Load(string folder, string file)
//{
//    // get the data path of this save data
//    string dataPath = GetFilePath(folder, file);

//    // if the file path or name does not exist, return the default SO
//    if (!Directory.Exists(Path.GetDirectoryName(dataPath)))
//    {
//        Debug.LogWarning("File or path does not exist! " + dataPath);
//        return string;
//    }

//    // load in the save data as byte array
//    byte[] jsonDataAsBytes = null;

//    try
//    {
//        jsonDataAsBytes = File.ReadAllBytes(dataPath);
//        Debug.Log("<color=green>Loaded all data from: </color>" + dataPath);
//    }
//    catch (Exception e)
//    {
//        Debug.LogWarning("Failed to load data from: " + dataPath);
//        Debug.LogWarning("Error: " + e.Message);
//        return default(@string);
//    }

//    if (jsonDataAsBytes == null)
//        return default(@string);

//    // convert the byte array to json
//    string jsonData;

//    // convert the byte array to json
//    jsonData = Encoding.ASCII.GetString(jsonDataAsBytes);

//    // convert to the specified object type
//    @string returnedData = JsonUtility.FromJson<@string>(jsonData);

//    // return the casted json object to use
//    return (@string)Convert.ChangeType(returnedData, typeof(@string));
//}
