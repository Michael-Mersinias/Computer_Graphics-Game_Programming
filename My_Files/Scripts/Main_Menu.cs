using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Main_Menu : MonoBehaviour {

    public Text highscoreText;

	// Use this for initialization
	void Start () {

        // Display the highscore. Attached song will also start playing (its play on awake and loop options are enabled).

        highscoreText.text = "Highscore : " + ((int)PlayerPrefs.GetFloat("Highscore")).ToString();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Button functionalities.

    public void NewGame()
    {
        SceneManager.LoadScene("my_world7");
    }

    public void ExitGame()
    {
        Debug.Log("Thanks for playing!");
        Application.Quit();
    }
}
