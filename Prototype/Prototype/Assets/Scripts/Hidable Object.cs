using UnityEngine;

public class HidableObject : MonoBehaviour, IDamage
{
    [SerializeField] Collider stayOut;

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player") {
            //GameManager.instance.playerScript.anim.SetBool("isCrouching", true);
            if (!GameManager.instance.playerScript.anim.GetBool("isCrouching")) {
                GameManager.instance.playerScript.Crouch();
            }
            stayOut.enabled = false;
            GameManager.instance.playerScript.isHiding = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player") {
            //GameManager.instance.playerScript.anim.SetBool("isCrouching", false);
            GameManager.instance.playerScript.isHiding = false;
            GameManager.instance.playerScript.Crouch();
            stayOut.enabled = true;
        }
    }

    public void takeDamage(int amount)
    {
        GameManager.instance.playerScript.anim.SetBool("isCrouching", false);
        GameManager.instance.playerScript.isHiding = false;
        Destroy(gameObject);
    }
}
