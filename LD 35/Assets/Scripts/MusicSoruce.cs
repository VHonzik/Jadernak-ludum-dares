using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MusicSoruce : MonoBehaviour {

    public AudioClip[] music;
    public AudioClip[] scribble;
    int currentMusicClip;
    private AudioSource musicSource;

    private AudioSource scribbleSource;

    public Slider volumeSlider;

    static System.Random rnd = new System.Random();

    private static MusicSoruce _instance;
    public static MusicSoruce Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<MusicSoruce>();
            }
            return _instance;
        }
    }



        // Use this for initialization
    void Awake () {
        _instance = this;
        musicSource = gameObject.AddComponent<AudioSource>();
        currentMusicClip = 0;
        musicSource.clip = music[currentMusicClip];
        musicSource.loop = false;
        musicSource.Play();
        musicSource.volume = 0.25f;

        scribbleSource = gameObject.AddComponent<AudioSource>();

        volumeSlider.onValueChanged.AddListener(delegate { VolumeChanged(); });

    }
	
	// Update is called once per frame
	void Update () {
	    if(!musicSource.isPlaying)
        {
            currentMusicClip = (currentMusicClip + 1) % music.Length;
            musicSource.clip = music[currentMusicClip];
            musicSource.Play();
        }
	}

    public void VolumeChanged()
    {
        musicSource.volume = volumeSlider.value;
    }

    public void PlayScribble()
    {
        if(!scribbleSource.isPlaying)
        {
            scribbleSource.clip = scribble[rnd.Next(scribble.Length)];
            scribbleSource.Play();
        }
    }
}
