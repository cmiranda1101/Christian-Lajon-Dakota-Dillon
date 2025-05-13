using UnityEngine;

public class GunBase : MonoBehaviour
{
    [SerializeField] GameObject rifle;
    [SerializeField] GameObject pistol;
    [SerializeField] AudioSource pistolSource;
    [SerializeField] AudioSource rifleSource;
    [SerializeField] AudioClip[] pistolShotClips;
    [SerializeField] AudioClip[] rifleShotClips;
    [SerializeField] AudioClip reloadClip;


    [SerializeField] int damage;
    [SerializeField] int range;
    [SerializeField] int magSize;
    [SerializeField] float fireRate;

    int currentBullets;
    public int magCount = 3;
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
            StartCoroutine(GameManager.instance.playerScript.MuzzleFlash());
            GunShotSound();
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
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, range, ~GameManager.instance.playerScript.ignoreLayer, QueryTriggerInteraction.Ignore)) 
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
        if (GameManager.instance.playerScript.pistol.activeSelf) {
            GameManager.instance.playerScript.pistol.GetComponent<GunBase>().magCount--;
        }
        else if (GameManager.instance.playerScript.rifle.activeSelf) {
            GameManager.instance.playerScript.rifle.GetComponent<GunBase>().magCount--;
        }

        Debug.Log("Reloaded " + magCount + " magazines remaining");

        if (GameManager.instance.playerScript.pistol.activeSelf) {
            pistolSource.clip = reloadClip;
            pistolSource.Play();
        }
        else if (GameManager.instance.playerScript.rifle.activeSelf) {
            rifleSource.clip = reloadClip;
            rifleSource.Play();
        }


    }

    public void PickUpAmmo()
    {
        if (GameManager.instance.playerScript.pistol.activeSelf) {
            GameManager.instance.playerScript.pistol.GetComponent<GunBase>().magCount++;
        }
        else if (GameManager.instance.playerScript.rifle.activeSelf) {
            GameManager.instance.playerScript.rifle.GetComponent<GunBase>().magCount++;
        }
    }
    public void EquipRifle()
    {
        GameManager.instance.playerScript.rifle = Instantiate(rifle, GameManager.instance.playerScript.rifleSpot.transform.position, GameManager.instance.playerScript.rifleSpot.transform.rotation, GameManager.instance.playerScript.rifleSpot.transform);
        GameManager.instance.hotbarRifle.SetActive(true);
    }

    void GunShotSound()
    {
        if (GameManager.instance.playerScript.pistol.activeSelf) {
            int i = Random.Range(0, pistolShotClips.Length);
            pistolSource.clip = pistolShotClips[i];
            pistolSource.Play();
        }
        else if (GameManager.instance.playerScript.rifle.activeSelf) {
            int j = Random.Range(0, rifleShotClips.Length);
            rifleSource.clip = rifleShotClips[j];
            rifleSource.Play();
        }
    }
}