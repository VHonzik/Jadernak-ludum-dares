using UnityEngine;
using System.Collections;

public class SceneSecond : MonoBehaviour {

    public GameObject pawn;
    public GameObject[] levelbgs;
    public GameObject[] stars;
    public GameObject exitDoor;
    private TextRenderer textRenderer;

    private int starsCount;
    private int startToCollect;


    // Use this for initialization
    void Awake()
    {
        textRenderer = gameObject.GetComponent<TextRenderer>();
        startToCollect = stars.Length;
        starsCount = 0;
    }

    void Start()
    {
        StartCoroutine(StartScene());
    }

    private IEnumerator StartScene()
    {
        exitDoor.GetComponent<Door>().setRenderLabel(false);
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(textRenderer.FadeInText("It seems your will is worthless,\nlike you're pawn beneath the sky."));

        foreach (GameObject go in levelbgs)
        {
            StartCoroutine(go.GetComponent<SpriteFader>().FadeInSprite());
        }

        foreach (GameObject go in stars)
        {
            go.GetComponent<CollectableStar>().setOnCollect(starAcquired);
            StartCoroutine(go.GetComponent<SpriteFader>().FadeInSprite());
        }

        

       
        yield return StartCoroutine(pawn.GetComponent<PlayerPawn>().FadeInPlayer());
        pawn.GetComponent<PlayerPawn>().enableMovement();
        pawn.GetComponent<PlayerPawn>().enableIteraction();
    }

    private void starAcquired()
    {
        starsCount++;
        if(starsCount >= startToCollect)
        {
            exitDoor.GetComponent<Door>().setRenderLabel(true);
            StartCoroutine(exitDoor.GetComponent<Door>().FadeIn());
            exitDoor.GetComponent<Door>().setEffectToExtCourutine(NextLevel());
        }
    }

    public IEnumerator NextLevel()
    {        
        StartCoroutine(pawn.GetComponent<PlayerPawn>().FadeOutPlayer());

        yield return StartCoroutine(textRenderer.FadeOutText());

        foreach (GameObject go in levelbgs)
        {
            StartCoroutine(go.GetComponent<SpriteFader>().FadeOutSprite());
        }

        yield return StartCoroutine(exitDoor.GetComponent<Door>().FadeOut());

        Application.LoadLevel("level3");
    }
}
