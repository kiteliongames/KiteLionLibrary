#region FileHeader

// test
// Project: Assembly-CSharp
// File:    FileUtilities.cs
// Author:  Eliot CS
// Created: 2024.09.18.01.09.15
// Edited: 2024.09.19.01.09.08
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

using System.IO;
using UnityEngine;

namespace KiteLionGames.KiteLionLibrary.Portables.Toolbox
{
    public static class FileUtilities
    {
        public static string GetPath(string fileName, string fileExtension, string folderPath = "")
        {
            return $"{Application.dataPath}/{folderPath}/{fileName}.{fileExtension}";
        }

        /// <summary>
        ///     Recursively search for a file in the project, and return the absolute path to it.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>First found instance of file.</returns>
        public static string FindFileInProject(string fileName)
        {
            var files = Directory.GetFiles(Application.dataPath, fileName, SearchOption.AllDirectories);
            if (files.Length == 0)
                return null;

            return files[0];
        }
    }
}
