using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
   
    // Public GameObjects for each enemy type 
    public GameObject meleePrefab;  
    public GameObject pistolPrefab; 
    public GameObject riflePrefab;  

   
   
    public Transform spawnPoint;  // Position where enemies will spawn

    
    public float spawnInterval = 2f;  //  to control spawn rate

    private float timer;  

    void Update()
    {
       
        timer += Time.deltaTime;

        // If the timer exceeds the spawn interval, spawn an enemy
        if (timer >= spawnInterval)
        {
            SpawnEnemy();  // Call SpawnEnemy function to spawn an enemy
            timer = 0f;    // Reset the timer
        }
    }

    void SpawnEnemy()
    {
        // Generate a random float between 0 and 1
        float rand = Random.Range(0f, 1f);

        // Variable to hold the selected prefab to spawn
        GameObject prefabToSpawn;

        // 50% chance to spawn Melee
        if (rand < 0.5f)
        {
            prefabToSpawn = meleePrefab;
        }
        // 30% chance to spawn Pistol (next range: 0.5 - 0.8)
        else if (rand < 0.8f)
        {
            prefabToSpawn = pistolPrefab;
        }
        // 20% chance to spawn Rifle (range: 0.8 - 1.0)
        else
        {
            prefabToSpawn = riflePrefab;
        }

        // Instantiate the selected enemy prefab at the spawn location
        Instantiate(prefabToSpawn, spawnPoint.position, Quaternion.identity);
    }
}
