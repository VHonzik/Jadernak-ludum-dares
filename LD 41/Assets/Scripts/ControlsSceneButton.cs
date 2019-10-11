using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ControlsSceneButton : MonoBehaviour
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
        var asyncLoad = SceneManager.LoadSceneAsync(2);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            if (!Loaded && asyncLoad.progress >= 0.9f)
            {
                Loaded = true;
                LoadingText.text = "Press Advance text button/key to continue...";
            }

            if (Loaded && Input.GetButtonDown("InspectItem"))
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }


        Loaded = true;
    }
}
