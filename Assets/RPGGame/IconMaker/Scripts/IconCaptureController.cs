using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IconCaptureController : MonoBehaviour
{
    public string iconFilename;
    public int startNameCount = 1;
    public List<GameObject> objects;
    public List<GameObject> hiddenObjects;
    public string savePath;
    public TransparentScreenCapture capturer;
    public void Capture()
    {
        StartCoroutine(_Capture());
    }

    IEnumerator _Capture()
    {
        foreach (GameObject hiddingObject in hiddenObjects)
            hiddingObject.gameObject.SetActive(false);

        int count = 0;
        int nameCount = startNameCount;
        int total = objects.Count;
        while (count < total)
        {
            // Hide all models
            foreach (GameObject obj in objects)
            {
                obj.SetActive(false);
            }
            // Show one model to create icon
            objects[count].SetActive(true);
            // Save the Icon
            yield return capturer.CaptureScreenshot(savePath + "/" + iconFilename + "_" + nameCount + ".png");
            count++;
            nameCount++;
        }
        yield return new WaitForEndOfFrame();

        foreach (GameObject showingObj in hiddenObjects)
            showingObj.gameObject.SetActive(true);
    }
}