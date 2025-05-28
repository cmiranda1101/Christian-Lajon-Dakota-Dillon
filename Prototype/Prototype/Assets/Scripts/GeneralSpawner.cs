using UnityEngine;

public class GeneralSpawner : MonoBehaviour
{
    [SerializeField] GameObject objToSpawn;
    [SerializeField] Transform[] Locations;

    [SerializeField] float spawnRate;
    [SerializeField] int amount;

    float timer;
    int count;

    public bool startSpawn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (startSpawn) {
            timer = Time.deltaTime;

            if(timer > spawnRate && count < amount) {
                count++;
                Spawn();
                startSpawn = false;
            }
        }
    }

    void Spawn()
    {
        for(int i = 0; i < Locations.Length; ++i)
        Instantiate(objToSpawn, Locations[i].position, Locations[i].rotation);
        timer = 0;
    }
}
