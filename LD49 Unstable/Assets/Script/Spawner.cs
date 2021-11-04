using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Spawner : MonoBehaviour
{
    public static Spawner spwn;

    public GameObject[] blockPrefabs;
    
    public List<GameObject> spawnedBlocks = new List<GameObject>();

    public float spawnHeight;
    public float highestBlock;

    public float spawnTime;
    public bool spawn = false;

    public int counter;

    public CinemachineVirtualCamera vcam;

    private void Awake()
    {
        spwn = this;
    }

    public void Spawn()
    {
        if (spawn)
        {
            GameObject spawned = Instantiate(blockPrefabs[Random.Range(0, blockPrefabs.Length)]);
            
            spawnedBlocks.Add(spawned);

            transform.position = new Vector3(0, highestBlock / 2, 0);
            spawned.transform.position = new Vector3(0f, spawnHeight + transform.position.y, 0f);

            counter++;
        }
    }

    public void StartGame()
    {
        spawn = true;
        Spawn();
    }
}
