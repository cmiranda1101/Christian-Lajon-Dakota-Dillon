using UnityEngine;
using System.Collections;


//Put this Script on any object to be picked up by the player//

//Drag Directions/Text to display onto directions in inspector//

public class PickUpItem : MonoBehaviour
{
    [SerializeField] AudioSource itemPickupSource;
    [SerializeField] AudioClip[] itemPickupClips;
    [SerializeField] public GameObject pickUpText;

    [SerializeField] int healthAmount;

    Color originColorItem;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //aitemModel = gameObject.GetComponentInParent<MeshRenderer>();
        //aoriginColorItem = itemModel.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerDirection = GameManager.instance.player.transform.position - transform.position;
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDirection.x, playerDirection.y, playerDirection.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 100);
    }

    IEnumerator ItemPickupSound()
    {
        int i = Random.Range(0, itemPickupClips.Length);

        AudioManager.PlaySFX(itemPickupSource, itemPickupClips[i]);
        yield return new WaitWhile(() => itemPickupSource.isPlaying);

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
                        StartCoroutine(ItemPickupSound());
                    }
                }
                else if(gameObject.transform.parent.tag == "Ammo") {
                    GameManager.instance.weaponScript.PickUpAmmo();
                    StartCoroutine(ItemPickupSound());
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
