using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteFader : MonoBehaviour {

    public const float fadingDuration = 1.0f;
    private SpriteRenderer spriteRenderer;
    public bool fadedAtStart = true;

	// Use this for initialization
	void Awake () {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (fadedAtStart)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r,
                  spriteRenderer.color.g,
                  spriteRenderer.color.b,
                  0); 
        }
       
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public IEnumerator FadeOutSprite()
    {
         float timer = fadingDuration;
         while (timer > 0)
         {
             spriteRenderer.color = new Color( spriteRenderer.color.r,
                 spriteRenderer.color.g,
                 spriteRenderer.color.b,
                 timer / fadingDuration);
             timer -= Time.deltaTime;
             yield return null;
         }
    }

    public IEnumerator FadeInSprite()
    {
        float timer = 0f;
        while (timer < fadingDuration)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r,
                spriteRenderer.color.g,
                spriteRenderer.color.b,
                timer / fadingDuration);
            timer += Time.deltaTime;
            yield return null;
        }
    }
}
