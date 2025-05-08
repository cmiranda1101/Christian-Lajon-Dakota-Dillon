using UnityEngine;

public class GunBase : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] int range;
    [SerializeField] int magSize;
    [SerializeField] float fireRate;

    int currentBullets;
    int magCount = 3;
    float shotTimer = 0;


    GameObject enemyHit;
    EnemyAI enemyScript;

    void Start()
    {
        currentBullets = magSize;
    }

    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * range, Color.blue);
        shotTimer += Time.deltaTime;
        if (Input.GetButtonDown("Fire1") && currentBullets > 0 && shotTimer > fireRate)
        {
            Fire();
        }
        if (Input.GetButton("Reload") && currentBullets != magSize && magCount > 0)
        {
            Reload();
        }
    }

    public void Fire()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, range))
        {
            Debug.Log(hit.collider.name);
            if (hit.transform.CompareTag("Enemy"))
            {
                enemyHit = hit.transform.gameObject;
                enemyScript = enemyHit.GetComponent<EnemyAI>();
                enemyScript.takeDamage(damage);
            }
        }
        currentBullets--;
        if (currentBullets <= 0)
        {
            Debug.Log("Out of bullets");
        }
    }

    void Reload()
    {
        currentBullets = magSize;
        magCount--;
        Debug.Log("Reloaded " + magCount + " magazines remaining");
    }

    public void PickUpAmmo()
    {
        magCount++;
    }
}
