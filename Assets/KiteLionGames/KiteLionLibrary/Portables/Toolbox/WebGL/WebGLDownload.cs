#region FileHeader

// test
// Project: Assembly-CSharp
// File:    WebGLDownload.cs
// Author:  Eliot CS
// Created: 2024.09.18.01.09.15
// Edited: 2024.09.19.01.09.11
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
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using KiteLionGames.KiteLionLibrary.Portables.BetterDebug;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
namespace KiteLionGames.KiteLionLibrary.Portables.Toolbox.WebGL
{
    public class WebGLDownload : MonoBehaviour
    {
        public enum ImageFormat
        {
            jpg,
            png,
        }
        private bool _isRecording;
        [DllImport("__Internal")]
        private static extern void DownloadFileJsLib(byte[] byteArray, int byteLength, string fileName);
        [DllImport("__Internal")]
        private static extern void DownloadFileByPathJsLib(string base64, string extension);

        /// <summary>
        ///     ___
        ///     <para>bytes -> The bytes to be downloaded</para>
        ///     <para>fileName -> The downloaded file name (without extension)</para>
        ///     <para>fileExtension -> WebGLDownload.FileExtension.jpg/png/zip/</para>
        /// </summary>
        public void DownloadFile(byte[] bytes, string fileName, string fileExtension)
        {
            if (fileName == "") fileName = "UnnamedFile";
#if UNITY_EDITOR
            var path = EditorUtility.SaveFilePanel("Save file...", "", fileName, fileExtension);
            File.WriteAllBytes(path, bytes);
            CBUG.Log("File saved: " + path);
#elif UNITY_WEBGL
            CBUG.Log("WebGLDownload.DownloadingFile!");
            DownloadFileJsLib(bytes, bytes.Length, fileName + "." + fileExtension);
#endif
        }

        /// <summary>
        ///     Download a file by path, this will open the file in the browser.
        ///     Only works in WebGL or Editor.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileExtension"></param>
        public void DownloadFileByPath(string path, string fileExtension)
        {
            var bytes = File.ReadAllBytes(path);
            var base64 = Convert.ToBase64String(bytes);
#if UNITY_EDITOR
            // open file
            Process.Start(path);
            // open containing folder
            Process.Start(Path.GetDirectoryName(path));
            Debug.Log("File saved: " + path);
#elif UNITY_WEBGL
            CBUG.Log("WebGLDownload.DownloadingFile!");
            DownloadFileByPathJsLib(base64, fileExtension);
#endif
        }

        /// <summary>
        ///     ___
        ///     <para>imageFormat -> WebGLDownload.ImageFormat.jpg/png</para>
        ///     <para>screenshotUpscale -> Upscale the frame. default = 1</para>
        ///     <para>
        ///         fileName -> Optional filename. Empty filename creates a name texture.width x texture.height in pixel +
        ///         current datetime
        ///     </para>
        /// </summary>
        public void GetScreenshot(ImageFormat imageFormat, int screenshotUpscale, string fileName = "")
        {
            if (!_isRecording)
                this.StartCoroutine(RecordUpscaledFrame(imageFormat, screenshotUpscale, fileName));
        }

        private IEnumerator RecordUpscaledFrame(ImageFormat imageFormat, int screenshotUpscale, string fileName)
        {
            _isRecording = true;
            yield return new WaitForEndOfFrame();
            try
            {
                if (fileName == "")
                {
                    var resWidth = Camera.main.pixelWidth * screenshotUpscale;
                    var resHeight = Camera.main.pixelHeight * screenshotUpscale;
                    var dateFormat = "yyyy-MM-dd-HH-mm-ss";
                    fileName = resWidth + "x" + resHeight + "px_" + DateTime.Now.ToString(dateFormat);
                }
                var screenShot = ScreenCapture.CaptureScreenshotAsTexture(screenshotUpscale);
                if (imageFormat == ImageFormat.jpg) DownloadFile(screenShot.EncodeToJPG(), fileName, "jpg");
                else if (imageFormat == ImageFormat.png) DownloadFile(screenShot.EncodeToPNG(), fileName, "png");
                Destroy(screenShot);
            }
            catch (Exception e)
            {
                Debug.Log("Original error: " + e.Message);
            }
            _isRecording = false;
        }
    }
}
