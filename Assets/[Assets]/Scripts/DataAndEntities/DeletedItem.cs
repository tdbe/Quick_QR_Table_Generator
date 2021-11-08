using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeletedItem : MonoBehaviour
{
    public int index = -1;
    public string groupName = "";
    public TMP_Text myLabel = null;
    QRSystem.QR_SourceObject m_qrSourceObject = null;


    void OnEnable()
    {
        
    }

    public void Init(QRSystem.QR_SourceObject qrSourceObject)
    {
        m_qrSourceObject = qrSourceObject;
        index = m_qrSourceObject.qrCodeIndexInGroup;
        groupName = m_qrSourceObject.qrGroupID;
        RefreshMyLabel();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void RefreshMyLabel()
    {
        myLabel.text = index + "_" + groupName;
    }

    public void RequestUndeleteSelf()
    {
        bool success = QRSystem._instance.QRSourceOperation_ProcessRequestToUnDiscardCode(m_qrSourceObject);
        if (success)
            Destroy(gameObject);
    }
}
