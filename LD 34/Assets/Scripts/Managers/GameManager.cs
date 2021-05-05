using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    private static GameManager _instance;   

    public static GameManager Instance
    {
        get
        {

            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameManager>();
            }
            return _instance;
        }
    }

    void Awake()
    {
        _instance = this;
        Cursor.lockState = CursorLockMode.Locked;
    }


}
