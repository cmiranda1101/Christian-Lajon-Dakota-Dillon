using System.Collections;
using System.Diagnostics.Contracts;
using UnityEngine;

public class GunBase : MonoBehaviour
{
    [SerializeField] GameObject rifle;
    [SerializeField] GameObject pistol;
    [SerializeField] AudioSource pistolSource;
    [SerializeField] AudioSource rifleSource;
    [SerializeField] AudioClip[] pistolShotClips;
    [SerializeField] AudioClip[] rifleShotClips;
    [SerializeField] AudioClip reloadClip1;
    [SerializeField] AudioClip reloadClip2;


    [SerializeField] int damage;
    [SerializeField] int range;
    [SerializeField] int magSize;
    [SerializeField] float fireRate;

    public int currentBullets;
    public int magCount = 3;
    float shotTimer = 0;

    void Start()
    {
        //currentBullets = magSize;
    }

    void Update()
    {

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
        if (Time.timeScale > 0)
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
            StartCoroutine(GameManager.instance.playerScript.MuzzleFlash());
            GunShotSound();
            currentBullets--;
            UpdateAmmo();
            if (currentBullets <= 0)
            {
                Debug.Log("Out of bullets");
            }
        }
    }

    void Reload()
    {
        if (Time.timeScale > 0)
        {
            if (GameManager.instance.playerScript.pistol.activeSelf)
            {
                GameManager.instance.playerScript.pistol.GetComponent<GunBase>().magCount--;
                GameManager.instance.playerScript.pistol.GetComponent<GunBase>().currentBullets = magSize;
            }
            else if (GameManager.instance.playerScript.rifle.activeSelf)
            {
                GameManager.instance.playerScript.rifle.GetComponent<GunBase>().magCount--;
                GameManager.instance.playerScript.rifle.GetComponent<GunBase>().currentBullets = magSize;

            }

            Debug.Log("Reloaded " + magCount + " magazines remaining");

            //if (GameManager.instance.playerScript.pistol.activeSelf)
            //{
            //    pistolSource.clip = reloadClip1;
            //    pistolSource.Play();
            //    pistolSource.clip = reloadClip2;
            //    pistolSource.Play();
            //}
            //else if (GameManager.instance.playerScript.rifle.activeSelf)
            //{
            //    rifleSource.clip = reloadClip1;
            //    rifleSource.Play();
            //}
            StartCoroutine(ReloadGun());
            UpdateAmmo();
        }
    }

    public void PickUpAmmo()
    {
        if (GameManager.instance.playerScript.pistol.activeSelf) {
            GameManager.instance.playerScript.pistol.GetComponent<GunBase>().magCount++;
            GameManager.instance.playerScript.pistol.GetComponent<GunBase>().UpdateAmmo();
        }
        else if (GameManager.instance.playerScript.rifle.activeSelf) {
            GameManager.instance.playerScript.rifle.GetComponent<GunBase>().magCount++;
            GameManager.instance.playerScript.rifle.GetComponent<GunBase>().UpdateAmmo();
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

    public void UpdateAmmo()
    {
        if(GameManager.instance.playerScript.heldWeapon == pistol)
        {
            GameManager.instance.ammoScript.UpdatePistolAmmoAndMagCount();
        } else if (GameManager.instance.playerScript.heldWeapon == rifle)
        {
            GameManager.instance.ammoScript.UpdateRifleAmmoAndMagCount();
        }
    }

    IEnumerator ReloadGun()
    {
        if (GameManager.instance.playerScript.pistol.activeSelf) {
            pistolSource.clip = reloadClip1;
            pistolSource.Play();
            yield return new WaitWhile(() => pistolSource.isPlaying);
            yield return new WaitForSeconds(.2f);
            pistolSource.clip = reloadClip2;
            pistolSource.Play();
        }
        else if (GameManager.instance.playerScript.rifle.activeSelf) {
            rifleSource.clip = reloadClip1;
            rifleSource.Play();
            yield return new WaitWhile(() => rifleSource.isPlaying);
            yield return new WaitForSeconds(.2f);
            rifleSource.clip = reloadClip2;
            rifleSource.Play();
        }
    }
}