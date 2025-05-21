using NUnit.Framework;
using System.Collections;
using System.Diagnostics.Contracts;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GunBase : MonoBehaviour
{
    [SerializeField] GameObject rifle;
    [SerializeField] GameObject pistol;
    [SerializeField] List<GunStats> gunList = new List<GunStats>();
    [SerializeField]AudioSource gunSource;
    AudioClip[] shotClips;
    AudioClip reloadClip1;
    AudioClip reloadClip2;


    [SerializeField] int damage;
    [SerializeField] int range;
    [SerializeField] int magSize;
    [SerializeField] float fireRate;

    public int currentBullets;
    public int magCount = 3;
    float shotTimer = 0;
    int gunListIndex = 0;

    private void Start()
    {
        ChangeGun();
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
        SelectGun();
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
            currentBullets = magSize;
            magCount--;
            gunList[gunListIndex].magCount--;
            Debug.Log("Reloaded " + magCount + " magazines remaining");
            StartCoroutine(ReloadGun());
            UpdateAmmo();
        }
    }

    public void PickUpAmmo()
    {
        magCount++;
        gunList[gunListIndex].magCount++;
        UpdateAmmo();
    }
    public void EquipRifle()
    {
        GameManager.instance.playerScript.rifle = Instantiate(rifle, GameManager.instance.playerScript.rifleSpot.transform.position, GameManager.instance.playerScript.rifleSpot.transform.rotation, GameManager.instance.playerScript.rifleSpot.transform);
        GameManager.instance.hotbarRifle.SetActive(true);
    }

    void GunShotSound()
    {
        int i = Random.Range(0, shotClips.Length);
        gunSource.clip = shotClips[i];
        gunSource.Play();
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
        gunSource.clip = reloadClip1;
        gunSource.Play();
        yield return new WaitWhile(() => gunSource.isPlaying);
        yield return new WaitForSeconds(0.2f);
        gunSource.clip = reloadClip2;
        gunSource.Play();
    }

    void SelectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && gunListIndex < gunList.Count - 1)
        {
            gunListIndex++;
            ChangeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && gunListIndex > 0)
        {
            gunListIndex--;
            ChangeGun();
        }
    }

    void ChangeGun()
    {
        damage = gunList[gunListIndex].damage;
        range = gunList[gunListIndex].range;
        fireRate = gunList[gunListIndex].fireRate;
        currentBullets = gunList[gunListIndex].currentAmmo;
        magSize = gunList[gunListIndex].magSize;
        if (SceneManager.GetActiveScene().name != "Shop" || SceneManager.GetActiveScene().name != "Level2")
        {
            magCount = gunList[gunListIndex].startingMagCount;
            gunList[gunListIndex].magCount = gunList[gunListIndex].startingMagCount;
        }
        else
        {
            magCount = gunList[gunListIndex].magCount;
        }
        shotClips = gunList[gunListIndex].shootSounds;
        reloadClip1 = gunList[gunListIndex].reloadSound1;
        reloadClip2 = gunList[gunListIndex].reloadSound2;
    }

    public void GetGunStats(GunStats _gun)
    {
        gunList.Add(_gun);
        gunListIndex = gunList.Count - 1;
        ChangeGun();
    }
}