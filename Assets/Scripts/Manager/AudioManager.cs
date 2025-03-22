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

    //控制主音量
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat(MasterSound, volume);
    }

    //控制背景音乐音量
    public void SetBackgroundVolume(float volume)
    {
        audioMixer.SetFloat(BackgroundSound, volume);
    }

    //控制音效音量
    public void SetEffectVolume(float volume)
    {
        audioMixer.SetFloat(EffectSound, volume);
    }
}
