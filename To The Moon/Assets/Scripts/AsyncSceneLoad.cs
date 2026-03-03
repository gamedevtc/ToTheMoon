using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AsyncSceneLoad : MonoBehaviour
{
    [SerializeField] string playScene;
    [SerializeField] string loadingScene;

    [SerializeField] public Slider loadSlider;
    [SerializeField] Canvas loadScreen;

    [SerializeField] Canvas blackScreen;
    [SerializeField] Animator blackScreenAnim;
    [SerializeField] string fadeInTrigger;
    [SerializeField] string fadeOutTrigger;
    [SerializeField] float startFadePercentage = 80;
    [SerializeField] bool hasTriggered = false;

    [SerializeField] float waitForFadeTime = 1f;
    [SerializeField] float waitToDestroyTime = 5f;

    [SerializeField] public List<AsyncOperation> sceneLoad = new List<AsyncOperation>();

    float sceneProgress;

    IEnumerator LoadLevel()
    {
        for (int i = 0; i < sceneLoad.Count; i++)
        {
            while (!sceneLoad[i].isDone)
            {
                sceneProgress = 0;
                foreach(AsyncOperation op in sceneLoad)
                {
                    sceneProgress += op.progress;
                }
                sceneProgress = (sceneProgress / (float)sceneLoad.Count) * 100f;
                loadSlider.value = Mathf.RoundToInt(sceneProgress);
                if (sceneProgress >= startFadePercentage && !hasTriggered)
                {
                    blackScreenAnim.SetTrigger(fadeInTrigger);
                    hasTriggered = true;
                }
                yield return null;
            }
        }

        loadScreen.gameObject.SetActive(false);
        yield return new WaitForSecondsRealtime(waitForFadeTime);
        blackScreenAnim.SetTrigger(fadeOutTrigger);
        yield return new WaitForSecondsRealtime(waitForFadeTime);
        blackScreen.gameObject.SetActive(false);
        sceneLoad.Clear();
        yield return new WaitForSecondsRealtime(waitToDestroyTime);
        Destroy(this.gameObject);
    }

    IEnumerator LoadFirst()
    {
        yield return new WaitForSecondsRealtime(waitForFadeTime);
        for (int i = 0; i < sceneLoad.Count; i++)
        {
            while (!sceneLoad[i].isDone)
            {
                yield return null;
            }
        }
        blackScreenAnim.SetTrigger(fadeOutTrigger);
        sceneLoad.Clear();
        yield return new WaitForSecondsRealtime(waitForFadeTime);
        loadScreen.gameObject.SetActive(true);
        LoadGame();
    }

    public void LoadGame()
    {
        sceneLoad.Add(SceneManager.LoadSceneAsync(playScene));
        StartCoroutine(LoadLevel());
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void runTransition()
    {
        blackScreen.gameObject.SetActive(true);
        blackScreenAnim.SetTrigger(fadeInTrigger);
        sceneLoad.Add(SceneManager.LoadSceneAsync(loadingScene));
        StartCoroutine(LoadFirst());
    }
}
