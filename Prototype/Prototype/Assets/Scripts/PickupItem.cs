using UnityEngine;
using System.Collections;


//Put this Script on any object to be picked up by the player//

//Drag Directions/Text to display onto directions in inspector//

public class PickUpItem : MonoBehaviour
{
    [SerializeField] Renderer itemModel;
    [SerializeField] public GameObject pickUpText;

    [SerializeField] int healthAmount;

    Color originColorItem;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        itemModel = gameObject.GetComponentInParent<MeshRenderer>();
        originColorItem = itemModel.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerDirection = GameManager.instance.player.transform.position - transform.position;
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDirection.x, playerDirection.y, playerDirection.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 100);
    }

    IEnumerator ItemPickupFlash()
    {
        //Give feedback on interaction
        itemModel.material.color = Color.white;
        yield return new WaitForSeconds(0.2f);
        itemModel.material.color = originColorItem;

        //Destroy obj
        Destroy(gameObject.transform.parent.gameObject);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            string triggerName = gameObject.name;

            if (triggerName == "DisplayTextZone") {
                pickUpText.SetActive(true);
            }
            else if (triggerName == "PickupZone") {
                if(gameObject.transform.parent.tag == "Health") {
                    if (GameManager.instance.playerScript.currentHP < GameManager.instance.playerScript.maxHP) {
                        GameManager.instance.playerScript.Heal(healthAmount);
                        Debug.Log("Healed " + healthAmount + " health.");
                        StartCoroutine(ItemPickupFlash());
                    }
                }
                else if(gameObject.transform.parent.tag == "Ammo") {
                    GameManager.instance.weaponScript.PickUpAmmo();
                    StartCoroutine(ItemPickupFlash());
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) {
            string triggerName = gameObject.name;
            if (triggerName == "DisplayTextZone") {
                pickUpText.SetActive(false);
            }
        }
    }

}
