using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Control : MonoBehaviour {

    public Text positionText;
    private int position;

    private GameObject[] enemies;
    private GameObject[] enemies2;

    public float speed_H;
    public float speed_V;
    public float speed_B;
    public float top_speed_V;
    public float start_delay;
    private Rigidbody body;
    private Vector3 movement_V;
    private Vector3 height_fix;

    private int num_of_opponents;

    private bool flag;

    // Use this for initialization
    void Start () {
        flag = false;   // When false, do not move. When true, keep moving.

        body = GetComponent<Rigidbody>();
        movement_V = new Vector3(0.0f, 0.0f, 1.0f);
        height_fix = new Vector3(0.0f, -0.1f, 0.0f);

        position = 0;
        num_of_opponents = 0;
        SetPositionText();

        StartCoroutine(startDelay());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        // If we move and the song is not being played, play the song attached to this gameobject.
        //


        if (flag == true)
        {

            // If we move and the song is not being played, play the song attached to this gameobject.

            if (!gameObject.GetComponent<AudioSource>().isPlaying)
            {
                gameObject.GetComponent<AudioSource>().Play();
            }


            // Constantly apply a force to move forward.
            // Move horizontally based on user input.

            float moveH = Input.GetAxis("Horizontal");

            Vector3 movement_H = new Vector3(moveH, 0.0f, 0.0f);

            movement_H = movement_H * body.velocity.magnitude;
            body.AddForce(movement_H * speed_H); // * Time.deltaTime);

            if (body.velocity.magnitude < top_speed_V)
            {
                body.AddForce(movement_V * speed_V); // * Time.deltaTime);
            }
            //print(body.velocity.z);


            // Jump

            if (Input.GetKeyDown("space") && body.position.y < 1)
            {
                Vector3 jump = new Vector3(0.0f, 600.0f, -10.0f);
                body.AddForce(jump);
            }


            // And land again quickly

            if (body.position.y > 1)
            {
                height_fix = new Vector3(0.0f, (-body.position.y), 0.0f);
                body.AddForce(height_fix);
            }


            // Call this function for position ranking.
            
            calculatePos();
        }
    }


    // If collided with a booster, get a speed boost and play a boosting speed sound.
    // Destroy the booster after 2 seconds.

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Booster"))
        {
            StartCoroutine(speedBoost(other));
        }

    }

    private IEnumerator speedBoost(Collider other)
    {
        other.GetComponent<AudioSource>().Play();

        body.AddForce(movement_V * speed_B);
        yield return new WaitForSeconds(2.0f);
        Destroy(other.gameObject);
    }


    // Function to set our position ranking as text.

    private void SetPositionText()
    {
        positionText.text = "Position: " + position.ToString() + " / " + num_of_opponents.ToString();
    }

    public void gameOver()
    {
        gameObject.GetComponent<AudioSource>().Stop();

        speed_V = 0;
        speed_H = 0;
        top_speed_V = 0;

        GetComponent<Ranking>().endGame();

        body.isKinematic = true;
        Destroy(gameObject);
    }


    // Start moving the player ball after some time has passed, so opponents have an advantage.

    private IEnumerator startDelay()
    {
        yield return new WaitForSeconds(start_delay);
        flag = true;
    }


    // Function to set the maximum speed of the player ball, which is changed for every difficulty level.

    public void setMaxSpeed(int difficulty)
    {
        top_speed_V = (top_speed_V-10) + (10* difficulty);
    }


    // Get player ball position on the Z axis.

    public float zPosition()
    {
        return gameObject.transform.position.z;
    }


    // Find all opponents (through a tag) and compare their Z position to that of the player ball.
    // Update position ranking accordingly.

    public void calculatePos()
    {
        position = 1;

        enemies = GameObject.FindGameObjectsWithTag("Opponent");

        for (int i=0; i<enemies.Length; i++)
        {
            if (zPosition() < enemies[i].GetComponent<Ball_Script>().zPosition())
            {
                position = position + 1;
            }

        }

        enemies2 = GameObject.FindGameObjectsWithTag("Smart_Opponent");

        for (int i = 0; i < enemies2.Length; i++)
        {
            if (zPosition() < enemies2[i].GetComponent<Smart_Balls>().zPosition())
            {
                position = position + 1;
            }

        }

        num_of_opponents = enemies.Length + enemies2.Length + 1;

        SetPositionText();
    }

}
