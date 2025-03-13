using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class UIMangaer : MonoBehaviour
{
    //通知对话结束,也就是字幕消失的时间
    public event Action OnDialogueEnd;


    [Header("字幕")]
    [SerializeField] private SubtitleUI subtitleUI;
    private Coroutine subtitleCoroutine;



    public void ShowSubtitle(string subtitleText, float duration, 
        bool isTypewriterEffect = false, float typeSpeed = 0.05f)
    {
        if (subtitleUI == null)
        {
            Debug.LogWarning("SubtitleUI 未绑定，请检查 UIManager 设置！");
            return;
        }

        if (subtitleCoroutine != null)
        {
            StopCoroutine(subtitleCoroutine);
        }

        subtitleCoroutine = StartCoroutine(DisplaySubtitle(subtitleText, duration, isTypewriterEffect, typeSpeed));
    }

    //显示字幕
    private IEnumerator DisplaySubtitle(string subtitleText, float duration, bool isTypewriterEffect = false,
        float typeSpeed = 0.05f)
    {

        //淡入字幕 0.5秒
        subtitleUI.FadeIn(0.5f);

        //设置字幕，并使用打字机效果
        subtitleUI.SetSubtitleText(subtitleText, isTypewriterEffect, typeSpeed);

        //等待打字完成 + 显示时间
        yield return new WaitForSeconds(duration + (subtitleText.Length * typeSpeed));

        //淡出字幕 0.5秒
        subtitleUI.FadeOut(0.5f);
        subtitleUI.SetSubtitleText("");//清空文本

        //触发事件，通知对话结束
        OnDialogueEnd?.Invoke();
    }
}
