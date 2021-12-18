using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Static_Obstacle_Script : MonoBehaviour {

    private float original_step;
    private float step;
    private int counter;

	// Use this for initialization
	void Start () {
        original_step = 0.05f;
        step = 1;
        counter = 0;

        StartCoroutine(destroyObject());
    }
	
	// Update is called once per frame
	void Update () {

        // Scale the gameobject. Every 50 frames change the way it scales.
        // As a result, it will approximately scale 1m bigger and 1m smaller linearly.

        counter = counter + 1;

        if (counter>=50)
        {
            step = -original_step;
        }
        
        if(counter<50)
        {
            step = original_step;
        }

        if(counter==100)
        {
            counter = 0;
        }

        
        transform.localScale = transform.localScale + new Vector3(step, 0, 0);

    }


    // Destroy the collided gameobject if it's a ball, a smart ball or a player ball.

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Player"))
        {
            gameObject.GetComponent<AudioSource>().Play();
            other.GetComponent<Player_Control>().gameOver();
            //print("KILL");
        }

        if (other.name.Contains("Ball") && (transform.position.z > (other.transform.position.z + 0.1f)))
        {
            //print("KILL");
            //gameObject.GetComponent<AudioSource>().Play();
            other.GetComponent<Ball_Script>().gameOver();
        }
    }


    // Destroy the gameobject after 1 minute.

    private IEnumerator destroyObject()
    {
        yield return new WaitForSeconds(60);
        Destroy(gameObject);
    }
}
