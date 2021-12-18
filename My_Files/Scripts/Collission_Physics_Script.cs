using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collission_Physics_Script : MonoBehaviour {

	// Use this for initialization
	void Start () {

        // Make the desired transformations and rotations, depending on the gameobject this script is attached to.

        if (gameObject.name.Contains("Jumper"))
        {
            transform.Rotate(-45, 0, 0);
            transform.Translate(0f, -0.4f, 0f);
            gameObject.GetComponent<BoxCollider>().isTrigger = false;
            StartCoroutine(destroyObject());
        }
        else if(gameObject.name.Contains("Table"))
        {
            transform.Rotate(-90, 0, -90);
            StartCoroutine(destroyObject());
        }
        else if(gameObject.name.Contains("Machine"))
        {
            transform.Rotate(-79, 0, 180);
            StartCoroutine(destroyObject());
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    // If the gameobject is lethal (aka an obstacle, not a wall or a jumper), then destroy the collided gameobject if it's a ball, a smart ball or a player ball.

    private void OnTriggerEnter(Collider other)
    {

        if (other.name.Contains("Player"))
        {
            //print("KILL");
            gameObject.GetComponent<AudioSource>().Play();
            other.GetComponent<Player_Control>().gameOver();
        }

        if (other.name.Contains("Ball") && (transform.position.z > (other.transform.position.z + 0.1f)) && (gameObject.name.Contains("Table") || gameObject.name.Contains("Machine")))
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
