using System.Collections;
using UnityEngine;

public class HidableObject : MonoBehaviour, IDamage
{
    [SerializeField] GameObject breakEffect;
    Transform spawnSpot;

    //Always make the object a child of an empty object

    private void Start()
    {
        spawnSpot = gameObject.transform.parent.transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
            if (!GameManager.instance.playerScript.anim.GetBool("isCrouching")) {
                GameManager.instance.playerScript.Crouch();
            }
            GameManager.instance.playerScript.isHiding = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player") {
            GameManager.instance.playerScript.isHiding = false;
            //GameManager.instance.playerScript.Crouch();
        }
    }

    public void takeDamage(int amount)
    {
        Destroy(gameObject.transform.parent.gameObject);

        GameManager.instance.playerScript.isHiding = false;

        Instantiate(breakEffect, spawnSpot.position, Quaternion.identity);
    }
}
