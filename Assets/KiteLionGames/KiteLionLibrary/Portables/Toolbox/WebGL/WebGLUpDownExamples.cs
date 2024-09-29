#region FileHeader

// test
// Project: Assembly-CSharp
// File:    WebGLUpDownExamples.cs
// Author:  Eliot CS
// Created: 2024.09.18.01.09.15
// Edited: 2024.09.19.01.09.20
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

using UnityEngine;
using UnityEngine.UI;

namespace KiteLionGames.KiteLionLibrary.Portables.Toolbox.WebGL
{
    public class WebGLUpDownExamples : MonoBehaviour
    {
        public static WebGLUpload _webGLUpload;
        public static WebGLDownload _webGLDownload;
        public Image _targetImage;

        private void Awake()
        {
            _webGLUpload = this.GetComponent<WebGLUpload>();
            _webGLDownload = this.GetComponent<WebGLDownload>();
        }

        public void SelectLocalImage()
        {
            if (_targetImage) _webGLUpload.UploadTexture(WebGLUpload.ImageFormat.jpg, 1024, true, null, _targetImage);
            else Debug.LogWarning("target image is null!");
        }

        private void UploadZip()
        {
            //Upload a zip file
            _webGLUpload.UploadFile(WebGLUpload.FileExtension.zip);
        }

        public void UploadFdx()
        {
            //Upload a zip file
            _webGLUpload.UploadFile(WebGLUpload.FileExtension.fdx);
        }

        private void UploadTexture(WebGLUpload.ImageFormat imageFormat)
        {
            //Upload a Texture and don't downsize the image (0)
            _webGLUpload.UploadTexture(imageFormat, 0, true);
        }

        private void UploadTextureToMaterial(WebGLUpload.ImageFormat imageFormat, Material mat)
        {
            //Upload a Texture and set the material texture
            _webGLUpload.UploadTexture(imageFormat, 1024, true, mat);
        }

        private void UploadTextureToImage(WebGLUpload.ImageFormat imageFormat, Image img)
        {
            //Upload a Texture and set the image sprite
            _webGLUpload.UploadTexture(imageFormat, 1024, true, null, img);
        }

        private void DownloadFile(byte[] bytes)
        {
            _webGLDownload.DownloadFile(bytes, "myFilename", "myExtension");
        }

        private void DownloadZip()
        {
            ////Zip example(Zip / gzip Multiplatform Native Plugin from the asset store)
            ////----
            //lzip.inMemory mZip = new lzip.inMemory();
            //string myText = "Some text";
            //byte[] bytes = System.Text.Encoding.ASCII.GetBytes(myText);
            //lzip.compress_Buf2Mem(mZip, 9, bytes, "data/" + "myData.dat", null, null);
            //Texture2D tex = null;
            //lzip.compress_Buf2Mem(mZip, 9, tex.EncodeToJPG(), "tex/" + "texName.jpg", null, null);
            //byte[] bZip = mZip.getZipBuffer();
            //_webGLDownload.DownloadFile(bZip, "myZipFilename", "zip");
            //bZip = null;
            //lzip.free_inmemory(mZip);
        }

        private void DownloadTexture(Texture2D tex, WebGLDownload.ImageFormat imageFormat)
        {
            byte[] texBytes;
            if (imageFormat == WebGLDownload.ImageFormat.png) texBytes = tex.EncodeToPNG();
            else texBytes = tex.EncodeToJPG();
            _webGLDownload.DownloadFile(texBytes, "texFileName", imageFormat.ToString());
        }

        private void DownloadScreenshot()
        {
            _webGLDownload.GetScreenshot(WebGLDownload.ImageFormat.jpg, 1);
        }
    }
}
