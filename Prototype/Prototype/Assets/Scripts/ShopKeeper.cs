using UnityEngine;

//Do not change the layer of the shopkeeper
public class ShopKeeper : MonoBehaviour, IInteract
{
    [SerializeField] GameObject directions;
    public void Interact()
    {
        if(GameManager.instance.isPaused)
        {
            return;
        }
        Debug.Log("Interacting with shopkeeper");
        GameManager.instance.OpenShop();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            directions.transform.localPosition = new Vector3(0, 0.3f, 0);
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
