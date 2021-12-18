using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ranking : MonoBehaviour {

    private float score = 11.0f;
    public Text scoreText;
    public Death_Menu deathMenu;

    private GameObject[] smartBall;

    private int difficulty;
    private int threshold;
    private int max_difficulty;
    private bool flag;
    private bool end_flag;

    // Use this for initialization
    void Start () {
        difficulty = 1;
        threshold = 90;
        max_difficulty = 3;
        flag = false;
        end_flag = false;
	}
	
	// Update is called once per frame
	void Update () {

        // Countdown from 10 to 0, display Go!, and then add up the score.
        // Also increase difficulty every minute, and increase player ball's speed for every difficulty increase.
        // Also by finding its tag in the world, we increase the white ball's speed for every difficulty level increase, but NOT the speed of the other balls.
        // Maximum difficulty level is set to 3.

        if(end_flag == true)
        {
            return;
        }

        if(flag==false && score<0) {
            flag = true;
        }

        if(flag==true)
        {
            score = score + Time.deltaTime * 10 * difficulty;
            scoreText.text = ((int)score).ToString();
            if (score > threshold * 10 * difficulty && difficulty < max_difficulty)
            {
                difficulty = difficulty + 1;
                GetComponent<Player_Control>().setMaxSpeed(difficulty);

                smartBall = GameObject.FindGameObjectsWithTag("Smart_Opponent");

                for (int i = 0; i < smartBall.Length; i++)
                {
                    smartBall[i].GetComponent<Smart_Balls>().setMaxSpeed(difficulty);
                }

            }
        }
        else if (flag==false)
        {
            score = score - Time.deltaTime;
            scoreText.text = ((int)score).ToString();
            if(score<=0.5)
            {
                scoreText.text = "Go!";
            }
        }
	}


    // Our current difficulty.

    public int getDifficulty()
    {
        return difficulty;
    }


    // Game over, save highscore if we achieved one.

    public void endGame()
    {
        end_flag = true;

        if(PlayerPrefs.GetFloat("Highscore") < score)
        {
            PlayerPrefs.SetFloat("Highscore", score);
        }

        deathMenu.popDeathMenu(score);
    }
}
