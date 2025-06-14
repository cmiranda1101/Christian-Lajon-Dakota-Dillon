using System.Collections;
using UnityEngine;

public class LurkerSpawn : MonoBehaviour
{
    [SerializeField] GameObject lurker;
    [SerializeField] GameObject spawnPos;
    [SerializeField] float spawnDelay;
    GameObject player;

    private void Start()
    {
        player = GameManager.instance.player;
    }

    private void Update()
    {
        spawnPos.transform.LookAt(player.transform);   
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(SpawnLurker());
        }
    }

    IEnumerator SpawnLurker()
    {
        yield return new WaitForSeconds(spawnDelay);
        Instantiate(lurker, spawnPos.transform.position, spawnPos.transform.rotation);
        Destroy(gameObject);
    }
}
