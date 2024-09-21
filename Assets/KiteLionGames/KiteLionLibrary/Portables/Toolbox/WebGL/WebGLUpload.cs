#region FileHeader

// test
// Project: Assembly-CSharp
// File:    WebGLUpload.cs
// Author:  Eliot CS
// Created: 2024.09.18.01.09.15
// Edited: 2024.09.19.01.09.07
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
using System.Runtime.InteropServices;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace KiteLionGames.KiteLionLibrary.Portables.Toolbox.WebGL
{
    /// <summary>
    ///     Todo: Remove functionality not immediately relevant, like where it applies to a material.
    /// </summary>
    public class WebGLUpload : MonoBehaviour
    {
        public enum FileExtension
        {
            zip,
            fdx,
        }
        public enum ImageFormat
        {
            jpg,
            png,
        }
        private bool _nonReadable = true;
        private Image _targetImage;
        private Material _targetMaterial;
        public Action<byte[]> OnLoadFile;
        public Action<string> OnLoadString;

        public Action<Texture> OnLoadTexture;


        [DllImport("__Internal")]
        private static extern void UploadFileJsLib(string gameObjectName, string methodName, string fileExtension);
        [DllImport("__Internal")]
        private static extern void UploadTextureJsLib(string gameObjectName, string methodName, int maxSize, string imageFormat);

        /// <summary>
        ///     Convert byte[] of ascii chars to string
        /// </summary>
        /// <param name="buff"></param>
        /// <returns></returns>
        public static string ByteToString(byte[] buff)
        {
            return Encoding.ASCII.GetString(buff);
        }

        /// <summary>
        ///     ___
        ///     <para>
        ///         imageFormat -> Use "jpg" to allow jpg and png images. Use "png" if you need textures with alpha! (allow png
        ///         only) you can edit the filter in the .jslib file
        ///     </para>
        ///     <para>
        ///         maxSize -> downsize large images. Max pixel size for the larger side (width or height, only for WebGL ->
        ///         function in the .jslib) 0 = disabled
        ///     </para>
        ///     <para>nonReadable -> should be "true" unless you have to edit the pixels (less memory usage)</para>
        ///     <para>targetMaterial -> set a material for the texture target. default = null</para>
        ///     <para>targetImage -> set a image for the texture target (it creates a sprite). default = null</para>
        /// </summary>
        public void UploadTexture(ImageFormat imageFormat, int maxSize, bool nonReadable, Material targetMaterial = null, Image targetImage = null)
        {
            _nonReadable = nonReadable;
            _targetMaterial = targetMaterial;
            _targetImage = targetImage;
#if UNITY_EDITOR
            string[] allImages =
            {
                "images", imageFormat.ToString(),
            };
            if (imageFormat == ImageFormat.jpg)
                allImages = new[]
                {
                    "jpg/png images", "png,jpg,jpeg",
                };
            var path = EditorUtility.OpenFilePanelWithFilters("Load a texture...", "", allImages);
            //string path = UnityEditor.EditorUtility.OpenFilePanel("Load a texture...", "", imageFormat.ToString());
            this.StartCoroutine(LoadTexture(path));
#elif UNITY_WEBGL
            UploadTextureJsLib(gameObject.name, "LoadTexture", maxSize, imageFormat.ToString());
#endif
        }

        /// <summary>
        ///     ___
        ///     <para>fileExtension -> Use your file extension "zip" e.g.</para>
        ///     <para>fileExtension -> Edit the enum values to add more extensions</para>
        /// </summary>
        public void UploadFile(FileExtension fileExtension)
        {
#if UNITY_EDITOR
            var path = EditorUtility.OpenFilePanel("Load a file...", "", fileExtension.ToString());
            this.StartCoroutine(LoadFile(path));
#elif UNITY_WEBGL
            UploadFileJsLib(gameObject.name, "LoadFile", fileExtension.ToString());
#endif
        }

        //Load the texture from blob or from url. Called from the .jslib
        private IEnumerator LoadTexture(string url)
        {
            using var uwr = UnityWebRequestTexture.GetTexture(url, _nonReadable);
            yield return uwr.SendWebRequest();
            if (uwr.error != null) Debug.Log(uwr.error);
            else
            {
                var texture = DownloadHandlerTexture.GetContent(uwr);
                Debug.Log("Loaded texture size: " + texture.width + "x" + texture.height + "px" + " | URL: " + url);

                //apply the texture to a material or image
                if (_targetMaterial) SetMaterialTexture(_targetMaterial, texture, false);
                else if (_targetImage) _targetImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
                else
                {
                    OnLoadTexture?.Invoke(texture);
                }
            }
        }

        //URP -> Set the material textures
        public void SetMaterialTexture(Material mat, Texture2D tex, bool emissionInclusive)
        {
            mat.SetTexture("_BaseMap", tex);
            if (emissionInclusive) mat.SetTexture("_EmissionMap", tex);
        }

        //Load the byte[] from blob or from url. Called from the .jslib
        private IEnumerator LoadFile(string url)
        {
            using var uwr = UnityWebRequest.Get(url);
            yield return uwr.SendWebRequest();
            if (uwr.error != null) Debug.Log(uwr.error);
            else
            {
                var result = new byte[uwr.downloadHandler.data.Length];
                Array.Copy(uwr.downloadHandler.data, 0, result, 0, uwr.downloadHandler.data.Length);
                Debug.Log("Loaded file size: " + uwr.downloadHandler.data.Length + " bytes");

                OnLoadString?.Invoke(ByteToString(result));
            }
        }

        private void ByteResultExamples(byte[] result)
        {
            Debug.Log("unused file here");

            //Zip example (Zip / gzip Multiplatform Native Plugin from the asset store)
            //----
            //bool validZip = lzip.validateFile(null, result);
            //if (validZip)
            //{
            //      bool exist = lzip.entryExists(null, "data/" + "myfile.dat", result);
            //      if (exist)
            //      {
            //          byte[] fileBuffer = lzip.entry2Buffer(null, "data/" + "myfile.dat", result);
            //          string myfileString = System.Text.Encoding.ASCII.GetString(fileBuffer);
            //          fileBuffer = lzip.entry2Buffer(null, "tex/" + "mytexture.jpg", result);
            //          Texture2D mytexture = new Texture2D(2, 2);
            //          mytexture.LoadImage(fileBuffer);
            //      }
            //}

            //Texture2D example (if byte[] array from an image)
            //----
            //Texture2D tex = new Texture2D(2, 2);
            //tex.LoadImage(result);
        }
    }
}
