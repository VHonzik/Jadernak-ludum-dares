using UnityEngine;
using System.Collections;

public class SceneFourth : MonoBehaviour {

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
        yield return StartCoroutine(textRenderer.FadeInText("Feel empty so I breath in,\nkeep myself from giving in."));
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(textRenderer.FadeOutText());
        Application.LoadLevel("level3");
    }
}
