using UnityEngine;

public class Molotov : MonoBehaviour
{
    public GameObject fireEffectPrefab;

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.contacts.Length > 0)
        {
            ContactPoint contact = collision.contacts[0];
            Vector3 hitPoint = contact.point;
            Quaternion hitRotation = Quaternion.FromToRotation(Vector3.forward, contact.normal);
            GameObject fireEffect = Instantiate(fireEffectPrefab, hitPoint, hitRotation);
            Destroy(gameObject);
        }
    }

}
