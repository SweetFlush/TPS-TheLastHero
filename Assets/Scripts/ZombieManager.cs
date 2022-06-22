using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieManager : MonoBehaviour
{
    public GameManager gameManager;

    public float timer = 0f;
    public float spawnTime = 7f;

    //public int spawnNumber = 10;

    public Transform[] SpawnPoints;

    public GameObject[] zombies;

    public int level = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timer >= spawnTime)
            SpawnMobs();

        timer += Time.deltaTime;
    }

    public void SpawnMobs()
    {
        timer = 0;
        if (gameManager.currentZombies >= gameManager.maxZombies) return;
        
        switch(level)
        {
            case 0:
                SpawnAtBus(5);
                break;
            case 1:
                SpawnAtBus(20);
                SpawnAtCities(1);
                break;
            case 2:
                SpawnAtBus(30);
                SpawnAtCities(3);
                break;
            case 3:
                SpawnAtBus(30);
                SpawnAtCities(20);
                break;
        }
    }

    private void SpawnAtBus(int spawnNumber)
    {
        gameManager.currentZombies += spawnNumber;
        for (int i = 0; i < spawnNumber; i++)
        {
            int gender = Random.Range(0, 2);
            Instantiate(zombies[gender], SpawnPoints[i % 5].position, Quaternion.identity);
        }
    }

    private void SpawnAtCities(int spawnNumber)
    {
        gameManager.currentZombies += spawnNumber;
        for (int i = 0; i < spawnNumber; i++)
        {
            int gender = Random.Range(0, 2);
            Instantiate(zombies[gender], SpawnPoints[(i % 5) + 5].position, Quaternion.identity);
        }
    }
}
