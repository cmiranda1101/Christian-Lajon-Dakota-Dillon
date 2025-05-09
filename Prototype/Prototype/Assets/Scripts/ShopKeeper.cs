using UnityEngine;

public class ShopKeeper : MonoBehaviour, IInteract
{

    public void Interact()
    {
        Debug.Log("Interacting with shopkeeper");
        GameManager.instance.OpenShop();
    }

}
