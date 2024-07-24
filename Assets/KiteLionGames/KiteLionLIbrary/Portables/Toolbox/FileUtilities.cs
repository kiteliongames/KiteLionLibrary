using UnityEngine;

namespace KiteLionGames.Utilities
{
    public static class FileUtilities
    {
        public static string GetPath(string fileName, string fileExtension, string folderPath = "")
        {
            return $"{Application.dataPath}/{folderPath}/{fileName}.{fileExtension}";
        }


        /// <summary>
        /// Recursively search for a file in the project, and return the absolute path to it.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>First found instance of file.</returns>
        public static string FindFileInProject(string fileName)
        {
            string[] files = System.IO.Directory.GetFiles(Application.dataPath, fileName, System.IO.SearchOption.AllDirectories);
            if (files.Length == 0)
                return null;

            return files[0];
        }
    }
}

