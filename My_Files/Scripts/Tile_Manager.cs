using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile_Manager : MonoBehaviour {

    public GameObject[] tilePrefabs;

    private Transform playerTransform;
    private float spawnZ;
    private float tileLength;
    private float safezone;
    private int numTilesOnScreen;
    private int lastPrefabIndex;

    private List<GameObject> activeTiles;

	// Use this for initialization
	private void Start () {
        spawnZ = 0f;
        tileLength = 150.0f;
        numTilesOnScreen = 7;
        safezone = tileLength*3f;
        lastPrefabIndex = 0;
        activeTiles = new List<GameObject>();

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // Spawn numTilesOnScreen prefabs of 0: Ground and 1: Spawner

        for (int i=0; i< numTilesOnScreen; i++)
        {
            SpawnTile(0);
            if(i<(numTilesOnScreen-1))
            {
                SpawnTile(1);
            }
        }
	}

    // Update is called once per frame
    private void Update()
    {
        // Spawn new prefabs, and delete old ones, of 0: Ground and 1: Spawner.
        // Do that based on the position of the player ball on the Z axis.

        if (playerTransform.position.z - safezone > (spawnZ - numTilesOnScreen * tileLength))
        {
            SpawnTile(0);
            SpawnTile(1);
            DeleteTile();
            //DeleteTile();
        }
    }

    // Spawn tile and increase spawn distance accordingly.
    // Also give the option to take a prefab argument, so we can choose what we spawn from our tilePrefabs array.
    // Otherwise, spawn a random prefab element of the tilePrefabs array.

    private void SpawnTile(int prefabIndex = -1)
    {
        GameObject go;

        if(prefabIndex == -1)
        {
            go = Instantiate(tilePrefabs[RandomPrefabIndex()]) as GameObject;
        }
        else
        {
            go = Instantiate(tilePrefabs[prefabIndex]) as GameObject;
        }

        go.transform.SetParent(transform);
        go.transform.position = Vector3.forward * spawnZ;

        if(prefabIndex == 0)
        {
            spawnZ = spawnZ + tileLength;
        }

        activeTiles.Add(go);
    }


    // Delete tile by destroying the gameobject and also removing it from the activeTiles list.

    private void DeleteTile()
    {
        Destroy(activeTiles[1]);
        activeTiles.RemoveAt(1);
        Destroy(activeTiles[0]);
        activeTiles.RemoveAt(0);
    }


    // Function to randomly select a prefab element from the tilePrefabs array. Inactive.

    private int RandomPrefabIndex()
    {
        if(tilePrefabs.Length <= 1)
        {
            return 0;
        }

        int randomIndex = lastPrefabIndex;
        while(randomIndex == lastPrefabIndex)
        {
            randomIndex = Random.Range(0, tilePrefabs.Length);
        }

        lastPrefabIndex = randomIndex;
        return randomIndex;
    }

}
