using UnityEngine;


public class PickupBoxes : MonoBehaviour, IDamage
{
    [SerializeField] GameObject[] pickupList;

    Transform spawnSpot;

    private void Start()
    {
        spawnSpot = gameObject.transform;
    }
    public void takeDamage(int amount)
    {
        Destroy(gameObject);

        int i = Random.Range(0, pickupList.Length);
        Instantiate(pickupList[i], spawnSpot.position, Quaternion.identity);
    }
}
