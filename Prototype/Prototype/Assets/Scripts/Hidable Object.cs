using UnityEngine;

public class HidableObject : MonoBehaviour
{

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player") {
            GameManager.instance.playerScript.isHiding = true;
            GameManager.instance.playerScript.MainCamera;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player") {
            GameManager.instance.playerScript.isHiding = false;
        }
    }



}
