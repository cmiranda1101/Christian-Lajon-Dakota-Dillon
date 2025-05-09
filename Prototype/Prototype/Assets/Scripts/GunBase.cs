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

    void Start()
    {
        currentBullets = magSize;
    }

    void Update()
    {
        if(GameManager.instance.isPaused)
        {
            return;
        }

        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * range, Color.blue);
        shotTimer += Time.deltaTime;
        if (Input.GetButtonDown("Fire1") && currentBullets > 0 && shotTimer > fireRate)
        {
            Fire();
        }
        if (Input.GetButtonDown("Reload") && currentBullets != magSize && magCount > 0)
        {
            Reload();
        }
    }

    public void Fire()
    {
        RaycastHit hit;
        shotTimer = 0;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, range))
        {
            Debug.Log(hit.collider.name);
            IDamage damaged = hit.collider.GetComponent<IDamage>();
            if (damaged != null)
            {
                damaged.takeDamage(damage);
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
