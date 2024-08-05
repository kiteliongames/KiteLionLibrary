using KiteLionGames.BetterDebug;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class WebGLDownload : MonoBehaviour
{
    public enum ImageFormat
    {
        jpg,
        png
    }
    private bool _isRecording = false;
    [DllImport("__Internal")]
    private static extern void DownloadFileJsLib(byte[] byteArray, int byteLength, string fileName);
    [DllImport("__Internal")]
    private static extern void DownloadFileByPathJsLib(string base64, string extension);

    /// <summary>
    /// ___
    /// <para>bytes -> The bytes to be downloaded</para>
    /// <para>fileName -> The downloaded file name (without extension)</para>
    /// <para>fileExtension -> WebGLDownload.FileExtension.jpg/png/zip/</para>
    /// </summary>
    public void DownloadFile(byte[] bytes, string fileName, string fileExtension)
    {
        if (fileName == "") fileName = "UnnamedFile";
#if UNITY_EDITOR
        string path = UnityEditor.EditorUtility.SaveFilePanel("Save file...", "", fileName, fileExtension);
        System.IO.File.WriteAllBytes(path, bytes);
        CBUG.Log("File saved: " + path);
#elif UNITY_WEBGL
        KiteLionGames.BetterDebug.CBUG.Log("WebGLDownload.DownloadingFile!");
        DownloadFileJsLib(bytes, bytes.Length, fileName + "." + fileExtension);
#endif
    }

    /// <summary>
    /// Download a file by path, this will open the file in the browser.
    /// Only works in WebGL or Editor.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="fileExtension"></param>
    public void DownloadFileByPath(string path, string fileExtension)
    {
        var bytes = System.IO.File.ReadAllBytes(path);
        string base64 = System.Convert.ToBase64String(bytes);
#if UNITY_EDITOR
        // open file
        System.Diagnostics.Process.Start(path);
        // open containing folder
        System.Diagnostics.Process.Start(System.IO.Path.GetDirectoryName(path));
        Debug.Log("File saved: " + path);
#elif UNITY_WEBGL
        KiteLionGames.BetterDebug.CBUG.Log("WebGLDownload.DownloadingFile!");
        DownloadFileByPathJsLib(base64, fileExtension);
#endif
    }


    /// <summary>
    /// ___
    /// <para>imageFormat -> WebGLDownload.ImageFormat.jpg/png</para>
    /// <para>screenshotUpscale -> Upscale the frame. default = 1</para>
    /// <para>fileName -> Optional filename. Empty filename creates a name texture.width x texture.height in pixel + current datetime</para>
    /// </summary>
    public void GetScreenshot(ImageFormat imageFormat, int screenshotUpscale, string fileName = "")
    {
        if (!_isRecording) StartCoroutine(RecordUpscaledFrame(imageFormat, screenshotUpscale, fileName));
    }

    IEnumerator RecordUpscaledFrame(ImageFormat imageFormat, int screenshotUpscale, string fileName)
    {
        _isRecording = true;
        yield return new WaitForEndOfFrame();
        try
        {
            if (fileName == "")
            {
                int resWidth = Camera.main.pixelWidth * screenshotUpscale;
                int resHeight = Camera.main.pixelHeight * screenshotUpscale;
                string dateFormat = "yyyy-MM-dd-HH-mm-ss";
                fileName = resWidth.ToString() + "x" + resHeight.ToString() + "px_" + System.DateTime.Now.ToString(dateFormat);
            }
            Texture2D screenShot = ScreenCapture.CaptureScreenshotAsTexture(screenshotUpscale);
            if (imageFormat == ImageFormat.jpg) DownloadFile(screenShot.EncodeToJPG(), fileName, "jpg");
            else if (imageFormat == ImageFormat.png) DownloadFile(screenShot.EncodeToPNG(), fileName, "png");
            Destroy(screenShot);
        }
        catch (System.Exception e)
        {
            Debug.Log("Original error: " + e.Message);
        }
        _isRecording = false;
    }
}