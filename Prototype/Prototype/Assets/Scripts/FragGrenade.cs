using UnityEngine;

public class FragGrenade : MonoBehaviour
{
    public AudioSource grenadeAudioSource;
    public AudioClip contactHitClip;
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts.Length > 0)
        {
            AudioManager.PlaySFX(grenadeAudioSource, contactHitClip);
        }
    }
}
