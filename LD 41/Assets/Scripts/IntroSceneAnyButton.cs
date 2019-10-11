using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroSceneAnyButton : MonoBehaviour
{
    private bool Loaded;

    public Text LoadingText;

    // Use this for initialization
    void Start()
    {
        Loaded = false;
        StartCoroutine(LoadScene());
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator LoadScene()
    {
        var asyncLoad = SceneManager.LoadSceneAsync(1);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            if (!Loaded && asyncLoad.progress >= 0.9f)
            {
                Loaded = true;
                LoadingText.text = "Press any button to continue...";
            }

            if (Loaded && Input.anyKeyDown)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }


        Loaded = true;
    }
}
