using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SubtitleUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI subtitleText;
    [SerializeField] private CanvasGroup canvasGroup;
    private Coroutine fadeCoroutine;
    private Coroutine typeCoroutine;


    private void Start()
    {
        SetSubtitleText("");
        canvasGroup.alpha = 0f;
        Hide();
    }

    public void Show()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }

    //设置字幕文本
    public void SetSubtitleText(string text, bool isTypewriterEffect = false, float typeSpeed = 0.05f)
    {
        if (subtitleText != null && !isTypewriterEffect)
        {
            subtitleText.text = text;
        }

        if (subtitleText != null && isTypewriterEffect)
        {
            if (typeCoroutine != null)
                StopCoroutine(typeCoroutine);

            typeCoroutine = StartCoroutine(TypeTextCoroutine(text, typeSpeed));
        }
    }

    public void TypeSubtitle(string text, float typeSpeed = 0.05f)
    {

    }

    //渐变显示字幕效果
    public void FadeIn(float duration = 0.5f)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        gameObject.SetActive(true);
        fadeCoroutine = StartCoroutine(FadeCanvasGroup(0f, 1f, duration));
    }

    //渐变隐藏字幕
    public void FadeOut(float duration = 0.5f)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeCanvasGroup(1f, 0f, duration, true));
    }

    //字幕打字机效果协程
    private IEnumerator TypeTextCoroutine(string text, float typeSpeed)
    {
        //清空文本
        subtitleText.text = "";

        foreach (var letter in text)
        {
            subtitleText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }
    }

    //渐变效果协程
    private IEnumerator FadeCanvasGroup(float startAlpha, 
        float targetAlpha, float duration, bool hideAfterFade = false)
    {
        float time = 0f;
        while (time < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        if (hideAfterFade && targetAlpha == 0f)
        {
            Hide();
        }
    }
}
