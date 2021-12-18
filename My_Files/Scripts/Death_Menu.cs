using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Death_Menu : MonoBehaviour {

    public Text scoreText;
    private Image img;

    private bool isShown;
    private float transition;

	// Use this for initialization
	void Start () {

        // Death screen is inactive when the game starts.
        gameObject.SetActive(false);

        img = gameObject.GetComponent<Image>();
        isShown = false;

        transition = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if(!isShown)
        {
            return;
        }

        // Slowly fade out from game screen to death screen.

        transition = transition + (Time.deltaTime/2);
        img.color = Color.Lerp(new Color(0,0,0,0), Color.black, transition);

	}


    // Activate the death screen and write the final score.

    public void popDeathMenu(float score)
    {
        gameObject.SetActive(true);
        scoreText.text = "Game over!\nScore: " + ((int)score).ToString() + " points";
        isShown = true;
    }


    // Button functionality.

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // SceneManager.LoadScene("my_world7");
    }

    public void ToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
