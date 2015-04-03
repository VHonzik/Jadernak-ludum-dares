using UnityEngine;
using System.Collections;


public class Door : MonoBehaviour {

    public enum DoorEffect { NONE , TELEPORT,EXTERNALCO };

    public GUISkin styles;
    public string doorLabel;

    public AudioClip doorSound;

    public float distMin = 1f;
    public float distMax = 2f;

    private DoorEffect effect;
    private Vector2 telepParam;
    private IEnumerator extCoParam;

    private bool renderLabel;

    private AudioSource soundPlayer;


    void Awake()
    {
        renderLabel = true;
        effect = DoorEffect.NONE;

        soundPlayer = gameObject.AddComponent<AudioSource>();
        soundPlayer.playOnAwake = false;
        soundPlayer.loop = false;
        soundPlayer.volume = 0.5f;
    }

    void OnGUI()
    {
        GUI.skin = styles;
        if (renderLabel)
        {
            RenderLabel();
        }
        
    }

    void Update()
    {
        float dist = Vector2.Distance(
            new Vector2(GameObject.Find("pawn").transform.position.x, GameObject.Find("pawn").transform.position.y),
            new Vector2(gameObject.transform.position.x, gameObject.transform.position.y));
        if ( Input.GetAxis("Jump") != 0 && dist<0.5f)
        {
            if (effect != DoorEffect.NONE && GameObject.Find("pawn").GetComponent<PlayerPawn>().getIteractionEnabled())
            {
                GameObject.Find("pawn").GetComponent<PlayerPawn>().disableIteraction();
                GameObject.Find("pawn").GetComponent<PlayerPawn>().disableMovement();
                Debug.Log("Using door:" + gameObject.name);
                startEffect();
            }
            
        }
    }

    public IEnumerator FadeIn()
    {
        gameObject.GetComponent<Animator>().Play("door_fadin");
        yield return new WaitForSeconds(1.1f);
    }

    public IEnumerator FadeOut()
    {

        gameObject.GetComponent<Animator>().Play("door_fadeout");
        yield return new WaitForSeconds(1.1f);
    }

    public IEnumerator Swirl()
    {

        gameObject.GetComponent<Animator>().Play("door_swirl");
        yield return new WaitForSeconds(3);
    }


    public void setRenderLabel(bool rl)
    {
        renderLabel = rl;
    }

    public void setEffectToNone()
    {
        effect = DoorEffect.NONE;
    }

    public void setEffectToTeleport(Vector2 destination)
    {
        effect = DoorEffect.TELEPORT;
        telepParam = destination;
    }

    public void setEffectToExtCourutine(IEnumerator co)
    {
        effect = DoorEffect.EXTERNALCO;
        extCoParam = co;
    }

    private void startEffect()
    {
        Debug.Log("Starting effect.");
        switch (effect)
        {
            case DoorEffect.TELEPORT:                
                StartCoroutine("TeleportEffect",telepParam);
                break;
            case DoorEffect.EXTERNALCO:
                renderLabel = false;
                soundPlayer.PlayOneShot(doorSound);
                StartCoroutine(extCoParam);
                break;
        }        
        Debug.Log("Effect ended.");
    }

    private void RenderLabel()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(this.gameObject.transform.position);
        Vector2 textSize = styles.label.CalcSize(new GUIContent(doorLabel));
        float distXY = Vector2.Distance(
            new Vector2(GameObject.Find("pawn").transform.position.x, GameObject.Find("pawn").transform.position.y),
            new Vector2(gameObject.transform.position.x, gameObject.transform.position.y));
        float dist = (Mathf.Clamp(distXY, distMin, distMax) - distMin) / (distMax-distMin);
        float alpha = 1f - dist;
        string textWithAlpha = "<color=#FFFFFF" + HexFromFloat(alpha) + ">" + doorLabel + "</color>";
        Rect labelPos = new Rect(Mathf.Clamp(screenPos.x - textSize.x / 2f,10f,Screen.width - textSize.x - 5f), 
            Screen.height - screenPos.y - 75f, textSize.x, textSize.y);
        GUI.Label(labelPos, textWithAlpha);
    }

    public static string HexFromFloat(float number)
    {
        int h = (int)Mathf.Floor(number * 255);
        return h.ToString("X2").ToLower();
    }

    public IEnumerator TeleportEffect(Vector2 targetPos)
    {
        GameObject pawn = GameObject.Find("pawn");

        Debug.Log("Teleporting");
        soundPlayer.PlayOneShot(doorSound);
        yield return StartCoroutine(pawn.GetComponent<PlayerPawn>().FadeOutPlayer());
        pawn.transform.position = new Vector3(targetPos.x, targetPos.y, pawn.transform.position.z);
        yield return new WaitForSeconds(0.5f);
        soundPlayer.PlayOneShot(doorSound);
        yield return StartCoroutine(pawn.GetComponent<PlayerPawn>().FadeInPlayer());
        pawn.GetComponent<PlayerPawn>().enableMovement();
        pawn.GetComponent<PlayerPawn>().enableIteraction();
        Debug.Log("End of teleport.");

    }
}
