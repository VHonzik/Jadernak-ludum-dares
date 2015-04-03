using UnityEngine;
using System.Collections;

public class SceneThird : MonoBehaviour {

    public GameObject pawn;
    public GameObject king;
    public GameObject[] levelbgs;
    public GameObject[] doors;


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
        yield return StartCoroutine(textRenderer.FadeInText("Fates erased by reason\nand passion's just a whim."));
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

        yield return StartCoroutine(king.GetComponent<KingBoss>().FadeInKing());
        yield return StartCoroutine(pawn.GetComponent<PlayerPawn>().FadeInPlayer());        
        pawn.GetComponent<PlayerPawn>().enableMovement();
        pawn.GetComponent<PlayerPawn>().enableIteraction();

        if (!GameObject.Find("played").GetComponent<scenethirdsave>().playedAlready)
        {
            yield return StartCoroutine(king.GetComponent<KingBoss>().sayText("We, the King, have come to stop this."));
            yield return StartCoroutine(king.GetComponent<KingBoss>().sayText("Your place is in the front line pawn. Return now."));
            yield return new WaitForSeconds(2f);
            yield return StartCoroutine(king.GetComponent<KingBoss>().sayText("You refuse to follow our order?"));
            yield return StartCoroutine(king.GetComponent<KingBoss>().sayText("Very well."));
        }

       
        StartCoroutine("BossFight");
        
    }

    private IEnumerator BossFight()
    {
        yield return new WaitForSeconds(2f);
        
        StartCoroutine(king.GetComponent<KingBoss>().FireHorizontal()); 
        yield return new WaitForSeconds(5f);

        StartCoroutine(king.GetComponent<KingBoss>().FireHorizontal());
        yield return new WaitForSeconds(5f);

        StartCoroutine(king.GetComponent<KingBoss>().FireVertical());
        yield return new WaitForSeconds(15f);

        StartCoroutine(king.GetComponent<KingBoss>().FireHorizontal());
        yield return new WaitForSeconds(5f);

        StartCoroutine(king.GetComponent<KingBoss>().FireHorizontal());
        yield return new WaitForSeconds(5f);

        StartCoroutine(king.GetComponent<KingBoss>().FireVertical());
        yield return new WaitForSeconds(15f);

        StartCoroutine(king.GetComponent<KingBoss>().PowerUp(doors[0],doors[1]));
        yield return new WaitForSeconds(22f);
        
        StartCoroutine(king.GetComponent<KingBoss>().FireHorizontalSerie());
        yield return new WaitForSeconds(6f);

        StartCoroutine(king.GetComponent<KingBoss>().FireHorizontalSerie());
        yield return new WaitForSeconds(6f);
        
        StartCoroutine(king.GetComponent<KingBoss>().FireRain());
        yield return new WaitForSeconds(24f);

        StartCoroutine(king.GetComponent<KingBoss>().FireHorizontalSerie());
        yield return new WaitForSeconds(6f);

        StartCoroutine(king.GetComponent<KingBoss>().FireHorizontalSerie());
        yield return new WaitForSeconds(6f);

        StartCoroutine(king.GetComponent<KingBoss>().FireRain());
        yield return new WaitForSeconds(24f);


        StartCoroutine(EndScene());

    }

    public IEnumerator WipeScene()
    {
        StopCoroutine("BossFight");
        foreach (GameObject go in levelbgs)
        {
            StartCoroutine(go.GetComponent<SpriteFader>().FadeOutSprite());
        }
        foreach (GameObject go in doors)
        {
            if (go != null)
            {
                StartCoroutine(go.GetComponent<Door>().FadeOut());
            }            
        }

        StartCoroutine(textRenderer.FadeOutText());
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(king.GetComponent<KingBoss>().sayText("Your end was inevitable."));
        yield return StartCoroutine(king.GetComponent<KingBoss>().FadeOutKing());
      

        GameObject.Find("played").GetComponent<scenethirdsave>().playedAlready = true;

        Application.LoadLevel("level4");
    }

    public IEnumerator EndScene()
    {
        StopCoroutine("BossFight");
        foreach (GameObject go in levelbgs)
        {
            StartCoroutine(go.GetComponent<SpriteFader>().FadeOutSprite());
        }

        StartCoroutine(textRenderer.FadeOutText());
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(king.GetComponent<KingBoss>().sayText("We..."));
        yield return StartCoroutine(king.GetComponent<KingBoss>().sayText("Perhaps we were wrong."));
        yield return StartCoroutine(king.GetComponent<KingBoss>().sayText("We cannot see beneath the surface."));
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(king.GetComponent<KingBoss>().sayText("Go now."));
        yield return StartCoroutine(king.GetComponent<KingBoss>().sayText("Be what you want to be."));
        yield return StartCoroutine(king.GetComponent<KingBoss>().FadeOutKing());
        yield return StartCoroutine(GameObject.Find("pawn").GetComponent<PlayerPawn>().FadeOutPlayer());
        yield return new WaitForSeconds(1f);

        Application.LoadLevel("levelcredits");
    }


}
