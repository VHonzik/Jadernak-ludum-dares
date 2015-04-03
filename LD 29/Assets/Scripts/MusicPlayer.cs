using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour {

    public bool dontDestroy = true;
    public bool fadeInOnStart = true;
    public float fadeInDuration;
    private AudioSource musicPlayer;

    private float initVolume;

	// Use this for initialization
	void Awake () {
        
        musicPlayer = gameObject.GetComponent<AudioSource>();
        initVolume = musicPlayer.volume;
        musicPlayer.volume = 0.0f;
        if (dontDestroy)
        {
            Object.DontDestroyOnLoad(this.gameObject);
        }

        if (fadeInOnStart)
        {
            StartCoroutine(FadeIn(fadeInDuration));
        }
	}
	

    public IEnumerator FadeIn(float duration)
    {
        float timer = 0;
        musicPlayer.volume = 0.0f;
        while (timer < duration)
        {
            musicPlayer.volume = Mathf.Lerp(0, initVolume, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator FadeOut(float duration)
    {
        float timer = 0;
        float curVolume = musicPlayer.volume;
        while (timer < duration)
        {
            musicPlayer.volume = Mathf.Lerp(curVolume, 0.0f, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

    }
}
