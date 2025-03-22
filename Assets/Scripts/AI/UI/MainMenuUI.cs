using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{

    [SerializeField] private LoadManager loadManager;
    [SerializeField] private OptionsUI optionsUI;
    [SerializeField] private Button playButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;



    private void Awake()
    {
        playButton.onClick.AddListener(Play);
        optionsButton.onClick.AddListener (Options);
        quitButton.onClick.AddListener(Quit);
    }

    private void Play()
    {
        loadManager.Load(LoadManager.Scene.GameScene);
    }

    private void Options()
    {
        optionsUI.Show();
    }

    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
