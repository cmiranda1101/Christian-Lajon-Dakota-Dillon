using UnityEngine;

public class HidableObject : MonoBehaviour, IDamage
{
    //Always make the object a child of an empty object

    private void OnTriggerStay(Collider other)
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
            GameManager.instance.playerScript.Crouch();
        }
    }

    public void takeDamage(int amount)
    {
        if (gameObject.GetComponentInParent<HidableObject>() != null) 
            Destroy(gameObject.transform.parent.gameObject);

        GameManager.instance.playerScript.isHiding = false;
        
    }
}
