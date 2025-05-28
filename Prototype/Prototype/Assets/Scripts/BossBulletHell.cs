using UnityEngine;
using UnityEngine.UIElements;

public class BossBulletHell : MonoBehaviour
{
    [SerializeField] Transform spawnLocation;
    [SerializeField] Transform spawnLocation2;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float fireRate;    //This is for the bullet aimed at player;

    Vector3 playerDir;

    float fireTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.isPaused) {
            fireTimer += Time.deltaTime;

            if (!GameManager.instance.heartBossScript.isShielded) {
                if (fireRate <= fireTimer) {
                    shootAtPlayer();
                }
            }
            SpawnBullet();
        }
    }

    void SpawnBullet()
    {
        Vector3 temp = Random.onUnitSphere;
        Instantiate(bulletPrefab, spawnLocation.position, Quaternion.LookRotation(temp));
    }
    void shootAtPlayer()
    {
        fireTimer = 0;
        Vector3 temp = (GameManager.instance.player.transform.position - spawnLocation2.position);
        Instantiate(bulletPrefab, spawnLocation2.position, Quaternion.LookRotation(temp));
    }
}
