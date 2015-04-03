using UnityEngine;
using System.Collections;

public class TextRenderer : MonoBehaviour {
    
    public GUISkin styles;    

    private Rect displayArea;

    private string textToRender;
    private string unformattedTextToRender;
    //Defines speed of Fading In by number of characters starting appearing with the first one
    private const float textFadingInCharCount = 5f;
    //Defines speed of Fading In by time required to appear one letter
    private const float textFadingInCharSpeed = 0.3f;
    //Defines delay after Fading In by Word(Count) / textFadingInDelay
    private const float textFadingInDelay = 500f / 60f;

    //Defines how fast text fades out in seconds
    private const float textFadingOutDuration = 1.0f;

	void Awake () {
        textToRender = "";
        unformattedTextToRender = "";
        float left = (Screen.width * 0.2f);
        float width = (Screen.width * 0.6f);

        displayArea = new Rect(left, 20, width, (Screen.height * 0.5f));
	}
	
	void OnGUI() {
        GUI.skin = styles;
        GUILayout.BeginArea(displayArea);
        GUILayout.Label(textToRender, styles.GetStyle("Title"));
        GUILayout.EndArea();
	}

    public IEnumerator FadeInText(string text)
    {
        unformattedTextToRender = text;
        char[] characters = text.ToCharArray();
        float[] alphas = new float[characters.Length];

        for (int i = 0; i < alphas.Length; i++)
        {
            alphas[i] = -i * (1f / (textFadingInCharCount + 1f));
        }

        while (alphas[alphas.Length - 1] < 1.0f)
        {
            textToRender = "";
            for (int i = 0; i < alphas.Length; i++)
            {
                float alpha = Mathf.Clamp(alphas[i], 0f, 1f);
                textToRender += "<color=#FFFFFF" + HexFromFloat(alpha) + ">" + characters[i] + "</color>";
                alphas[i] += Time.deltaTime / textFadingInCharSpeed;
            }
            yield return null;
        }
        textToRender = unformattedTextToRender;
    }

    public IEnumerator FadeOutText()
    {
        float timer = textFadingOutDuration;
        while (timer > 0)
        {
            string color = "#FFFFFF" +
                HexFromFloat(timer / textFadingOutDuration);

            textToRender = "<color=" + color + ">" + unformattedTextToRender + "</color>";

            timer -= Time.deltaTime;
            yield return null;
        }
        textToRender = "";
        unformattedTextToRender = "";
    }

    public static string HexFromFloat(float number)
    {
        int h = (int)Mathf.Floor(number * 255);
        return h.ToString("X2").ToLower();
    }
}
