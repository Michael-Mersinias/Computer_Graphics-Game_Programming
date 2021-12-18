using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_Script : MonoBehaviour {

    public float speed;
    private int direction;
    private Rigidbody body;
    private int count_time;

    // Use this for initialization
    void Start () {
        direction = 1;
        body = GetComponent<Rigidbody>();

        count_time = 0;

    }
	
	// Update is called once per frame
	void Update ()
    {
        // Destroy the object after 1 minute or if it somehow ignores collision with walls.

        count_time = count_time + 1;

        if (transform.position.x >= 17 || transform.position.x <= -17 || count_time > 3000)
        {
            Destroy(gameObject);
        }

        // Translate the gameobject by a small margin towards a certain direction (1 for right, -1 for left). 

        transform.Translate(Vector3.right * speed * direction);
    }


    // If it collides with a non-ball object, change the direction accordingly.
    // Destroy the collided gameobject if it's a ball, a smart ball or a player ball.

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("West"))
        {
            direction = 1;
        }

        if (other.gameObject.name.Contains("East"))
        {
            direction = -1;
        }

        if (other.gameObject.name.Contains("Obstacle") || other.gameObject.name.Contains("Booster"))
        {
            direction = -direction;
        }

        if (other.name.Contains("Player"))
        {
            //print("KILL");
            //speed = 0;
            gameObject.GetComponent<AudioSource>().Play();
            other.GetComponent<Player_Control>().gameOver();
        }

        if (other.name.Contains("Ball") && (transform.position.z > (other.transform.position.z + 0.1f)))
        {
            //print("KILL");
            //speed = 0;
            //gameObject.GetComponent<AudioSource>().Play();
            other.GetComponent<Ball_Script>().gameOver();
        }
    }


    // Destroy the gameobject after 1 minute. Not used.

    private IEnumerator destroyObject()
    {
        yield return new WaitForSeconds(60);
        Destroy(gameObject);
    }

}
