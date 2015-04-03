using UnityEngine;
using System.Collections;

public class SceneZero : MonoBehaviour {

    void Update()
    {
        if (Input.anyKeyDown)
        {
            StartCoroutine(KeyPressed());
        }

    }


    private IEnumerator KeyPressed()
    {
        yield return  StartCoroutine(GameObject.Find("title").GetComponent<SpriteFader>().FadeOutSprite());
        Application.LoadLevel("level1");
    }
}
