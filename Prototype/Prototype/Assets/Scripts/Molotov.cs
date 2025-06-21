using System.Collections;
using UnityEngine;

public class Molotov : MonoBehaviour
{
    public GameObject fireEffectPrefab;
    public AudioSource molotovAudioSource;
    public AudioClip glassBreakClip;
    public void OnCollisionEnter(Collision collision)
    {
        if(collision.contacts.Length > 0)
        {
            StartCoroutine(DestroyObject());
            ContactPoint contact = collision.contacts[0];
            Vector3 hitPoint = contact.point;
            Quaternion hitRotation = Quaternion.FromToRotation(Vector3.forward, contact.normal);
            GameObject fireEffect = Instantiate(fireEffectPrefab, hitPoint, hitRotation);
            
        }
    }
    IEnumerator DestroyObject()
    {
        AudioManager.PlaySFX(molotovAudioSource, glassBreakClip);
        yield return new WaitForSeconds(.2f);
        Destroy(gameObject);
    }
}
