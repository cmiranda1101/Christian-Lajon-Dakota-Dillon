using UnityEngine;


public class PickupBoxes : MonoBehaviour, IDamage, IInteract
{
    [SerializeField] GameObject[] pickupList;
    [SerializeField] GameObject breakEffect;

    Transform spawnSpot;

    private void Start()
    {
        spawnSpot = gameObject.transform;
    }
    public void takeDamage(int amount)
    {
        Instantiate(breakEffect, spawnSpot.position, Quaternion.identity);

        int i = Random.Range(0, pickupList.Length);
        Instantiate(pickupList[i], spawnSpot.position, Quaternion.identity);

        Destroy(gameObject);
    }

    public void Interact()
    {
        Instantiate(breakEffect, spawnSpot.position, Quaternion.identity);

        int i = Random.Range(0, pickupList.Length);
        Instantiate(pickupList[i], spawnSpot.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
