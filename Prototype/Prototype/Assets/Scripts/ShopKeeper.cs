using UnityEngine;

public class ShopKeeper : MonoBehaviour, IInteract
{
    [SerializeField] GameObject directions;
    public void Interact()
    {
        Debug.Log("Interacting with shopkeeper");
        GameManager.instance.OpenShop();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            directions.SetActive(true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            directions.SetActive(false);
        }
    }

}
