using UnityEngine;
using System.Collections;

public class KingBoss : MonoBehaviour {

    public GUISkin styles;
    public GameObject projectile;
    private string bossText;

    public AudioClip shootSound;
    public AudioClip hitSound;

    private const float textFadingInDelay = 200f / 60f;

    private AudioSource soundPlayer;

    void Awake()
    {
        bossText = "";
        soundPlayer = gameObject.AddComponent<AudioSource>();
        soundPlayer.playOnAwake = false;
        soundPlayer.loop = false;
        soundPlayer.volume = 0.5f;
    }

    void OnGUI()
    {
        GUI.skin = styles;
        RenderLabel();

    }

    public IEnumerator PowerUp(GameObject door, GameObject door2)
    {
        yield return StartCoroutine(sayText("We are confused..."));
        yield return StartCoroutine(sayText("It cannot be..."));
        yield return StartCoroutine(sayText("Time to end this."));
        float timer = 0f;
        float flyingUpDur = 2.0f;
        Vector3 origPos = transform.position;
        Vector3 targetPos = origPos;
        targetPos.y = 1.3f;
        StartCoroutine(GlowInKing());
        while (timer < flyingUpDur)
        {
            transform.position = Vector3.Lerp(origPos, targetPos, timer / flyingUpDur);
            timer += Time.deltaTime;
            yield return null;
        }        
        
        StartCoroutine(GlowKing());
        yield return StartCoroutine(sayText("The power surges through us!"));
        yield return StartCoroutine(sayText("Soon you are helpless."));
        door.GetComponent<Door>().setEffectToNone();
        door2.GetComponent<Door>().setEffectToNone();
        StartCoroutine(door2.GetComponent<Door>().Swirl());
        yield return StartCoroutine(door.GetComponent<Door>().Swirl());
        Destroy(door);
        Destroy(door2);
        StartCoroutine(GlowOutKing());
        timer = 0f;
        while (timer < flyingUpDur)
        {
            transform.position = Vector3.Lerp(targetPos, origPos, timer / flyingUpDur);
            timer += Time.deltaTime;
            yield return null;
        }
        yield return StartCoroutine(sayText("Now die!"));
    }

