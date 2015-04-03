using UnityEngine;
using System.Collections;


public class scenethirdsave : MonoBehaviour {

    public bool playedAlready = false;


	void Awake () {
        DontDestroyOnLoad(this);
	}
	
}
