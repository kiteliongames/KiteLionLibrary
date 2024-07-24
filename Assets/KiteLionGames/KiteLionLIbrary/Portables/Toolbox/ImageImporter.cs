/* Copyright (C) KiteLion Games, LLC - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * 
 * Written by Eliot Carney-Seim <support@kiteliongames.com>, January 2023
 */

using KiteLionGames.BetterDebug;
using KiteLionGames.Common;
using KiteLionGames.Legal;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

//NOTICE A: REMOVAL OR MODIFICATION OF THE LINES ABOVE 'NOTICE B' VOIDS ANY AND ALL RESPONSIBLITY AND SUPPORT OF THIS SOFTWARE BY KITELION GAMES, LLC AND IT'S PARTNERS.
namespace KiteLionGames.Utilities.ImageImporter
{
    [Flags]
    public enum ImportOptions
    {
        None = 0,
        ScaleToFit = 1,
        ScaleToFill = 2,
        Crop = 4,
        BlitOntoTemplate = 8,
    }

    public class ImageImporter : ILegal
    {


        public string KiteLionGamesSoftwareName { get => typeof(ImageImporter).Name; }
        public string URI { get; private set; }
        public byte[] ResultImageBytes { get; private set; }
        public float? Progress { get { return _webRequest == null ? -1f : _webRequest.downloadProgress; } } //todo change when make local async
                                                                                                            //todo properly implement downloadProgress https://docs.unity3d.com/ScriptReference/Networking.DownloadHandler.html
        public ImportOptions Options { get => _importOptions; set => _importOptions = value; }
        public bool Complete { get => _isFinishedLoading; }

        private static readonly string _testURLImage = "https://upload.wikimedia.org/wikipedia/en/2/27/Bliss_%28Windows_XP%29.png";
        private static readonly int _minPNGSize = 119; //bytes. 119 is the size of the smallest png I could find on my computer. I'm using this to check if the png is empty or not.
        private bool _isWeb;
        private bool _isLocal;
        private bool _isFinishedLoading;
        private Action<byte[]> _onFinishedLoading { get; set; }
        private UnityWebRequest _webRequest;
        private Material _targetToBlitTo;
        private ImportOptions _importOptions = ImportOptions.None;

        public Action<byte[]> OnFinishedLoading
        {
            get => _onFinishedLoading;
            set
            {
                _onFinishedLoading += value;
                if (_isFinishedLoading)
                {
                    _onFinishedLoading.Invoke(ResultImageBytes);
                }
            }
        }

        public ImageImporter(string uri, ImportOptions options = ImportOptions.None, Material targetToBlitTo = null)
        {
            Options = options;
            _targetToBlitTo = targetToBlitTo;
            Copyright.RecordUsage(this);
            string s = Tools.RemoveWhitespace(uri)[^4..];
            if (s != ".png")
            {
                CBUG.Error("Only PNGs are supported");
                return;
            }
            URI = uri;
            _isWeb = uri.Contains("http");
            _isLocal = new Uri(URI).IsFile;
        }

        /// <summary>
        /// Uses given uri to GET an image and apply it to the material.
        /// URI can be local or web.
        /// 
        /// PNG ONLY. Thanks
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public ImageImporter ImportAsync()
        {
            CBUG.Do("GETting texture ...");
            if (_isWeb)
            {
                DownloadHelper(URI);
            }
            else if (_isLocal)
            {
                CBUG.Do("Loading texture from local file ...");
                byte[] bytes = File.ReadAllBytes(URI); //todo make this async
                if (PassesByteSanityTestHelper(bytes))
                {
                    ResultImageBytes = bytes;
                    _isFinishedLoading = true;
                    OnFinishedLoading?.Invoke(ResultImageBytes);
                }
                else
                {
                    CBUG.Error("Saved texture png failed sanity test, corrupted?");
                }
            }
            else
            {
                CBUG.Error("URI is not a web or local file");
            }
            return this;
        }

        /// <summary>
        /// Downloads the image from the test url and applies it to the material.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="targetMaterial"></param>
        /// <returns></returns>
        private void DownloadHelper(string uri)
        {
            _webRequest = UnityWebRequestTexture.GetTexture(uri);
            _webRequest.SendWebRequest().completed += OnDownloadCompleteHelper;
        }

        private void OnDownloadCompleteHelper(AsyncOperation result)
        {
            bool failed = true;
            if (_webRequest.result == UnityWebRequest.Result.Success)
            {
                if (PassesByteSanityTestHelper(_webRequest.downloadHandler.data))
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(_webRequest);
                    if (texture != null)
                    {
                        byte[] textureData = new byte[_webRequest.downloadHandler.data.Length];
                        Array.Copy(_webRequest.downloadHandler.data, textureData, _webRequest.downloadHandler.data.Length);

                        ResultImageBytes = textureData;
                        failed = false;
                        _isFinishedLoading = true;
                        if ((_importOptions & ImportOptions.None) == ImportOptions.None)
                        {
                            OnFinishedLoading?.Invoke(ResultImageBytes);
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        CBUG.Error("Failed to download and convert image to texture.");
                    }
                }
                else
                {
                    CBUG.Error("Failed to download image: PNG failed sanity test.");
                }
            }
            else
            {
                CBUG.Error("Failed to download image: " + _webRequest.error);
            }
            if (failed)
            {
                CBUG.Error("Attempting sanity check, passing default evergreen PNG url ...");
                DownloadHelper(_testURLImage);
            }
            _webRequest.Dispose();
        }

        /// <summary>
        /// Validates the byte array is big enough to be a PNG.
        /// </summary>
        /// <param name="bytes">the serialized png.</param>
        /// <returns>True if passes.</returns>
        private static bool PassesByteSanityTestHelper(byte[] bytes)
        {
            if (bytes.Length < _minPNGSize)
            {
                CBUG.Error("PNG is too small to be valid");
                return false;
            }
            return true;
        }

        private void ApplyImageToMaterialHelper()
        {
            if (_targetToBlitTo == null)
            {
                CBUG.Error("No material to blit to");
                return;
            }
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(ResultImageBytes);
            _targetToBlitTo.mainTexture = texture;
        }
    }
}
//NOTICE B: REMOVAL OR MODIFICATION OF THE LINES BELOW 'NOTICE A' VOIDS ANY AND ALL RESPONSIBLITY AND SUPPORT OF THIS SOFTWARE BY KITELION GAMES, LLC AND IT'S PARTNERS.
