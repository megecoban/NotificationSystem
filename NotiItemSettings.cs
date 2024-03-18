using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotiItemSettings : MonoBehaviour
{
    [Header("Requirements")]
    [SerializeField] private NotiSettings notiBaseSettings;

    [Header("Assignments")]
    [SerializeField] private Image notiPanel;
    [SerializeField] private TextMeshProUGUI notiTextArea;

    [Header("Color Settings")]
    private Color panelColor = new Color(0f, 0f, 0f, 0.8f);
    private Color textColor = Color.white;

    [Header("Settings")]
    [Range(1f, 3f)] private int line = 3;
    [Range(8f, 24f)] private float fontSize = 17f;
    [Range(0f, 20f)] private float marginY = 5f;
    [Range(0f, 20f)] private float marginX = 15f;
    [Range(100f, 550f)] private float width = 400f;

    private bool isRaise = false;

    private void Awake()
    {
        notiPanel.color = panelColor;
        notiTextArea.color = textColor;

        notiTextArea.fontSize = fontSize;

        notiTextArea.rectTransform.sizeDelta = new Vector2(width, fontSize * line);
        notiPanel.rectTransform.sizeDelta = new Vector2(width + (marginX * 2f), (fontSize * line) + (marginY * 2f));
    }

    private void Setting(Color panelColor, Color textColor, float fontSize, float width, int line, float marginX, float marginY)
    {
        this.panelColor = panelColor;
        this.textColor = textColor;
        this.fontSize = fontSize;
        this.marginX = marginX;
        this.marginY = marginY;
        this.width = width;
        this.line = line;

        notiPanel.color = panelColor;
        notiTextArea.color = textColor;

        notiTextArea.fontSize = fontSize;

        notiTextArea.rectTransform.sizeDelta = new Vector2(width, fontSize * line);
        notiPanel.rectTransform.sizeDelta = new Vector2(width + (marginX * 2f), (fontSize * line) + (marginY * 2f));
    }

    public void Init(NotiSettings nS, string text, Color panelColor, Color textColor, float fontSize, float width, int line, float marginX, float marginY, float lifetimeTimer = 2.5f)
    {
        notiBaseSettings = nS;

        Setting(panelColor, textColor, fontSize, width, line, marginX, marginY);

        if (this.gameObject.activeSelf == false || this.gameObject.activeInHierarchy == false)
            this.gameObject.SetActive(true);

        lifetimeTimer = Mathf.Abs(lifetimeTimer);
        notiTextArea.text = text;

        StartCoroutine(NotiLifetime(lifetimeTimer));
    }

    IEnumerator NotiLifetime(float lifetimeDuration)
    {
        float lifetime = Mathf.Abs(lifetimeDuration);

        while (lifetime>=0f)
        {
            lifetime -= Time.deltaTime;

            if(lifetime <= 0f)
            {
                break;
            }

            yield return Time.deltaTime;
        }
        notiBaseSettings.DestroyThisNotiItem(this.gameObject.GetComponent<RectTransform>());
        Destroy(this.gameObject);
    }

    public void AddHeight(float height, float time = 0.25f)
    {
        height = Mathf.Abs(height);
        isRaise = true;
        StartCoroutine(Raise(height, time));
    }
    public void SetHeight(float newHeight, float time = 0.25f)
    {
        newHeight = Mathf.Abs(newHeight);
        StartCoroutine(Raise(newHeight, time));
    }

    IEnumerator Raise(float newHeight, float maxTime = 0.25f)
    {
        float elapsedTime = 0;
        Vector2 startPos = notiPanel.rectTransform.anchoredPosition;
        Vector2 endPos = new Vector2(notiPanel.rectTransform.anchoredPosition.x, newHeight);

        while (elapsedTime < maxTime)
        {
            float t = elapsedTime / maxTime;
            notiPanel.rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            elapsedTime += Time.deltaTime;
            yield return Time.deltaTime;
        }

        notiPanel.rectTransform.anchoredPosition = endPos;
    }


    public void DestroyNow()
    {
        StopCoroutine("NotiLifetime");
        notiBaseSettings.DestroyThisNotiItem(this.gameObject.GetComponent<RectTransform>());
        Destroy(this.gameObject);
    }
}
