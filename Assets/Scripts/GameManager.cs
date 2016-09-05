using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;


public class GameManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
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
