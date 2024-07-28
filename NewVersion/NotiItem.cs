using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private Image notiPanel;
    [SerializeField] private TextMeshProUGUI notiTextArea;

    private NotiPhase coroutineNotiPhase = NotiPhase.Show;

    // SETS

    public void Set(NotiManager notiManager, string text, float lineHeight = 48f, float fontSizeMin = 18f, float fontSizeMax = 22f, HorizontalAlignmentOptions horizontalAlignment = HorizontalAlignmentOptions.Center, VerticalAlignmentOptions verticalAlignment = VerticalAlignmentOptions.Middle, float showTime = 0.15f, float hideTime = 0.15f, float lifeTime = 2.5f)
    {
        this.notiManager = notiManager;

        notiPanel = this.GetComponent<Image>();
        notiTextArea = this.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        notiPanel.rectTransform.sizeDelta = new Vector2(notiPanel.rectTransform.parent.GetComponent<RectTransform>().sizeDelta.x, lineHeight);

        StartCoroutine(Show(showTime, () =>
        {
            StartCoroutine(LifeTime(lifeTime, () => { Remove(hideTime); }));
        }));

        Color backgroundColor = new Color(0f, 0f, 0f, 0f);
        Color whiteColor = new Color(1f, 1f, 1f, 1f);

        SetPanel(backgroundColor);
        SetText(text, whiteColor, fontSizeMin, fontSizeMax, horizontalAlignment, verticalAlignment);
    }

    public void SetPanel(Color backgroundColor)
    {
        notiPanel.color = backgroundColor;
    }

    public void SetText(string text, Color textColor, float fontSizeMin = 18f, float fontSizeMax = 22f, HorizontalAlignmentOptions horizontalAlignment = HorizontalAlignmentOptions.Center, VerticalAlignmentOptions verticalAlignment = VerticalAlignmentOptions.Middle)
    {
        notiTextArea.text = text;
        notiTextArea.color = textColor;
        notiTextArea.horizontalAlignment = HorizontalAlignmentOptions.Center;
        notiTextArea.verticalAlignment = VerticalAlignmentOptions.Middle;
        notiTextArea.autoSizeTextContainer = true;
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
        Debug.Log("Show ");
        float timer = showTime;
        float panelChannelA = notiPanel.color.a;
        float textChannelA = notiTextArea.color.a;


        while (timer > 0f)
        {
            timer -= Time.deltaTime;

            float tempPanelA = Mathf.Lerp(panelChannelA, 0f, timer / showTime);
            float tempTextA = Mathf.Lerp(textChannelA, 0f, timer / showTime);

            notiPanel.color = new Color(notiPanel.color.r, notiPanel.color.g, notiPanel.color.b, tempPanelA);
            notiTextArea.color = new Color(notiTextArea.color.r, notiTextArea.color.g, notiTextArea.color.b, tempTextA);

            yield return null;
        }

        notiPanel.color = new Color(notiPanel.color.r, notiPanel.color.g, notiPanel.color.b, panelChannelA);
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

        Debug.Log("Life ");


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
        Debug.Log("Hide ");
        float timer = hideTime;
        float panelChannelA = notiPanel.color.a;
        float textChannelA = notiTextArea.color.a;

        while (timer>0f)
        {
            timer -= Time.deltaTime;

            float tempPanelA = Mathf.Lerp(0f, panelChannelA, timer / hideTime);
            float tempTextA = Mathf.Lerp(0f, textChannelA, timer / hideTime);

            notiPanel.color = new Color(notiPanel.color.r, notiPanel.color.g, notiPanel.color.b, tempPanelA);
            notiTextArea.color = new Color(notiTextArea.color.r, notiTextArea.color.g, notiTextArea.color.b, tempTextA);

            yield return null;
        }

        notiPanel.color = new Color(notiPanel.color.r, notiPanel.color.g, notiPanel.color.b, 0f);
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
            notiPanel.rectTransform.anchoredPosition = Vector2.Lerp(notiPanel.rectTransform.anchoredPosition,
                new Vector2(notiPanel.rectTransform.anchoredPosition.x, newPosY)
                , timer/riseTime);

            timer += Time.deltaTime;

            yield return null;
        }
        notiPanel.rectTransform.anchoredPosition = new Vector2(notiPanel.rectTransform.anchoredPosition.x, newPosY);
    }

    public void SetNewPosY(float newPosY, float riseTime = 0.1f)
    {
        Debug.Log("New Pos Y: " + newPosY);
        if(newPosY == notiPanel.rectTransform.anchoredPosition.y) { return; }
        StopCoroutine("Rise");
        StartCoroutine(Rise(newPosY, riseTime));
    }


    // INTERFACE

    public void Notify()
    {
        notiManager.NotifyCheck();  
    }
}