    public IEnumerator FireHorizontalSerie()
    {
        StartCoroutine(sayText("There is no escape."));
        yield return StartCoroutine(FireHorizontal(0.6f, false, 0f));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FireHorizontal(-0.2f, false, 0f));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FireHorizontal(-1.0f, false, 0f));        
    }


    public IEnumerator FireHorizontal(float height = 0.6f, bool withText = true, float delay = 0.5f)
    {
        if (withText)
        {
            string[] texts = { "You are defeated.", "This line is your grave.", "You are worthless." };
            StartCoroutine(sayText(texts[Random.Range(0, texts.Length)]));
        }
        float rnd = Random.value;
        Vector3 firstPos,secondPos;
        float firstOri,secondOri;
        if (rnd < 0.5f)
        {
            firstPos = new Vector3(1.0f, height);
            secondPos = new Vector3(-1.0f, height);
            firstOri = 270f;
            secondOri = 90f;
        }
        else 
        {
            firstPos = new Vector3(-1.0f, height);
            secondPos = new Vector3(1.0f, height);
            firstOri = 90f;
            secondOri = 270f;
        }
        GameObject proj1 = (GameObject)Instantiate(projectile,
            gameObject.transform.position - firstPos, Quaternion.Euler(0, 0, firstOri));
        soundPlayer.PlayOneShot(shootSound);
        StartCoroutine(proj1.GetComponent<Projectile>().FadeIn());
        proj1.GetComponent<Projectile>().onHit = hittedPlayer;
        yield return new WaitForSeconds(delay);
        GameObject proj2 = (GameObject)Instantiate(projectile,
            gameObject.transform.position - secondPos, Quaternion.Euler(0, 0, secondOri));
         soundPlayer.PlayOneShot(shootSound);
         StartCoroutine(proj2.GetComponent<Projectile>().FadeIn());
         proj2.GetComponent<Projectile>().onHit = hittedPlayer;
    }

    public IEnumerator FireRain()
    {
        yield return StartCoroutine(sayText("Impossible..."));
        float timer = 0f;
        float flyingUpDur = 2.0f;
        Vector3 origPos = transform.position;
        Vector3 targetPos = origPos;
        targetPos.y = 1.6f;
        StartCoroutine(GlowInKing());
        while (timer < flyingUpDur)
        {
            transform.position = Vector3.Lerp(origPos, targetPos, timer / flyingUpDur);
            timer += Time.deltaTime;
            yield return null;
        }
        yield return StartCoroutine(sayText("We dare you, dodge the rain!"));

        int projectCount = 40;
        GameObject[] projectiles = new GameObject[projectCount];
        for (int i = 0; i < projectCount; i++)
        {

            float rndx = Random.Range(-9.2f, 11.2f);
            if (rndx > 9.2f)
            {
                rndx = GameObject.Find("pawn").transform.position.x + Random.Range(-0.1f, 0.1f);
            }
            Vector3 rndPos = new Vector3(rndx, gameObject.transform.position.y + 5f,
                gameObject.transform.position.z);
            projectiles[i] = (GameObject)Instantiate(projectile, rndPos, Quaternion.Euler(new Vector3(0f, 0f, 0f)));
            projectiles[i].GetComponent<Projectile>().setAcc(10);
            projectiles[i].GetComponent<Projectile>().onHit = hittedPlayer;
            soundPlayer.PlayOneShot(shootSound);
            StartCoroutine(projectiles[i].GetComponent<Projectile>().FadeIn());
            yield return new WaitForSeconds(0.3f);
        }

        StartCoroutine(GlowOutKing());
        timer = 0f;
        while (timer < flyingUpDur)
        {
            transform.position = Vector3.Lerp(targetPos, origPos, timer / flyingUpDur);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator FireVertical()
    {
        yield return StartCoroutine(sayText("Your actions don't matter."));
        float timer = 0f;
        float flyingUpDur = 2.0f;
        Vector3 origPos = transform.position;
        Vector3 targetPos = origPos;
        targetPos.y = 1.6f;
        StartCoroutine(GlowInKing());
        while (timer < flyingUpDur)
        {
            transform.position = Vector3.Lerp(origPos, targetPos, timer / flyingUpDur);
            timer += Time.deltaTime;
            yield return null;
        }
        yield return StartCoroutine(sayText("You will always be a pawn!"));

        int projectCount = 20;
        GameObject[] projectiles = new GameObject[projectCount];
        float rnd = Random.value;
        for (int i = 0; i < projectCount; i++)
        {
            Vector3 dirAngles;
            if (rnd < 0.5f)
            {
                dirAngles = new Vector3(0, 0, Mathf.Lerp(-70f, 70f, (float)i / (projectCount - 1f)));
            }
            else 
            {
                dirAngles = new Vector3(0, 0, Mathf.Lerp(70f, -70f, (float)i / (projectCount - 1f)));
            }
                
                
            Vector3 dir = new Vector3(Mathf.Sin((180-dirAngles.z) * Mathf.Deg2Rad),
            Mathf.Cos((180 - dirAngles.z) * Mathf.Deg2Rad), 0f);
            projectiles[i] = (GameObject)Instantiate(projectile,
            gameObject.transform.position + dir * 1f , Quaternion.Euler(dirAngles));
            projectiles[i].GetComponent<Projectile>().setAcc(10);
            projectiles[i].GetComponent<Projectile>().onHit = hittedPlayer;
            soundPlayer.PlayOneShot(shootSound);
            StartCoroutine(projectiles[i].GetComponent<Projectile>().FadeIn());
            yield return new WaitForSeconds(0.2f);
        }

        StartCoroutine(GlowOutKing());
        timer = 0f;
        while (timer < flyingUpDur)
        {
            transform.position = Vector3.Lerp(targetPos,origPos, timer / flyingUpDur);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    private void hittedPlayer()
    {
        soundPlayer.PlayOneShot(hitSound);
        StopAllCoroutines();
        StartCoroutine(GameObject.Find("scripts").GetComponent<SceneThird>().WipeScene());
    }

    private void RenderLabel()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(this.gameObject.transform.position);
        Vector2 textSize = styles.label.CalcSize(new GUIContent(bossText));
        Rect labelPos = new Rect(Mathf.Clamp(screenPos.x - textSize.x / 2f, 10f, Screen.width - textSize.x - 5f),
            Screen.height - screenPos.y - 100f, textSize.x, textSize.y);
        GUI.Label(labelPos, bossText);
    }

    public IEnumerator sayText(string text)
    {
        yield return StartCoroutine(FadeInText(text));
        yield return new WaitForSeconds(Mathf.Max(1.0f, WordCount(text) / textFadingInDelay));
        yield return StartCoroutine(FadeOutText(text));
        bossText = "";
    }

    public IEnumerator FadeInKing()
    {
        gameObject.GetComponent<Animator>().Play("king_fadingin");
        yield return new WaitForSeconds(1.87f);
    }

    public IEnumerator GlowInKing()
    {
        gameObject.GetComponent<Animator>().Play("king_glowingin");
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator GlowKing()
    {
        gameObject.GetComponent<Animator>().Play("king_glowing");
        yield return null;
    }



    public IEnumerator GlowOutKing()
    {
        gameObject.GetComponent<Animator>().Play("king_glowingout");
        yield return new WaitForSeconds(1f);
    }

    public IEnumerator FadeOutKing()
    {

        gameObject.GetComponent<Animator>().Play("king_fadingout");
        yield return new WaitForSeconds(1.87f);
    }

    public IEnumerator FadeInText(string text)
    {
        float fadingDuration = 0.5f;
        float timer = 0f;
        while (timer < fadingDuration)
        {
            float alpha = timer / fadingDuration;
            bossText = "<color=#FFFFFF" + HexFromFloat(alpha) + ">" + text + "</color>";
            timer += Time.deltaTime;
            yield return null;
        }
        //bossText = text;
    }

    public IEnumerator FadeOutText(string text)
    {
        float fadingDuration = 0.5f;
        float timer = fadingDuration;
        while (timer > 0 )
        {
            float alpha = timer / fadingDuration;
            bossText = "<color=#FFFFFF" + HexFromFloat(alpha) + ">" + text + "</color>";
            timer -= Time.deltaTime;
            yield return null;
        }
        bossText = text;
    }

    public static string HexFromFloat(float number)
    {
        int h = (int)Mathf.Floor(number * 255);
        return h.ToString("X2").ToLower();
    }

    private float WordCount(string text)
    {
        int wordCount = 0, index = 0;

        while (index < text.Length)
        {
            // check if current char is part of a word
            while (index < text.Length && System.Char.IsWhiteSpace(text[index]) == false)
                index++;

            wordCount++;

            // skip whitespace until next word
            while (index < text.Length && System.Char.IsWhiteSpace(text[index]) == true)
                index++;
        }
        return (float)wordCount;
    }
}
