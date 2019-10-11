using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Health : MonoBehaviour
{
    public int HP { get; private set; }

    private List<Animator> Hearts;

    // Use this for initialization
    void Start()
    {
        HP = transform.childCount;
        Hearts = new List<Animator>();
        for(var i = 0; i < transform.childCount; i++)
        {
            Hearts.Add(transform.GetChild(i).GetChild(0).GetComponent<Animator>());
        }
    }

    public void BrakeHeart()
    {
        HP -= 1;
        Hearts[HP].SetBool("Broken", true);
    }

    public void Heal()
    {
        HP = Hearts.Count;
        foreach(var heart in Hearts)
        {
            heart.SetBool("Broken", false);
        }
    }
}
