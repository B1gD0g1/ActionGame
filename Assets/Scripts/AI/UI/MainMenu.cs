using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    [SerializeField] private Button playButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;


    private void Start()
    {
        //startButton.onClick.AddListener(StartGame);
        //quitButton.onClick.AddListener(Quit);

        if (playButton == null)
        {
            Debug.LogError("startButton 未正确赋值，请检查 Inspector 面板！");
        }
        else
        {
            playButton.onClick.AddListener(StartGame);
        }

        if (quitButton == null)
        {
            Debug.LogError("quitButton 未正确赋值，请检查 Inspector 面板！");
        }
        else
        {
            quitButton.onClick.AddListener(Quit);
        }
    }

    private void StartGame()
    {
        Debug.Log("Start 按钮被点击！");
        SceneManager.LoadScene("GameScene");
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
