using NUnit.Framework;
using System.Collections;
using System.Diagnostics.Contracts;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GunBase : MonoBehaviour
{
    [SerializeField] public List<GunStats> gunList = new List<GunStats>();
    [SerializeField] GameObject gunModel;
    [SerializeField]AudioSource gunSource;
    [SerializeField] AudioClip emptyClip;
    AudioClip[] shotClips;
    AudioClip reloadClip1;
    AudioClip reloadClip2;


    [SerializeField] int damage;
    [SerializeField] float fireRate;
    [SerializeField] int range;
    [SerializeField] public int currentAmmo;
    [SerializeField] int magSize;
    [SerializeField] public int startingMagCount;
    [SerializeField] public int magCount;

    bool isReloading = false;

    float shotTimer = 0;
    public int gunListIndex = 0;

    private void Start()
    {
        ChangeGun();
        SaveAmmoState();
    }

    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * range, Color.blue);
        shotTimer += Time.deltaTime;
        if (Input.GetButtonDown("Fire1") && currentAmmo > 0 && shotTimer > fireRate && !isReloading)
        {
            Fire();
            UpdateAmmo();
        } 
        else if (Input.GetButtonDown("Fire1") && currentAmmo <= 0)
        {
            AudioManager.PlaySFX(gunSource, emptyClip);
        }
        if (Input.GetButtonDown("Reload") && currentAmmo != magSize && magCount > 0 && !isReloading)
        {
            StartCoroutine(Reload());
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
                if (hit.collider.name == "CritSpot")
                {
                    IDamage critical = hit.collider.GetComponentInParent<IDamage>();
                    if(critical != null)
                    {
                        critical.takeDamage(damage * 2);
                    }
                }
                else if (damaged != null)
                {
                    damaged.takeDamage(damage);
                }
            }
            StartCoroutine(GameManager.instance.playerScript.MuzzleFlash());
            GunShotSound();
            currentAmmo--;
            gunList[gunListIndex].currentAmmo--;
            UpdateAmmo();
            if (currentAmmo <= 0)
            {
                Debug.Log("Out of bullets");
            }
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        AudioManager.PlaySFX(gunSource, reloadClip1);
        yield return new WaitForSeconds(.3f);
        AudioManager.PlaySFX(gunSource, reloadClip2);
        yield return new WaitForSeconds(.3f);
        currentAmmo = magSize;
        magCount--;
        gunList[gunListIndex].currentAmmo = magSize;
        gunList[gunListIndex].magCount--;

        UpdateAmmo();
        isReloading = false;
    }

    public void PickUpAmmo()
    {
        magCount++;
        gunList[gunListIndex].magCount++;
        UpdateAmmo();
    }

    void GunShotSound()
    {
        int i = Random.Range(0, shotClips.Length);
        AudioManager.PlaySFX(gunSource, shotClips[i]);
    }

    public void UpdateAmmo()
    {
        GameManager.instance.ammoScript.UpdateAmmoAndMagCount();
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
        magSize = gunList[gunListIndex].magSize;
        if (SceneManager.GetActiveScene().name == "IntroLevel")
        {
            currentAmmo = gunList[gunListIndex].magSize;
            magCount = gunList[gunListIndex].startingMagCount;
            gunList[gunListIndex].currentAmmo = gunList[gunListIndex].magSize;
            gunList[gunListIndex].magCount = gunList[gunListIndex].startingMagCount;
        }
        else if (SceneManager.GetActiveScene().name == "Tutorial")
        {
            currentAmmo = 0;
            magCount = 0;
            gunList[gunListIndex].currentAmmo = 0;
            gunList[gunListIndex].magCount = 0;
        }
        else
        {
            magCount = gunList[gunListIndex].magCount;
            currentAmmo = gunList[gunListIndex].currentAmmo;
        }
        shotClips = gunList[gunListIndex].shootSounds;
        reloadClip1 = gunList[gunListIndex].reloadSound1;
        reloadClip2 = gunList[gunListIndex].reloadSound2;
        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[gunListIndex].model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[gunListIndex].model.GetComponent<MeshRenderer>().sharedMaterial;
        UpdateAmmo();
    }

    public void GetGunStats(GunStats _gun)
    {
        gunList.Add(_gun);
        gunListIndex = gunList.Count - 1;
        ChangeGun();
    }

    public void SaveAmmoState()
    {
        gunList[gunListIndex].levelStartCurrentAmmo = currentAmmo;
        gunList[gunListIndex].levelStartmagCount = magCount;
    }
}