using UnityEngine;
using System.Collections;

public class CredistScene : MonoBehaviour {


    private TextRenderer textRenderer;

    // Use this for initialization
    void Awake()
    {
        textRenderer = gameObject.GetComponent<TextRenderer>();
    }

    void Start()
    {
        StartCoroutine(StartScene());
    }
    private IEnumerator StartScene()
    {
        yield return StartCoroutine(textRenderer.FadeInText("The pawn wasn't a pawn anymore.\n"));
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(textRenderer.FadeOutText());
        yield return StartCoroutine(textRenderer.FadeInText("Thanks for playing.\n\n#LD29"));
        yield return new WaitForSeconds(10f);
        Application.Quit();
    }

}
