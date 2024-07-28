using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class NotiManager : MonoBehaviour, IObserver
{

    [Header("Assignments")]
    [SerializeField] private RectTransform notiPanel;
    [SerializeField] private RectTransform notiItemPrefab;

    [Header("Settings")]
    [SerializeField][Range(1f, 60f)] private int maxNotiItemSameTime = 3;
    [SerializeField][Range(0.0001f, 6f)] private float notiItemLifeTimeSeconds = 2.75f;
    [SerializeField][Range(0.0001f, 6f)] private float notiItemShowSeconds = 0.15f;
    [SerializeField][Range(0.0001f, 6f)] private float notiItemHideSeconds = 0.15f;
    [SerializeField][Range(0.0001f, 6f)] private float notiItemRiseSeconds = 0.1f;
    [SerializeField][Range(0.0001f, 1080f)] private float notiItemPanelLineHeight = 48f;
    [SerializeField][Range(0f, 1080f)] private float notiItemPaddingY = 20f;

    private List<NotiItem> notiItems = new List<NotiItem>();
    public int counts = 0; //Just For Debug


    // Update is called once per frame
    void Update()
    {
        counts = notiItems.Count;
        if (Input.GetKeyDown(KeyCode.A))
        {
            CreateNewNoti("Test " + Random.Range(0, 100), 3f);
        }
    }

    public void CreateNewNoti(string text, float lifeTime = 2.5f)
    {
        CheckNotiSizeAndClear();

        RectTransform newItem = Instantiate(notiItemPrefab, notiPanel);

        NotiItem newNotiItem = newItem.GetComponent<NotiItem>();

        notiItems.Add(newNotiItem);

        newNotiItem.Set(this, text, lineHeight: notiItemPanelLineHeight, lifeTime: lifeTime, showTime: notiItemShowSeconds, hideTime: notiItemHideSeconds);

        SetAllPos();
    }

    public void SetAllPos()
    {
        for (int i = 0; i < notiItems.Count; i++)
        {
            int val = notiItems.Count - 1 - i;
            if (notiItems[val] != null)
            {
                notiItems[val]?.SetNewPosY(i * (notiItemPanelLineHeight + notiItemPaddingY), notiItemRiseSeconds);
            }
        }
    }

    public void CheckNotiSizeAndClear()
    {
        if (notiItems.Count > maxNotiItemSameTime)
        {
            int willRemoveCount = notiItems.Count - maxNotiItemSameTime;

            for (int i = 0; i < willRemoveCount; i++)
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
