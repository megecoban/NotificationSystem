using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotiManager : MonoBehaviour, IObserver
{

    [Header("Assignments")]
    [SerializeField] private RectTransform notiPanel;
    [SerializeField] private RectTransform notiItemPrefab;

    [Header("Settings")]
    [SerializeField][Range(1f, 60f)] private int maxNotiItemSameTime = 3;
    [SerializeField][Range(0.0001f, 100f)] private float notiItemLifeTimeSeconds = 2.75f;
    [SerializeField][Range(0.0001f, 6f)] private float notiItemShowSeconds = 0.15f;
    [SerializeField][Range(0.0001f, 6f)] private float notiItemHideSeconds = 0.15f;
    [SerializeField][Range(0.0001f, 6f)] private float notiItemRiseSeconds = 0.1f;
    [SerializeField][Range(0.0001f, 1080f)] private float notiItemPanelLineHeight = 48f;
    [SerializeField][Range(10f, 32f)] private float fontSizeMax = 22f;
    [SerializeField][Range(10f, 32f)] private float fontSizeMin = 18f;

    [Header("Text Margins And Paddings")]
    [SerializeField][Range(0f, 1080f)] private float notiItemMarginY = 20f;
    [SerializeField][Range(0f, 1080f)] private float notiItemMarginX = 20f;
    [SerializeField][Range(0f, 1080f)] private float notiItemPaddingY = 20f;
    [SerializeField][Range(0f, 1080f)] private float notiItemPaddingX = 20f;

    [Header("Offset")]
    [SerializeField][Range(0f, 1080f)] private float notiItemOffsetY = 20f;

    [Header("Text Alignments")]
    [SerializeField] private HorizontalAlignmentOptions textHorizontalAlignment = HorizontalAlignmentOptions.Center;
    [SerializeField] private VerticalAlignmentOptions textVerticalAlignment = VerticalAlignmentOptions.Middle;

    [Header("Text Colors")]
    [SerializeField] private Color textColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color backgroundColor = new Color(0f, 0f, 0f, 0.75f);

    private List<NotiItem> notiItems = new List<NotiItem>();
    [Header("Debug")]
    public int counts = 0; //Just For Debug


    // Update is called once per frame
    void Update()
    {
        counts = notiItems.Count;
        if (Input.GetKeyDown(KeyCode.A))
        {
            CreateNewNoti("Test " + Random.Range(0, 100), notiItemLifeTimeSeconds);
        }
    }

    public void CreateNewNoti(string text, float lifeTime = 2.5f)
    {
        CheckNotiSizeAndClear();

        RectTransform newItem = Instantiate(notiItemPrefab, notiPanel.transform, false);

        NotiItem newNotiItem = newItem.GetComponent<NotiItem>();

        notiItems.Add(newNotiItem);

        newNotiItem.Set(this, text, fontSizeMin: fontSizeMin, fontSizeMax: fontSizeMax, textColor: textColor, paddingX: notiItemPaddingX, paddingY: notiItemPaddingY, backgroundColor: backgroundColor, horizontalAlignment: textHorizontalAlignment, marginX: notiItemMarginX, verticalAlignment: textVerticalAlignment, lineHeight: notiItemPanelLineHeight, lifeTime: lifeTime, showTime: notiItemShowSeconds, hideTime: notiItemHideSeconds);

        SetAllPos();
    }

    public void SetAllPos()
    {
        for (int i = 0; i < notiItems.Count; i++)
        {
            int val = notiItems.Count - 1 - i;
            if (notiItems[val] != null)
            {
                notiItems[val]?.SetNewPosY(notiItemOffsetY + (i * (notiItemPanelLineHeight + notiItemMarginY)), notiItemRiseSeconds);
            }
        }
    }

    public void CheckNotiSizeAndClear()
    {
        if (notiItems.Count >= maxNotiItemSameTime)
        {
            int willRemoveCount = notiItems.Count - maxNotiItemSameTime;

            for (int i = 0; i <= willRemoveCount; i++)
            {
                if (notiItems[i] != null)
                {
                    notiItems[i].RemoveImmediatley();
                }
            }
        }

        for (int i = notiItems.Count - 1; i > -1; i--)
        {
            if (notiItems[i] == null)
            {
                notiItems?.RemoveAt(i);
            }
        }
    }

    public void NotifyCheck()
    {
        for (int i = notiItems.Count - 1; i > -1; i--)
        {
            if (notiItems[i] == null)
            {
                notiItems?.RemoveAt(i);
            }
        }
    }
}
