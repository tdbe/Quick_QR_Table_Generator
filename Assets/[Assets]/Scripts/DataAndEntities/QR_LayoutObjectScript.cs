using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QR_LayoutObjectScript : MonoBehaviour
{
    public TMP_Text qr_GroupName;
    public TMP_Text qr_Number;
    public TMP_Text qr_LabelMerged;
    public TMP_Text qr_Code;

    public GameObject qrSpritePlane;
    public SpriteRenderer qrSpriteRenderer;
    public GameObject qrPlacehohder;
    public GameObject qrText;

    public enum QRLayoutObjectState { Usable = 0, Locked = 1 }
    QRLayoutObjectState m_QRLayoutObjectState = QRLayoutObjectState.Usable;
    public QRLayoutObjectState qrLayoutObjectState { get { return m_QRLayoutObjectState; } }
    
    [SerializeField]
    QRSystem.QR_SourceObject m_qrSrcObj = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUpQRLayout(int number, Vector2 qrPos, Vector2 qrScale, Vector2 qrTextPos, Vector2 qrTextAreaSize, float qrFontSize, string qrGroupName)
    {
        qr_GroupName.text = "Empty Slot ";// qrGroupName;
        qr_Number.text = "#";// "#" + number;
        qrSpritePlane.transform.localPosition = qrPos;
        qrText.transform.localPosition = qrTextPos;
        float radScale = Mathf.Min(qrScale.x, qrScale.y);
        qrSpritePlane.transform.localScale = new Vector2(radScale, radScale);
        qrPlacehohder.transform.localScale = new Vector2(qrScale.x*2.5f, qrScale.y*2.5f);

        qr_LabelMerged.fontSize = qrFontSize;
        RectTransform rt = qr_LabelMerged.GetComponent<RectTransform>();
        rt.sizeDelta = qrTextAreaSize;




    }

    /// <summary>
    /// The QRSystem loops through all these objects and checks if QR_LayoutObjectScript.QRLayoutObjectState.Locked and if not locked it fills it with a QR via SetUpQRCode. SetUpQRCode is also used to reset the texture/names whether or not the slot is locked.
    /// </summary>
    /// <param name="locked"></param>
    public void LockOrUnlockSlot(UnityEngine.UI.Toggle toggle)
    {
        bool locked = toggle.isOn;
        m_QRLayoutObjectState = locked ? QRLayoutObjectState.Locked : QRLayoutObjectState.Usable;
        QRSystem._instance.QRSystemOperation_ProcessRequestToRegenerateQRs();
    }

    /// <summary>
    /// qrSrcObj should be a const reference like in c++ but this is not implemented in c# yet.
    /// // The QRSystem loops through all these objects and checks if QR_LayoutObjectScript.QRLayoutObjectState.Locked and if not locked it fills it with a QR via SetUpQRCode. SetUpQRCode is also used to reset the texture/names whether or not the slot is locked.
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="qrSrcObj"></param>
    public void SetUpQRCode(Texture2D texture, QRSystem.QR_SourceObject qrSrcObj)
    {
        m_qrSrcObj = qrSrcObj;
        //var qrID = qrSrcObj.qrCodeID;
        var number = qrSrcObj.qrCodeIndexInGroup;
        var qrGroupName = qrSrcObj.qrGroupID;
      
        qr_Number.text = "#" + number;
        qr_GroupName.text = qrGroupName;
        qr_LabelMerged.text = qr_Number.text + "_" + qr_GroupName.text;
        if (texture != null)
        {
            Rect rect = new Rect(0, 0, texture.width, texture.height);
            Vector2 pivot = new Vector2(0.5f, 0.5f);
            Sprite qrSprite = Sprite.Create(texture, rect, pivot);
            qrSpriteRenderer.sprite = qrSprite;
        }
        else
        {
            qrSpriteRenderer.sprite = null;
        }
    }

    /// <summary>
    /// the QRSystem will move the qr code source that is currently attributed to this QR_LayoutObject, to a discard list
    /// and then will re-call SetUpQRCode here to replace this code with a new one (the next in queue)
    /// </summary>
    public void QRSourceOperation_RequestDiscardThisCode()
    {
        if (m_qrSrcObj == null || qrSpriteRenderer.sprite == null)
            return;
        QRSystem._instance.QRSourceOperation_ProcessRequestToDiscardCode(m_qrSrcObj);
    }
}
