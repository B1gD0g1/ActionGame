using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class UIMangaer : MonoBehaviour
{
    //֪ͨ�Ի�����,Ҳ������Ļ��ʧ��ʱ��
    public event Action OnDialogueEnd;


    [Header("��Ļ")]
    [SerializeField] private SubtitleUI subtitleUI;
    private Coroutine subtitleCoroutine;



    public void ShowSubtitle(string subtitleText, float duration, 
        bool isTypewriterEffect = false, float typeSpeed = 0.05f)
    {
        if (subtitleUI == null)
        {
            Debug.LogWarning("SubtitleUI δ�󶨣����� UIManager ���ã�");
            return;
        }

        if (subtitleCoroutine != null)
        {
            StopCoroutine(subtitleCoroutine);
        }

        subtitleCoroutine = StartCoroutine(DisplaySubtitle(subtitleText, duration, isTypewriterEffect, typeSpeed));
    }

    //��ʾ��Ļ
    private IEnumerator DisplaySubtitle(string subtitleText, float duration, bool isTypewriterEffect = false,
        float typeSpeed = 0.05f)
    {

        //������Ļ 0.5��
        subtitleUI.FadeIn(0.5f);

        //������Ļ����ʹ�ô��ֻ�Ч��
        subtitleUI.SetSubtitleText(subtitleText, isTypewriterEffect, typeSpeed);

        //�ȴ�������� + ��ʾʱ��
        yield return new WaitForSeconds(duration + (subtitleText.Length * typeSpeed));

        //������Ļ 0.5��
        subtitleUI.FadeOut(0.5f);
        subtitleUI.SetSubtitleText("");//����ı�

        //�����¼���֪ͨ�Ի�����
        OnDialogueEnd?.Invoke();
    }
}
