using UnityEngine;

public class GunBase : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] int range;
    [SerializeField] int magSize;
    [SerializeField] float fireRate;

    int currentBullets;
    float shotTimer = 0;

    void Start()
    {
        currentBullets = magSize;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * range, Color.blue);
        shotTimer += Time.deltaTime;
        if (Input.GetButtonDown("Fire1") && currentBullets > 0)
        {
            Fire();
        }
        if (Input.GetButton("Reload") && currentBullets != magSize)
        {
            Reload();
        }
    }

    void Fire()
    {
        if (shotTimer > fireRate)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, range))
            {
                Debug.Log(hit.collider.name);
            }
            currentBullets--;
        }
        if (currentBullets <= 0)
        {
            Debug.Log("Out of bullets");
        }
    }

    void Reload()
    {
        currentBullets = magSize;
        Debug.Log("Reloaded");
    }
}
