using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Control : MonoBehaviour {

    public GameObject player;

    private Vector3 offset;

	// Use this for initialization
	void Start () {
        
        // Starting distance between camera and the player

        offset = transform.position - player.transform.position;
	}

    // Update is called once per frame
    void LateUpdate () {

        // Camera keeps following the player. 
        // Particle System is also attached to the camera as a child, so it follows the player as well, creating a flame trail effect.

        transform.position = player.transform.position + offset;

    }

}
