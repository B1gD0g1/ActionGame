using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    private const string MasterSound = "MasterSound";
    private const string BackgroundSound = "BackgroundSound";
    private const string EffectSound = "EffectSound";

    public static AudioManager Instance { get; private set; }

    public AudioMixer audioMixer;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //����������
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat(MasterSound, volume);
    }

    //���Ʊ�����������
    public void SetBackgroundVolume(float volume)
    {
        audioMixer.SetFloat(BackgroundSound, volume);
    }

    //������Ч����
    public void SetEffectVolume(float volume)
    {
        audioMixer.SetFloat(EffectSound, volume);
    }
}
