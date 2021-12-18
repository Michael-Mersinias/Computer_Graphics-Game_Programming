using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public GameObject ground;

    public GameObject[] items;
    public GameObject opponent;
    private Vector3 spawnValues;
    private float spawnWait;
    public float spawnMostWait;
    public float spawnLeastWait;
    public int startWait;
    public int num;
    private bool spawnOrNot;
    private int randItem;

    private float[] spawnX;
    private float[] spawnZ;

    private int xIndex;
    private int zIndex;
    private int lastX;
    private int lastZ;

    private int temp;
    private float temp2;

    public int difficulty;

    List<float> used = new List<float>();

    // Use this for initialization
    private void Start () {

        // We need to create discrete positions on the plane the spawner is located at.
        // Therefore, we break the Z axis into 5 equal pieces and the X axis into another 5 equal pieces.
        // After doing that, we check if our spawner is located at Z=0 (starting position).
        // In this case, we spawn opponents. In any other case, we spawn obstacles from the items array.

        spawnValues = new Vector3(5, 1, 5);

        spawnX = new float[(int) spawnValues.x];
        spawnZ = new float[(int) spawnValues.z];


        for (int i=0; i< spawnX.Length; i++)
        {
            spawnX[i] = (-(ground.GetComponent<Renderer>().bounds.min.x) + 3f) + (i+1) * ((2*ground.GetComponent<Renderer>().bounds.min.x) / spawnX.Length);
        }

        for (int i = 0; i < spawnZ.Length; i++)
        {
            spawnZ[i] = (-(ground.GetComponent<Renderer>().bounds.min.z) + 5f) + (i + 1) * ((2 * ground.GetComponent<Renderer>().bounds.min.z) / spawnZ.Length);
        }

        if (gameObject.transform.position.z > 0)
        {
            StartCoroutine(waitSpawner());
        }
        else
        {
            StartCoroutine(spawnOpponents());
        }
    }
	
	// Update is called once per frame
	private void Update () {
        spawnWait = Random.Range(spawnLeastWait, spawnMostWait);
	}


    // After a starting delay, start spawning num (public variable) items from the items array, in one of the discrete positions of the plane that we calculated before.
    // We are using the used list, to make sure we do not use the same position twice.
    // The spawning is also stochastic, meaning that:
    // There is a chance equal to temp that the object does indeed spawn, and a chance equal to 1-temp that the spawning of the object is ignored.
    // The chance equals 5-difficulty, with difficulty being able to range from 1 to 3. Difficulty = 3, equals temp = 50%.
    // The difficulty variable of the spawner is strongly recommended to be set to 3, for a nice and challenging game.
    // It should be noted that the difficulty variable here is unrelated to the difficulty variable which affects the speed of the player ball over time.

    private IEnumerator waitSpawner()
    {
        used.Clear();

        yield return new WaitForSeconds(startWait);

        for(int i=0; i<num; i++)
        {
            temp = Random.Range(0, (5-difficulty));

            if (temp == 1)
            {
                spawnOrNot = true;
            }
            else
            {
                spawnOrNot = false;
            }

            if (spawnOrNot==true)
            {
                randItem = Random.Range(0, items.Length);

                lastX = xIndex;
                lastZ = zIndex;

                while (lastX == xIndex)
                {
                    xIndex = Random.Range(0, spawnX.Length);
                }

                while (lastZ == zIndex)
                {
                    zIndex = Random.Range(0, spawnZ.Length);
                }

                temp2 = (xIndex+2) * (zIndex+3);

                if(used.Contains(temp2)==false)
                {
                    used.Add(temp2);

                    Vector3 spawnPosition = new Vector3(spawnX[xIndex], 1f, spawnZ[zIndex]);

                    if(randItem == 5)
                    {
                        spawnPosition = new Vector3(spawnX[xIndex], 1.5f, spawnZ[zIndex]);
                    }

                    Instantiate(items[randItem], spawnPosition + transform.TransformPoint(0, 0, 0), gameObject.transform.rotation); // 2nd: + transform.TransformPoint(0, 0, 0)

                    //yield return new WaitForSeconds(spawnWait);
                }
            }

        }
    }


    // Function that spawn opponents during the first 10 seconds of the game.
    // The number of lines equals to difficulty.
    // The difficulty variable of the spawner is strongly recommended to be set to 3, for a nice and challenging game.
    // With a delay between the spawns, we spawn 3 opponent balls at X = -10, X = 0, X = 10 positions respectively.
    // We also add a small Z offset of 5m for each line.

    private IEnumerator spawnOpponents()
    {

        int num_of_lines = difficulty;

        Vector3 oppPosition;

        for (int i=0; i<num_of_lines; i++)
        {
            oppPosition = new Vector3(-10, 0.5f, (i*+1)*5);
            Instantiate(opponent, oppPosition, gameObject.transform.rotation);

            oppPosition = new Vector3(0, 0.5f, (i+1)*5);
            Instantiate(opponent, oppPosition, gameObject.transform.rotation);

            oppPosition = new Vector3(10, 0.5f, (i+1)*5);
            Instantiate(opponent, oppPosition, gameObject.transform.rotation);

            yield return new WaitForSeconds(startWait);
        }
    }
}
