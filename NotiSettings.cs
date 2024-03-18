using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class NotiSettings : MonoBehaviour
{
    public enum notiPanelAllignment
    {
        Right,
        Left
    }

    [Header("Assignments")]
    [SerializeField] private Canvas notiCanvas;
    [SerializeField] private RectTransform notiPanel;
    [SerializeField] private RectTransform notiItemPanel;

    [Header("Settings")]
    [SerializeField] private notiPanelAllignment panelAllignment;
    [SerializeField] [Range(1f,6f)] private int maxNotiItemSameTime = 3;

    [Space(16)]
    [Header("Noti Item")]
    [Space(8)]

    [Header("Noti Item Color Settings")]
    [SerializeField] private Color panelColor = new Color(0f, 0f, 0f, 0.8f);
    [SerializeField] private Color textColor = Color.white;

    [Header("Noti Item Settings")]
    [SerializeField][Range(1f, 3f)] private int line = 3;
    [SerializeField][Range(8f, 18f)] private float fontSize = 17f;
    [SerializeField][Range(0f, 20f)] private float marginY = 5f;
    [SerializeField][Range(0f, 20f)] private float marginX = 15f;
    [SerializeField][Range(100f, 550f)] private float width = 400f;
    [SerializeField][Range(0, 50f)] private float spacingX = 20f;
    [SerializeField][Range(0, 50f)] private float spacingY = 20f;
    [SerializeField][Range(0, 50f)] private float spacingEachother = 10f;

    private List<RectTransform> notiItemPanels = new List<RectTransform>();
    private List<NotiItemSettings> notiItemSettings = new List<NotiItemSettings>();

    private void Awake()
    {
        // Calculations
        float sWidth = Screen.width;
        int hsWidth = Mathf.FloorToInt(sWidth / 2);
        Rect notiPanelRect = notiPanel.rect;
        notiPanelRect.width = hsWidth;

        // Noti Panel Setting
        notiPanel.SetInsetAndSizeFromParentEdge((panelAllignment == notiPanelAllignment.Right) ? RectTransform.Edge.Right : RectTransform.Edge.Left, 0, hsWidth);

        // Noti Item Panel Setting
        notiItemPanel.anchorMin = (panelAllignment == notiPanelAllignment.Right) ? new Vector2(1f, 0) : new Vector2(0, 0);
        notiItemPanel.anchorMax = (panelAllignment == notiPanelAllignment.Right) ? new Vector2(1f, 0) : new Vector2(0, 0);
        notiItemPanel.pivot = (panelAllignment == notiPanelAllignment.Right) ? new Vector2(1f, 0) : new Vector2(0, 0);
        notiItemPanel.anchoredPosition = (panelAllignment == notiPanelAllignment.Right) ? new Vector3(-1f * spacingX, spacingY, 0f) : new Vector3(spacingX, spacingY, 0f);


    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            CreateNewNoti("Test " + Random.Range(0, 77f).ToString());
        }
    }

    public void CreateNewNoti(string notiText, float lifetimeDuartion = 2.5f)
    {
        if (notiItemPanels.Count + 1 > maxNotiItemSameTime)
        {
            //ilkini sil
            notiItemPanels.RemoveAt(0);

            NotiItemSettings tempSettings = notiItemSettings[0];

            notiItemSettings.RemoveAt(0);

            tempSettings.DestroyNow();

        }

        //sonra

        CheckDestroyed();

        for (int i = 0; i < notiItemSettings.Count; i++)
        {
            int k = notiItemSettings.Count - i;
            notiItemSettings[i].SetHeight( ((line*fontSize) * k) + (spacingEachother * (k)) + spacingY);
        }

        RectTransform newNotiItem = Instantiate(notiItemPanel, notiCanvas.transform);

        // Noti Item Panel Setting
        newNotiItem.anchorMin = (panelAllignment == notiPanelAllignment.Right) ? new Vector2(1f, 0) : new Vector2(0, 0);
        newNotiItem.anchorMax = (panelAllignment == notiPanelAllignment.Right) ? new Vector2(1f, 0) : new Vector2(0, 0);
        newNotiItem.pivot = (panelAllignment == notiPanelAllignment.Right) ? new Vector2(1f, 0) : new Vector2(0, 0);
        newNotiItem.anchoredPosition = (panelAllignment == notiPanelAllignment.Right) ? new Vector3(-1f * spacingX, spacingY, 0f) : new Vector3(spacingX, spacingY, 0f);

        notiItemPanels.Add(newNotiItem);
        notiItemSettings.Add(newNotiItem.GetComponent<NotiItemSettings>());

        newNotiItem.GetComponent<NotiItemSettings>().Init(this, notiText, panelColor, textColor, fontSize, width, line, marginX, marginY, lifetimeDuartion);

    }

    public void DestroyThisNotiItem(RectTransform notiRect)
    {
        for(int i = 0; i < notiItemPanels.Count; ++i)
        {
            if (notiItemPanels[i] ==  notiRect)
            {
                notiItemPanels.RemoveAt(i);
                notiItemSettings.RemoveAt(i);
            }
        }
    }

    private void CheckDestroyed()
    {
        if(notiItemPanels.Count > 0)
        {
            for(int i = notiItemPanels.Count-1; i > -1; i--)
            {
                if (notiItemPanels[i] == null)
                {
                    notiItemPanels.RemoveAt(i);
                    notiItemSettings.RemoveAt(i);
                }
            }
        }
    }
}
