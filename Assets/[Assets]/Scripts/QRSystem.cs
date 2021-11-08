using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class QRSystem : MonoBehaviour
{
    public static QRSystem _instance = null;//todo: use MonoBehaviorSingleton<QRSystem>

    public LayoutData layoutData;
    public int totalQRObjectPool = 200;//20x10
    public QR_LayoutObjectScript qrPrefab = null;
    [SerializeField]
    QR_LayoutObjectScript[] m_qrLayoutObjectPool = null;

    [SerializeField]
    List<List<QR_SourceObject>> m_QRGroupList_of_QR_SourceObjects = new List<List<QR_SourceObject>>();

    [SerializeField]
    List<QR_SourceObject> m_discardSourceObjectsList = new List<QR_SourceObject>();

    int m_totalQRsOnPage = 0;

    [SerializeField]
    Texture2D tempQRSprite = null;

    [SerializeField] 
    GameObject m_deletedItemListParent = null;

    [SerializeField]
    ReadWriteQR m_zxing_ReadWriteQR = null;

    /// <summary>
    /// Holds info about how and if a qr image should be generated.
    /// The QR_LayoutObject is holds info about how to display it and how to handle user input.
    /// </summary>
    public class QR_SourceObject
    {
        public enum QRSourceObjectState { Usable = 0, Discarded = 1, Printed = 2 }
        public QRSourceObjectState qrSourceObjectState = QRSourceObjectState.Usable;

        public string qrCodeID = "";
        public string qrGroupID = "";
        public int qrCodeIndexInGroup = 0;
    }

    // Start is called before the first frame update
    void Awake()
    {
        _instance = this;// script is at the top of script execution order

        m_qrLayoutObjectPool = new QR_LayoutObjectScript[totalQRObjectPool];
        for(int i = 0; i< totalQRObjectPool; i++)
        {
            m_qrLayoutObjectPool[i] = Instantiate(qrPrefab, layoutData.qrContainer);
            m_qrLayoutObjectPool[i].gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ArrangeQRsLayout(Vector2 qrNumber, Vector2 qrDistanceRightDown, Vector2 qrPos, Vector2 qrScale, Vector2 qrTextPos, Vector2 qrTextAreaSize, float qrFontSize, string qrGroupName, QR_LayoutObjectScript demoObject = null)
    {
        int totalNumber = (int)(qrNumber.x * qrNumber.y);
        if(totalNumber > totalQRObjectPool)
        {
            //ALERT
            totalNumber = totalQRObjectPool;
        }

        for (int i = 0; i < totalQRObjectPool; i++)
        {
            if (i < totalNumber)
            {
                Vector3 pos = Vector3.zero;

                int indexInRow = 0;// ((i / qrDistanceRightDown.x)%1);
                if (i < qrNumber.x)
                    indexInRow = i;
                else
                    indexInRow = i - (int)((Mathf.Floor(i / qrNumber.x))* qrNumber.x);
                pos.x = qrDistanceRightDown.x * (indexInRow);

                int indexInColumn = 0;// ((i / qrDistanceRightDown.x)%1);
                if (i < qrNumber.x)
                    indexInColumn = 0;
                else
                    indexInColumn = (int)(Mathf.Floor(i / qrNumber.x));
                pos.y = -qrDistanceRightDown.y * (indexInColumn);

                m_qrLayoutObjectPool[i].transform.localPosition = pos;
                m_qrLayoutObjectPool[i].SetUpQRLayout(i, qrPos, qrScale, qrTextPos, qrTextAreaSize, qrFontSize, qrGroupName);
                

                m_qrLayoutObjectPool[i].gameObject.SetActive(true);
            }
            else
            {
                m_qrLayoutObjectPool[i].gameObject.SetActive(false);
            }
        }


        //Demo QR:
        if (demoObject != null)
        {

            demoObject.SetUpQRLayout(-1, qrPos, qrScale, qrTextPos, qrTextAreaSize, qrFontSize, qrGroupName);

            demoObject.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// returns list of QR_SourceObjects for the current file
    /// </summary>
    List<QR_SourceObject> ParseQRFile(FileInfo file)
    {
        List<QR_SourceObject> qrList = new List<QR_SourceObject>();
        string qrGroupId = "";
        FileStream fs = file.OpenRead();
        StreamReader sr = new StreamReader(fs);
        string line = "";
        int lineIndex = 0;

        System.Action parseMethod = () =>
        {
            bool headerMode = true;
            bool bodyMode = false;
            while ((line = sr.ReadLine()) != null)
            {
                if (headerMode)
                {
                    int headIdx = line.LastIndexOf("id: ");
                    if (headIdx >= 0)
                    {
                        string lineFromIdStart = line.Substring(headIdx + 4);
                        qrGroupId = lineFromIdStart.Substring(0, lineFromIdStart.IndexOf(")"));
                        Debug.Log("[ParseMethod] found qrGroupID: " + qrGroupId + "; in line: " + line);
                        headerMode = false;
                    }
                }
                else if (!bodyMode)
                {
                    if (line.Contains("----"))
                    {
                        bodyMode = true;
                        lineIndex = -1;
                    }
                }
                else
                {
                    string qrCodeID = line.Substring(0, line.IndexOf(" <-"));
                    Debug.Log("[ParseMethod] read qrCodeID on qr index: " + lineIndex + "; qrCodeID: " + qrCodeID);
                    QR_SourceObject so = new QR_SourceObject();
                    so.qrCodeID = qrCodeID;
                    so.qrGroupID = qrGroupId;
                    so.qrCodeIndexInGroup = lineIndex;
                    so.qrSourceObjectState = QR_SourceObject.QRSourceObjectState.Usable;
                    qrList.Add(so);
                }
                lineIndex++;
            }
        };
        parseMethod();

        return qrList;
    }

    public void BuildQRSourceDatabaseFromFiles(int totalQRsOnPage, string dirPath)
    {
        m_totalQRsOnPage = totalQRsOnPage;
        m_QRGroupList_of_QR_SourceObjects = new List<List<QR_SourceObject>>();

        DirectoryInfo dir = new DirectoryInfo(dirPath);
        FileInfo[] info = dir.GetFiles("*.txt");
        foreach (FileInfo file in info)
        {
            m_QRGroupList_of_QR_SourceObjects.Add(ParseQRFile(file));
        }

        GenerateQRImagesFromDB_andAssignToLayoutObjects();
    }

    void ClearLayoutObjectsOfPreviousQRData()
    {
        QR_SourceObject cleanSO = new QR_SourceObject();
        for(int i=0; i< m_qrLayoutObjectPool.Length; i++)
        {
            m_qrLayoutObjectPool[i].SetUpQRCode(null, cleanSO);
            m_qrLayoutObjectPool[i].gameObject.SetActive(false); 
        }

    }

    public void QRSystemOperation_ProcessRequestToRegenerateQRs()
    {
        if (m_QRGroupList_of_QR_SourceObjects == null || m_QRGroupList_of_QR_SourceObjects.Count == 0)
            return;
        GenerateQRImagesFromDB_andAssignToLayoutObjects();
    }

    void GenerateQRImagesFromDB_andAssignToLayoutObjects()
    {
        
        //string qrGroupName = "qrGroup id from file";

        ClearLayoutObjectsOfPreviousQRData();
        int skipAheadDiscarded = 0;
        int skippedFromLockedSlots = 0;
        for (int i = 0; i < m_totalQRsOnPage; i++)
        {
            //m_qrLayoutObjectPool[i].gameObject.SetActive(true);
            int i_wSrcSkip = i + skipAheadDiscarded;
            if (m_qrLayoutObjectPool[i].qrLayoutObjectState == QR_LayoutObjectScript.QRLayoutObjectState.Locked)
            {
                m_qrLayoutObjectPool[i].gameObject.SetActive(true);
                skippedFromLockedSlots++;
                continue;
            }
            //int i_wSrcSkip_Layout = i_wSrcSkip;
            i_wSrcSkip = i_wSrcSkip - skippedFromLockedSlots;

            int temp_total_qnos_in_groups = 0;
            int targetGroup = 0;
            for(int g = 0; g< m_QRGroupList_of_QR_SourceObjects.Count; g++)
            {
                temp_total_qnos_in_groups += m_QRGroupList_of_QR_SourceObjects[g].Count;
                //if i is within this group or these groups so far
                if (i_wSrcSkip< temp_total_qnos_in_groups)
                {
                    targetGroup = g;
                    break;
                }
            }
            if (i_wSrcSkip >= temp_total_qnos_in_groups)
            {
                break;
            }

            int indexInCurrentGroup = (i_wSrcSkip - (temp_total_qnos_in_groups - m_QRGroupList_of_QR_SourceObjects[targetGroup].Count));
            QR_SourceObject qrSrcObj = m_QRGroupList_of_QR_SourceObjects[targetGroup][indexInCurrentGroup];
            
            // If QRSourceObjectState is Discarded, don't generate image for this one; move to next qr in the file.
            while (qrSrcObj.qrSourceObjectState == QR_SourceObject.QRSourceObjectState.Discarded)
            {
                
                indexInCurrentGroup++;
                
                if (indexInCurrentGroup >= m_QRGroupList_of_QR_SourceObjects[targetGroup].Count)
                {
                    indexInCurrentGroup = 0;//<<<<<<<<<<<<<<<<-------------------------------------
                    targetGroup++;
                    if(targetGroup>= m_QRGroupList_of_QR_SourceObjects.Count)
                    {
                        break;
                    }
                }
                qrSrcObj = m_QRGroupList_of_QR_SourceObjects[targetGroup][indexInCurrentGroup];

                skipAheadDiscarded++;
                
            }


            //get qr texture
            Texture2D qrTex = m_zxing_ReadWriteQR.GenerateQR(qrSrcObj.qrCodeID);


            m_qrLayoutObjectPool[i].SetUpQRCode(qrTex, qrSrcObj);//i_wSrcSkip_Layout
            m_qrLayoutObjectPool[i].gameObject.SetActive(true);//i_wSrcSkip_Layout
        }
        //qrObjectPool[i].SetUpQRCode(Texture2D texture, int number, string qrGroupName)
    }

    

    public bool QRSourceOperation_ProcessRequestToDiscardCode(QR_SourceObject qrSrcObj)
    {
        bool success = false;
        // move code to discard list.
        qrSrcObj.qrSourceObjectState = QR_SourceObject.QRSourceObjectState.Discarded;
        m_discardSourceObjectsList.Add(qrSrcObj);

        AddToDeletedItemsUI(qrSrcObj);

        // rebuild all qrs but keep the existing database
        GenerateQRImagesFromDB_andAssignToLayoutObjects();
        success = true;
        return success;
    }

    public bool QRSourceOperation_ProcessRequestToUnDiscardCode(QR_SourceObject qrSrcObj)
    {
        bool success = false;
        // move code to discard list.
        qrSrcObj.qrSourceObjectState = QR_SourceObject.QRSourceObjectState.Usable;
        m_discardSourceObjectsList.Remove(qrSrcObj);
    
        // rebuild all qrs but keep the existing database
        GenerateQRImagesFromDB_andAssignToLayoutObjects();
        success = true;
        return success;
    }


    void AddToDeletedItemsUI(QR_SourceObject qrSrcObj)
    {
        
        GameObject src = m_deletedItemListParent.transform.GetChild(0).gameObject;
        GameObject newDeletedItemGO = Instantiate(src);
        //newDeletedItemGO.transform.SetParent(m_deletedItemListParent.transform);
        newDeletedItemGO.transform.parent = m_deletedItemListParent.transform;
        newDeletedItemGO.transform.localScale = Vector3.one;
        newDeletedItemGO.SetActive(true);
        DeletedItem newDelItem = newDeletedItemGO.GetComponent<DeletedItem>();
        newDelItem.Init(qrSrcObj);
    }


    public void TakeScreenshotAndSave()
    {

    }
}
