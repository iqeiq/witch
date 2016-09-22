using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;


public class GameManager : MonoBehaviour {

    public int score = 0;

	// Use this for initialization
	void Start () {
        score = 0;
	}
	
	// Update is called once per frame
	void Update () {

        // TODO:
        if (Input.GetButtonDown("Menu"))
        {
            SceneManager.LoadScene("Title");
        }
    }
}
