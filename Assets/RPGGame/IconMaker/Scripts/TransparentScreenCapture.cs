using UnityEngine;
using System.Collections;
using System.IO;

public class TransparentScreenCapture : MonoBehaviour
{
    public Camera targetCamera;
    void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }
    private IEnumerator _CaptureWithApplicationCaptureScreenShot(string filePath)
    {
        // Define filename for an black and white background textures and its path
        string blkFilePath = filePath + ".blk";
        string whtFilePath = filePath + ".wht";
        // Reference for transparent texture
        Texture2D returnTexture = null;
        // Keep temporary Camera variables
        var preClearFlags = targetCamera.clearFlags;
        var preBackgroundColor = targetCamera.backgroundColor;
        // Change clearFlags to Color because it's required color of background to find transparent pixels
        targetCamera.clearFlags = CameraClearFlags.Color;

        // Capture screen shots to create icon textures that have black and white background

        //// Create black background icon texture
        ////// WaitForEndOfFrame to capture from targetCamera's new frame which its background is black
        yield return new WaitForEndOfFrame();
        ////// Force the camera to render before call Application.CaptureScreenshot() to Capture screen shots
        CaptureScreenWithCameraColor(Color.black, blkFilePath);
        ////// Wait until screen shots texture already saved
        while (!File.Exists(blkFilePath))
        {
            yield return 0;
        }

        //// Create white background icon texture
        ////// WaitForEndOfFrame to capture from targetCamera's new frame which its background is white
        yield return new WaitForEndOfFrame();
        ////// Force the camera to render before call Application.CaptureScreenshot() to Capture screen shots
        CaptureScreenWithCameraColor(Color.white, whtFilePath);
        ////// Wait until screen shots texture already saved
        while (!File.Exists(whtFilePath))
        {
            yield return 0;
        }

        // Load an textures to compare difference color (black/white) and define as transparent pixels
        var blackBackgroundCapture = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
        blackBackgroundCapture.LoadImage(File.ReadAllBytes(blkFilePath));
        var whiteBackgroundCapture = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
        whiteBackgroundCapture.LoadImage(File.ReadAllBytes(whtFilePath));

        // Loop x,y pixels positions, compare and set difference color pixels to transparent pixels
        for (int x = 0; x < whiteBackgroundCapture.width; ++x)
        {
            for (int y = 0; y < whiteBackgroundCapture.height; ++y)
            {
                Color colorWhenBlack = blackBackgroundCapture.GetPixel(x, y);
                Color colorWhenWhite = whiteBackgroundCapture.GetPixel(x, y);
                if (colorWhenBlack != Color.clear)
                {
                    //// Turn different color pixels to transparent pixels
                    whiteBackgroundCapture.SetPixel(x, y, GetColor(colorWhenBlack, colorWhenWhite));
                }
            }
        }

        // Apply changes to any texture
        whiteBackgroundCapture.Apply();
        returnTexture = whiteBackgroundCapture;

        // Deletes temporary files
        Object.DestroyImmediate(blackBackgroundCapture);
        File.Delete(blkFilePath);
        File.Delete(whtFilePath);

        // Turn back camera variables
        targetCamera.backgroundColor = preBackgroundColor;
        targetCamera.clearFlags = preClearFlags;

        // Save new screen shot
        try
        {
            using (var file = new FileStream(filePath, FileMode.Create))
            {
                BinaryWriter writer = new BinaryWriter(file);
                writer.Write(returnTexture.EncodeToPNG());
            }
        }
        finally
        {
            Object.DestroyImmediate(returnTexture);
        }
    }

    private void CaptureScreenWithCameraColor(Color color, string fileName)
    {
        // Change background color before render and capture screen shot
        targetCamera.backgroundColor = color;
        targetCamera.Render();
        ScreenCapture.CaptureScreenshot(fileName);
    }

    private Color GetColor(Color pColorWhenBlack, Color pColorWhenWhite)
    {
        // Find Alpha, Can use only r variable
        // When r{white} = 0, r{black} = 1 
        // If colors are difference the result is 1 + (r{black} => 0) - (r{white} => 1) = 1 - 1 = alpha = 0
        // If colors are not difference the result is r{black} = r{white} = 1 + (r{black} => x) - (r{white} => x) = 1 + 0 = alpha = 1
        float alpha = 1 + pColorWhenBlack.r - pColorWhenWhite.r;
        return new Color(
            pColorWhenBlack.r,
            pColorWhenBlack.g,
            pColorWhenBlack.b,
            alpha);
    }

    public Coroutine CaptureScreenshot(string fileName)
    {
        return StartCoroutine(_CaptureWithApplicationCaptureScreenShot(fileName));
    }
}