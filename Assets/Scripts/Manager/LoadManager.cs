using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadManager : MonoBehaviour
{

    public enum Scene
    {
        MainMenuScene,
        GameScene
    }

    [SerializeField] private LoadingUI loadingUI;


    public void Load(Scene targetScene)
    {
        StartCoroutine(LoadScene(targetScene));
    }

    private IEnumerator LoadScene(Scene targetScene)
    {
        loadingUI.loadingPanel.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(targetScene.ToString());

        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            loadingUI.slider.value = operation.progress;

            loadingUI.loadingProgressText.text = operation.progress * 100 + "%";

            if (operation.progress >= 0.9f)
            {
                loadingUI.slider.value = 1;

                loadingUI.loadingProgressText.text = "按下任意键继续";

                if (Input.anyKeyDown)
                {
                    operation.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }
}
