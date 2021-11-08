using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;


public class LayoutData : MonoBehaviour
{
    

    public QRSystem qrSystem = null;
    

    public Transform qrContainer = null;
    public Transform paper = null;

    public TMP_Text pageHeaderDate = null;


    Vector3 qrContStartLocalPos;
    Vector3 paperStartScale;

    string m_defaultQRLoadPath = "C:/QR_SOURCE";
    string m_defaultQRSavePath = "C:/QR_Output";

    Vector2 default_qrContPos = new Vector2(35, 25);

    Vector2 default_qrDistanceRightDown = new Vector2(35, 35);
    Vector2 default_qrPos = Vector2.zero;
    Vector2 default_qrScale = new Vector2(12, 12);//30, 30
    Vector2 default_qrTextPos = new Vector2(0, -15.5f);
    Vector2 default_qrTextAreaSize = new Vector2(1f,0.2f);
    float default_qrFontSize = 1.5f;
    Vector2 default_qrNumber = new Vector2(5, 8);

    Vector2 qrDistanceRightDown = new Vector2(35, 35);
    Vector2 qrPos = Vector2.zero;
    Vector2 qrScale = new Vector2(12,12);//30, 30
    Vector2 qrTextPos = new Vector2(0, -15.5f);
    Vector2 qrTextAreaSize = new Vector2(1f, 0.2f);
    float qrFontSize = 1.5f;
    Vector2 qrNumber = new Vector2(5, 8);
    string qrGroupName = System.DateTime.Now.ToString("yyyy-MM-dd");

    [SerializeField]
    QR_LayoutObjectScript m_demoObject = null;


    [SerializeField]
    TakeScreenshot m_takeScreenshot;

