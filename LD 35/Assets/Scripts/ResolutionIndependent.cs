using UnityEngine;
using System.Collections;

public class ResolutionIndependent : MonoBehaviour {

    public float scale = 3.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        // Source: http://blogs.unity3d.com/2015/06/19/pixel-perfect-2d/
        float artHeight = 1200.0f;
        float wantedSize = (Screen.height / (scale * 100)) * 0.5f * (artHeight / Screen.height);
        Camera.main.orthographicSize = wantedSize;

    }
}
