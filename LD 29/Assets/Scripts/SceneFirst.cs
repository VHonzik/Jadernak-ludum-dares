using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TextRenderer))]
public class SceneFirst : MonoBehaviour {

    public GameObject pawn;
    public GameObject[] doors;
    public GameObject[] levelbgs;

    private TextRenderer textRenderer;

	// Use this for initialization
	void Awake () {
        textRenderer = gameObject.GetComponent<TextRenderer>();
	}

    void Start()
    {
        StartCoroutine(StartScene());
    }

    private IEnumerator StartScene()
    {
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(textRenderer.FadeInText("Scratch underneath the surface,\nwhere does your purpose lie?"));
        foreach (GameObject go in levelbgs)
        {
            StartCoroutine(go.GetComponent<SpriteFader>().FadeInSprite());
        }

        foreach (GameObject go in doors)
        {
            StartCoroutine(go.GetComponent<Door>().FadeIn());
        }

        doors[0].GetComponent<Door>().setEffectToTeleport(new Vector2(doors[1].transform.position.x, doors[1].transform.position.y));
        doors[1].GetComponent<Door>().setEffectToTeleport(new Vector2(doors[0].transform.position.x, doors[0].transform.position.y));
        doors[2].GetComponent<Door>().setEffectToTeleport(new Vector2(doors[4].transform.position.x, doors[4].transform.position.y));
        doors[4].GetComponent<Door>().setEffectToTeleport(new Vector2(doors[0].transform.position.x, doors[0].transform.position.y));
        doors[5].GetComponent<Door>().setEffectToTeleport(new Vector2(doors[1].transform.position.x, doors[1].transform.position.y));
        doors[3].GetComponent<Door>().setEffectToExtCourutine(NextLevel());

        
        yield return StartCoroutine(pawn.GetComponent<PlayerPawn>().FadeInPlayer());
        pawn.GetComponent<PlayerPawn>().enableMovement();
        pawn.GetComponent<PlayerPawn>().enableIteraction();
    }

    public IEnumerator NextLevel()
    {
        StartCoroutine(pawn.GetComponent<PlayerPawn>().FadeOutPlayer());

        yield return StartCoroutine(textRenderer.FadeOutText());

        foreach (GameObject go in levelbgs)
        {
             StartCoroutine(go.GetComponent<SpriteFader>().FadeOutSprite());
        }
        foreach (GameObject go in doors)
        {
            StartCoroutine(go.GetComponent<Door>().FadeOut());
        }
        yield return new WaitForSeconds(1f);
        
        Application.LoadLevel("level2");
    }

}