    // Start is called before the first frame update
    void Start()
    {
        pageHeaderDate.text = System.DateTime.Now.ToString("yyyy-MM-dd_hh:mm:ss");

        qrContStartLocalPos = qrContainer.transform.localPosition;
        paperStartScale = paper.localScale;

        qrSystem.ArrangeQRsLayout(qrNumber, qrDistanceRightDown, qrPos, qrScale, qrTextPos, qrTextAreaSize, qrFontSize, qrGroupName, m_demoObject);
        //ArrangeQRsLayout(new Vector2(5,8), new Vector2(35,35), Vector2.zero, 30, new Vector2(0, -0.5f), qrGroupName);
        //ArrangeQRsLayout(Vector2 qrNumber, Vector2 qrDistanceRightDown, Vector2 qrPos, Vector2 qrScale, Vector2 qrTextPos, string qrGroupName)
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetQRLayoutData()
    {
        qrDistanceRightDown = default_qrDistanceRightDown;
        qrPos = default_qrPos;
        qrScale = default_qrScale;
        qrTextPos = default_qrTextPos;
        qrTextAreaSize = default_qrTextAreaSize;
        qrFontSize = default_qrFontSize;

        qrSystem.ArrangeQRsLayout(qrNumber, qrDistanceRightDown, qrPos, qrScale, qrTextPos, qrTextAreaSize, qrFontSize, qrGroupName, m_demoObject);
    }

    //page top left to right
    public void SetValue_PTL2R(TMP_InputField tmpif)
    {
        Vector3 qrContPos = qrContainer.transform.localPosition;
        float result = 0;
        bool success = float.TryParse(tmpif.text, out result);
        if (tmpif.text == "-" || tmpif.text == "+")
        {
            return;
        }
        else
        if (!success)
        {
            tmpif.text = "";
            result = default_qrContPos.x;
        }

        qrContPos.x = /*qrContStartPos.x*/ result;
        qrContainer.transform.localPosition = qrContPos;
        
    }

    //page top left to down
    public void SetValue_PTL2D(TMP_InputField tmpif)
    {
        Vector3 qrContPos = qrContainer.transform.localPosition;
        float result = 0;
        bool success = float.TryParse(tmpif.text, out result);
        if (tmpif.text == "-" || tmpif.text == "+")
        {
            return;
        }
        else
        if (!success)
        {
            tmpif.text = "";
            result = default_qrContPos.y;
        }

        qrContPos.y = /*qrContStartPos.y*/ - result;
        qrContainer.transform.localPosition = qrContPos;
        
    }

    public void SetValue_PageWidth(TMP_InputField tmpif)
    {
        Vector3 paperScale = paper.localScale;
        float result = 0;
        bool success = float.TryParse(tmpif.text, out result);
        if (tmpif.text == "-" || tmpif.text == "+")
        {
            return;
        }
        else
        if (!success)
        {
            tmpif.text = "";
            result = paperScale.x;
        }

        paperScale.x = result;
        paper.localScale = paperScale;
        
    }

    public void SetValue_PageHeight(TMP_InputField tmpif)
    {
        Vector3 paperScale = paper.localScale;
        float result = 0;
        bool success = float.TryParse(tmpif.text, out result);
        if (tmpif.text == "-" || tmpif.text == "+")
        {
            return;
        }
        else
        if (!success)
        {
            tmpif.text = "";
            result = paperScale.y;
        }

        paperScale.y = result;
        paper.localScale = paperScale;

    }

    public void SetValue_QRDistanceToNextRight(TMP_InputField tmpif)
    {
        float result = 0;
        bool success = float.TryParse(tmpif.text, out result);
        if (tmpif.text == "-" || tmpif.text == "+")
        {
            return;
        }
        else
        if (!success)
        {
            tmpif.text = "";
            result = default_qrDistanceRightDown.x;
        }

        qrDistanceRightDown.x = result;
        qrSystem.ArrangeQRsLayout(qrNumber, qrDistanceRightDown, qrPos, qrScale, qrTextPos, qrTextAreaSize, qrFontSize, qrGroupName, m_demoObject);
        
    }

    public void SetValue_QRDistanceToNextDown(TMP_InputField tmpif)
    {
        float result = 0;
        bool success = float.TryParse(tmpif.text, out result);
        if (tmpif.text == "-" || tmpif.text == "+")
        {
            return;
        }
        else
        if (!success)
        {
            tmpif.text = "";
            result = default_qrDistanceRightDown.y;
        }

        qrDistanceRightDown.y = result;
        qrSystem.ArrangeQRsLayout(qrNumber, qrDistanceRightDown, qrPos, qrScale, qrTextPos, qrTextAreaSize, qrFontSize, qrGroupName, m_demoObject);
        
    }

    public void SetValue_QRScaleX(TMP_InputField tmpif)
    {
        float result = 0;
        bool success = float.TryParse(tmpif.text, out result);
        if (tmpif.text == "-" || tmpif.text == "+")
        {
            return;
        }
        else
        if (!success)
        {
            tmpif.text = "";
            result = default_qrScale.x;
        }

        qrScale.x = result;
        qrSystem.ArrangeQRsLayout(qrNumber, qrDistanceRightDown, qrPos, qrScale, qrTextPos, qrTextAreaSize, qrFontSize, qrGroupName, m_demoObject);
        
    }

    public void SetValue_QRScaleY(TMP_InputField tmpif)
    {
        float result = 0;
        bool success = float.TryParse(tmpif.text, out result);
        if (tmpif.text == "-" || tmpif.text == "+")
        {
            return;
        }
        else
        if (!success)
        {
            tmpif.text = "";
            result = default_qrScale.y;
        }

        qrScale.y = result;
        qrSystem.ArrangeQRsLayout(qrNumber, qrDistanceRightDown, qrPos, qrScale, qrTextPos, qrTextAreaSize, qrFontSize, qrGroupName, m_demoObject);
        
    }


    public void SetValue_QRPosX(TMP_InputField tmpif)
    {
        float result = 0;
        bool success = float.TryParse(tmpif.text, out result);
        if (tmpif.text == "-" || tmpif.text == "+")
        {
            return;
        }
        else
        if (!success)
        {
            tmpif.text = "";
            result = default_qrPos.x;
        }

        qrPos.x = result;
        qrSystem.ArrangeQRsLayout(qrNumber, qrDistanceRightDown, qrPos, qrScale, qrTextPos, qrTextAreaSize, qrFontSize, qrGroupName, m_demoObject);
        
    }

    public void SetValue_QRPosY(TMP_InputField tmpif)
    {
        float result = 0;
        bool success = float.TryParse(tmpif.text, out result);
        if (tmpif.text == "-" || tmpif.text == "+")
        {
            return;
        }
        else
        if(!success)
        {
            tmpif.text = "";
            result = default_qrPos.y;
        }

        qrPos.y = result;
        qrSystem.ArrangeQRsLayout(qrNumber, qrDistanceRightDown, qrPos, qrScale, qrTextPos, qrTextAreaSize, qrFontSize, qrGroupName, m_demoObject);
        
    }


    public void SetValue_QRTextPosX(TMP_InputField tmpif)
    {
        float result = 0;
        bool success = float.TryParse(tmpif.text, out result);
        if (tmpif.text == "-" || tmpif.text == "+")
        {
            return;
        }
        else
        if (!success)
        {
            tmpif.text = "";
            result = default_qrTextPos.x;
        }

        qrTextPos.x = result;
        qrSystem.ArrangeQRsLayout(qrNumber, qrDistanceRightDown, qrPos, qrScale, qrTextPos, qrTextAreaSize, qrFontSize, qrGroupName, m_demoObject);
        
    }

    public void SetValue_QRTextPosY(TMP_InputField tmpif)
    {
        float result = 0;
        bool success = float.TryParse(tmpif.text, out result);
        if (tmpif.text == "-" || tmpif.text == "+")
        {
            return;
        }
        else
        if (!success)
        {
            tmpif.text = "";
            result = default_qrTextPos.y;
        }
        qrTextPos.y = result;
        qrSystem.ArrangeQRsLayout(qrNumber, qrDistanceRightDown, qrPos, qrScale, qrTextPos, qrTextAreaSize, qrFontSize, qrGroupName, m_demoObject);
        

    }

    public void SetValue_QRTextAreaSizeX(TMP_InputField tmpif)
    {
        float result = 0;
        bool success = float.TryParse(tmpif.text, out result);
        if (tmpif.text == "-" || tmpif.text == "+")
        {
            return;
        }
        else
        if (!success)
        {
            tmpif.text = "";
            result = default_qrTextAreaSize.x;
        }
        qrTextAreaSize.x = result;
        qrSystem.ArrangeQRsLayout(qrNumber, qrDistanceRightDown, qrPos, qrScale, qrTextPos, qrTextAreaSize, qrFontSize, qrGroupName, m_demoObject);

    }

    public void SetValue_QRTextAreaSizeY(TMP_InputField tmpif)
    {
        float result = 0;
        bool success = float.TryParse(tmpif.text, out result);
        if (tmpif.text == "-" || tmpif.text == "+")
        {
            return;
        }
        else
        if (!success)
        {
            tmpif.text = "";
            result = default_qrTextAreaSize.y;
        }
        qrTextAreaSize.y = result;
        qrSystem.ArrangeQRsLayout(qrNumber, qrDistanceRightDown, qrPos, qrScale, qrTextPos, qrTextAreaSize, qrFontSize, qrGroupName, m_demoObject);

    }

    public void SetValue_QRFontSize(TMP_InputField tmpif)
    {
        float result = 0;
        bool success = float.TryParse(tmpif.text, out result);
        if (tmpif.text == "-" || tmpif.text == "+")
        {
            return;
        }
        else
        if (!success)
        {
            tmpif.text = "";
            result = default_qrFontSize;
        }
        qrFontSize = result;
        qrSystem.ArrangeQRsLayout(qrNumber, qrDistanceRightDown, qrPos, qrScale, qrTextPos, qrTextAreaSize, qrFontSize, qrGroupName, m_demoObject);

    }
    

    public void SetValue_QRNo_x(TMP_InputField tmpif)
    {
        float result = 0;
        bool success = float.TryParse(tmpif.text, out result);
        if (tmpif.text == "-" || tmpif.text == "+")
        {
            return;
        }
        else
        if (!success)
        {
            tmpif.text = "";
            result = default_qrNumber.x;
        }

        qrNumber.x = result;
        //qrContainer
        //qrPrefab
        qrSystem.ArrangeQRsLayout(qrNumber, qrDistanceRightDown, qrPos, qrScale, qrTextPos, qrTextAreaSize, qrFontSize, qrGroupName, m_demoObject);
    }

    public void SetValue_QRNo_y(TMP_InputField tmpif)
    {
        float result = 0;
        bool success = float.TryParse(tmpif.text, out result);
        if (tmpif.text == "-" || tmpif.text == "+")
        {
            return;
        } else
        if (!success)
        {
            tmpif.text = "";
            result = default_qrNumber.y;
        }


        qrNumber.y = result;
        qrSystem.ArrangeQRsLayout(qrNumber, qrDistanceRightDown, qrPos, qrScale, qrTextPos, qrTextAreaSize, qrFontSize, qrGroupName, m_demoObject);
        
    }

    public void SetValue_qrGroupName(TMP_InputField tmpif)
    {
        qrGroupName = tmpif.text;
        qrSystem.ArrangeQRsLayout(qrNumber, qrDistanceRightDown, qrPos, qrScale, qrTextPos, qrTextAreaSize, qrFontSize, qrGroupName, m_demoObject);
    }

    public void LoadFromQRCodeFile(TMP_InputField tmpif)
    {
        string path = tmpif.text;
        path = path.Replace("\\", "/");
        if (path == "")
            path = m_defaultQRLoadPath;
        //TODO: print alert if path is wrong

        if(Directory.Exists(path))
            qrSystem.BuildQRSourceDatabaseFromFiles((int)(qrNumber.x* qrNumber.y), path);
    }

    public void TakeScreenshot(TMP_InputField tmpif)
    {

        string path = tmpif.text;
        path = path.Replace("\\", "/");
        if (path == "")
            path = m_defaultQRSavePath;

        //also remember what you printed and continue from there

        if (!Directory.Exists(path))
        {
       
            Directory.CreateDirectory(path);

        }

        m_takeScreenshot.SetTargetDir(path, pageHeaderDate.text);
        m_takeScreenshot.TakeScreenshotNow();
    }
}
