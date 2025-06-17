using System.Collections;
using UnityEngine;

public class ThrowConsumable : MonoBehaviour
{
    public enum GrenadeType {Molotov, Frag}
    public GrenadeType currentType;

    public AudioSource throwingAudioSource;
    public Transform throwPoint;
    public float throwForce;

    public GameObject chemlightPrefab;
    public int chemlightCount;
    public int chemlightDuration;

    public GameObject molotovPrefab;
    public int molotovCount;

    public GameObject grenadePrefab;
    public GameObject grenadeExplosionPrefab;
    public AudioClip grenadeExplosionClip;
    public AudioClip grenadeThrowClip;
    public float grenadeFuze;
    public int grenadeCount;

    void Start()
    {
        GameManager.instance.chemlightCounter.text = chemlightCount.ToString();
        GameManager.instance.molotovCounter.text = molotovCount.ToString();
    }

    void Update()
    {
        //PlayerController script inputs will need to be deleted to avoid running these functions twice

        if (Input.GetButtonDown("Throw Chemlight"))
        {
            ThrowChemlight();
        }
        if (Input.GetButtonDown("Change Throwable"))
        {
            ChangeThrowable();
        }
        if (Input.GetButtonDown("Throw Molotov") && currentType == GrenadeType.Molotov)
        {
            ThrowMolotov();
        } 
        else if (Input.GetButtonDown("Throw Molotov") && currentType == GrenadeType.Frag)
        {
            ThrowGrenade();
        }
    }
    public void ChangeThrowable()
    {
        if (currentType == GrenadeType.Molotov)
        {
            if (grenadeCount > 0)
            {
                GameManager.instance.MolotovUI.SetActive(false);
                currentType = GrenadeType.Frag;
                GameManager.instance.GrenadeUI.SetActive(true);
            }
        } 
        else if (currentType == GrenadeType.Frag)
        {
            if(molotovCount > 0)
            {
                GameManager.instance.GrenadeUI.SetActive(false);
                currentType = GrenadeType.Molotov;
                GameManager.instance.MolotovUI.SetActive(true);
            }
        }
    }
    public void ThrowChemlight()
    {
        if(chemlightCount > 0)
        {
            GameObject chemlight = Instantiate(chemlightPrefab, throwPoint.position, throwPoint.rotation);
            Rigidbody rb = chemlight.GetComponent<Rigidbody>();
            rb.AddForce(throwPoint.forward * throwForce, ForceMode.Impulse);
            StartCoroutine(DestroyChemlight(chemlight));
            chemlightCount--;
            GameManager.instance.chemlightCounter.text = chemlightCount.ToString();
        }
    }

    IEnumerator DestroyChemlight(GameObject chemlight)
    {
        yield return new WaitForSeconds(chemlightDuration);
        Destroy(chemlight);
    }

    public void ThrowMolotov()
    {
        if (molotovCount > 0)
        {
            GameObject molotov = Instantiate(molotovPrefab, throwPoint.position, throwPoint.rotation);
            Rigidbody rb = molotov.GetComponent<Rigidbody>();
            rb.AddForce(throwPoint.forward * throwForce, ForceMode.Impulse);
            molotovCount--;
            GameManager.instance.molotovCounter.text = molotovCount.ToString();
        }
        if (molotovCount == 0)
        {
            GameManager.instance.MolotovUI.SetActive(false);
            if (grenadeCount > 0)
            {
                currentType = GrenadeType.Frag;
                GameManager.instance.GrenadeUI.SetActive(true);
            }
        }
    }

    public void ThrowGrenade()
    {
        if (grenadeCount > 0)
        {
            GameObject grenade = Instantiate(grenadePrefab, throwPoint.position, throwPoint.rotation);
            Rigidbody rb = grenade.GetComponent<Rigidbody>();
            rb.AddForce(throwPoint.forward * throwForce * 2, ForceMode.Impulse);
            AudioManager.PlaySFX(throwingAudioSource, grenadeThrowClip);
            grenadeCount--;
            GameManager.instance.grenadeCounter.text = grenadeCount.ToString();
            StartCoroutine(GrenadeExplosion(grenade));
        }
        if (grenadeCount == 0)
        {
            GameManager.instance.GrenadeUI.SetActive(false);
            if (molotovCount > 0)
            {
                currentType = GrenadeType.Molotov;
                GameManager.instance.MolotovUI.SetActive(true);
            }
        }
    }

    IEnumerator GrenadeExplosion(GameObject grenade)
    {
        float grenadeTimer = 0f;

        while (grenadeTimer < grenadeFuze)
        {
            grenadeTimer += Time.deltaTime;
            yield return null; 
        }
        GameObject explosion = Instantiate(grenadeExplosionPrefab, grenade.transform.position, grenade.transform.rotation);
        AudioSource audioSource = explosion.GetComponent<AudioSource>();
        AudioManager.PlaySFX(audioSource, grenadeExplosionClip);
        Destroy(grenade);
    }
    
}
