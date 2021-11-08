using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TakeScreenshot : MonoBehaviour
{
    [SerializeField]
    Camera m_camera = null;
    [SerializeField]
    Transform m_pageArea = null;//210x297
    [SerializeField]
    RenderTexture m_targetRT = null;

    [SerializeField]
    float m_px_per_mm = 11.8f;

    string m_targetDir = "";
    string m_pageHeaderDate = "";

    private void Awake()
    {
        m_camera.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Texture2D RtToTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }

    public void SetTargetDir(string path, string pageHeaderDate)
    {
        m_targetDir = path;
        m_pageHeaderDate = pageHeaderDate;
    }

    public int PngFileCount(string path)
    {
        DirectoryInfo dir = new DirectoryInfo(path);
        int i = 0;
        // Add file sizes.
        FileInfo[] fis = dir.GetFiles();
        foreach (FileInfo fi in fis)
        {
            if (fi.Extension.Contains("png"))
                i++;
        }
        return i;
    }

    // Take a shot immediately
    public void TakeScreenshotNow()
    {
        Vector3 newPos = m_pageArea.GetChild(0).position;
        newPos.z = m_camera.transform.position.z;
        m_camera.transform.position = newPos;

        int wid = (int)(m_px_per_mm * m_pageArea.localScale.x);
        int hei = (int)(m_px_per_mm * m_pageArea.localScale.y);
        m_targetRT = new RenderTexture(wid, hei, 16);
        m_camera.targetTexture = m_targetRT;
        m_camera.Render();

        byte[] bytes = RtToTexture2D(m_targetRT).EncodeToPNG();
        if (!m_targetDir.EndsWith("/"))
            m_targetDir = m_targetDir + "/";

        int pngFilesAreadyThere = PngFileCount(m_targetDir);
        m_pageHeaderDate = m_pageHeaderDate.Replace(":", "-");
        string finalPath = m_targetDir + m_pageHeaderDate + "_" + pngFilesAreadyThere + ".png";
        Debug.Log("Saving file: "+ finalPath);
        System.IO.File.WriteAllBytes(finalPath, bytes);


        //System.IO.File.WriteAllBytes(Application.dataPath + "/../SavedScreen.png", bytes);

        //var form = new WWWForm();
        //form.AddField("plant", plantComponentID);
        //form.AddBinaryData("image", bytes, "screenShot.png", "image/png");
    }


}
