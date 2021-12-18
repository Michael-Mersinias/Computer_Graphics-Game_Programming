using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Booster_Script : MonoBehaviour {

    public float interval;

    // Use this for initialization
    void Start () {
        // Make the desired transformations and rotations.

        transform.Rotate(0, 90, 0);
        transform.Translate(0f, -1.15f, 0f);

        StartCoroutine(destroyObject());
    }
	
	// Update is called once per frame
	void Update () {


    }

    // Destroy the gameobject after 1 minute.

    private IEnumerator destroyObject()
    {
        yield return new WaitForSeconds(60);
        Destroy(gameObject);
    }

}
