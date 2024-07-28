using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NotiItem: MonoBehaviour, ISubject
{
    public enum NotiPhase
    {
        Show,
        Life,
        Hide,
        None
    }

    [Header("Requirements")]
    [SerializeField] private NotiManager notiManager;

    [Header("Assignments")]
    [SerializeField] private Image notiTextBackgroundPanel;
    [SerializeField] private TextMeshProUGUI notiTextArea;
    [SerializeField] private RectTransform notiTextAreaRectTransform;

    private NotiPhase coroutineNotiPhase = NotiPhase.Show;

    // SETS

    public void Init(NotiManager notiManager)
    {
        this.notiManager = notiManager;

        notiTextBackgroundPanel = this.GetComponent<Image>();

        notiTextArea = notiTextBackgroundPanel.GetComponentInChildren<TextMeshProUGUI>();

        notiTextAreaRectTransform = notiTextArea.rectTransform;

        notiTextAreaRectTransform.localScale = Vector3.one;
    }

    public void InitCoroutine(float showTime = 0.15f, float hideTime = 0.15f, float lifeTime = 2.5f)
    {
        StartCoroutine(Show(showTime, () =>
        {
            StartCoroutine(LifeTime(lifeTime, () => { Remove(hideTime); }));
        }));
    }

    public void Set(NotiManager notiManager, string text, float paddingX = 0f, float paddingY = 0f, float marginX = 0f, float lineHeight = 48f, float fontSizeMin = 18f, float fontSizeMax = 22f, HorizontalAlignmentOptions horizontalAlignment = HorizontalAlignmentOptions.Center, VerticalAlignmentOptions verticalAlignment = VerticalAlignmentOptions.Middle, float showTime = 0.15f, float hideTime = 0.15f, float lifeTime = 2.5f)
    {
        Color backgroundColor = new Color(0f, 0f, 0f, 0f);
        Color textColor = new Color(1f, 1f, 1f, 1f);

        Set(notiManager, text, textColor, backgroundColor, paddingX, paddingY, marginX, lineHeight, fontSizeMin, fontSizeMax, horizontalAlignment, verticalAlignment, showTime, hideTime, lifeTime);
    }

    public void Set(NotiManager notiManager, string text, Color textColor, Color backgroundColor, float paddingX = 0f, float paddingY = 0f, float marginX = 0f, float lineHeight = 48f, float fontSizeMin = 18f, float fontSizeMax = 22f, HorizontalAlignmentOptions horizontalAlignment = HorizontalAlignmentOptions.Center, VerticalAlignmentOptions verticalAlignment = VerticalAlignmentOptions.Middle, float showTime = 0.15f, float hideTime = 0.15f, float lifeTime = 2.5f)
    {
        Init(notiManager);

        SetPanel(backgroundColor, marginX, lineHeight);
        SetText(text, textColor, paddingX, paddingY, fontSizeMin, fontSizeMax, horizontalAlignment, verticalAlignment);

        InitCoroutine(showTime, hideTime, lifeTime);
    }

    public void SetPanel(Color backgroundColor, float marginX, float lineHeight)
    {
        notiTextBackgroundPanel.color = backgroundColor;
        notiTextBackgroundPanel.rectTransform.sizeDelta = new Vector2(notiTextBackgroundPanel.rectTransform.parent.GetComponent<RectTransform>().sizeDelta.x - marginX, lineHeight);
    }

    public void SetText(string text, Color textColor, float paddingX, float paddingY, float fontSizeMin = 18f, float fontSizeMax = 22f, HorizontalAlignmentOptions horizontalAlignment = HorizontalAlignmentOptions.Center, VerticalAlignmentOptions verticalAlignment = VerticalAlignmentOptions.Middle)
    {
        notiTextArea.text = text;
        notiTextArea.color = textColor;

        notiTextArea.rectTransform.sizeDelta = new Vector2(notiTextBackgroundPanel.rectTransform.sizeDelta.x,
            notiTextBackgroundPanel.rectTransform.sizeDelta.y);

        notiTextArea.autoSizeTextContainer = true;
        notiTextArea.horizontalAlignment = horizontalAlignment;
        notiTextArea.verticalAlignment = verticalAlignment;
        notiTextArea.fontSizeMin = 18f;
        notiTextArea.fontSizeMax = 22f;
    }


    // REMOVES

    public void Remove(float hideTime = 0.25f)
    {
        StartCoroutine(Hide(hideTime, () => { RemoveImmediatley(); }));
    }

    public void RemoveImmediatley()
    {
        if(coroutineNotiPhase == NotiPhase.Show)
        {
            StopCoroutine("Show");
        }
        else if (coroutineNotiPhase == NotiPhase.Hide)
        {
            StopCoroutine("Hide");
        }
        else if (coroutineNotiPhase == NotiPhase.Life)
        {
            StopCoroutine("LifeTime");
        }
        Notify();
        Destroy(this.gameObject, Time.deltaTime);
    }


    // ANIMS

    IEnumerator Show(float showTime = 0.15f, UnityAction afterAction = null)
    {
        coroutineNotiPhase = NotiPhase.Show;

        float timer = showTime;
        float panelChannelA = notiTextBackgroundPanel.color.a;
        float textChannelA = notiTextArea.color.a;


        while (timer > 0f)
        {
            timer -= Time.deltaTime;

            float tempPanelA = Mathf.Lerp(panelChannelA, 0f, timer / showTime);
            float tempTextA = Mathf.Lerp(textChannelA, 0f, timer / showTime);

            notiTextBackgroundPanel.color = new Color(notiTextBackgroundPanel.color.r, notiTextBackgroundPanel.color.g, notiTextBackgroundPanel.color.b, tempPanelA);
            notiTextArea.color = new Color(notiTextArea.color.r, notiTextArea.color.g, notiTextArea.color.b, tempTextA);

            yield return null;
        }

        notiTextBackgroundPanel.color = new Color(notiTextBackgroundPanel.color.r, notiTextBackgroundPanel.color.g, notiTextBackgroundPanel.color.b, panelChannelA);
        notiTextArea.color = new Color(notiTextArea.color.r, notiTextArea.color.g, notiTextArea.color.b, textChannelA);

        if (afterAction != null)
        {
            afterAction.Invoke();
        }
    }

    IEnumerator LifeTime(float lifeTime = 2.5f, UnityAction afterAction = null)
    {
        coroutineNotiPhase = NotiPhase.Life;
        float timer = 0f;

        while (timer < lifeTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if(afterAction != null)
        {
            afterAction.Invoke();
        }
    }

    IEnumerator Hide(float hideTime = 0.15f, UnityAction afterAction = null)
    {
        coroutineNotiPhase = NotiPhase.Hide;

        float timer = hideTime;
        float panelChannelA = notiTextBackgroundPanel.color.a;
        float textChannelA = notiTextArea.color.a;

        while (timer>0f)
        {
            timer -= Time.deltaTime;

            float tempPanelA = Mathf.Lerp(0f, panelChannelA, timer / hideTime);
            float tempTextA = Mathf.Lerp(0f, textChannelA, timer / hideTime);

            notiTextBackgroundPanel.color = new Color(notiTextBackgroundPanel.color.r, notiTextBackgroundPanel.color.g, notiTextBackgroundPanel.color.b, tempPanelA);
            notiTextArea.color = new Color(notiTextArea.color.r, notiTextArea.color.g, notiTextArea.color.b, tempTextA);

            yield return null;
        }

        notiTextBackgroundPanel.color = new Color(notiTextBackgroundPanel.color.r, notiTextBackgroundPanel.color.g, notiTextBackgroundPanel.color.b, 0f);
        notiTextArea.color = new Color(notiTextArea.color.r, notiTextArea.color.g, notiTextArea.color.b, 0f);

        if(afterAction!=null)
        {
            afterAction.Invoke();
        }
    }

    IEnumerator Rise(float newPosY, float riseTime = 0.1f)
    {
        float timer = 0f;

        while(timer<riseTime)
        {
            notiTextBackgroundPanel.rectTransform.anchoredPosition = Vector2.Lerp(notiTextBackgroundPanel.rectTransform.anchoredPosition,
                new Vector2(notiTextBackgroundPanel.rectTransform.anchoredPosition.x, newPosY)
                , timer/riseTime);

            timer += Time.deltaTime;

            yield return null;
        }

        notiTextBackgroundPanel.rectTransform.anchoredPosition = new Vector2(notiTextBackgroundPanel.rectTransform.anchoredPosition.x, newPosY);
    }

    public void SetNewPosY(float newPosY, float riseTime = 0.1f)
    {
        Debug.Log("New Pos Y: " + newPosY);
        if(newPosY == notiTextBackgroundPanel.rectTransform.anchoredPosition.y) { return; }
        StopCoroutine("Rise");
        StartCoroutine(Rise(newPosY, riseTime));
    }


    // INTERFACE

    public void Notify()
    {
        notiManager.NotifyCheck();  
    }
}
