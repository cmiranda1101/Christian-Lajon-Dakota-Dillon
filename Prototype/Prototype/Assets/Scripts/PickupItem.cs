using UnityEngine;
using System.Collections;


//Put this Script on any object to be picked up by the player//

public class PickUpItem : MonoBehaviour, IInteract
{
    [SerializeField] Renderer itemModel;
    [SerializeField] Renderer playerModel;
    [SerializeField] GameObject pickUpText;

    [SerializeField] int healthAmount;

    Color originColorItem;
    Color originColorPlayer;

    float distanceFromPlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originColorItem = itemModel.material.color;
        playerModel = GameObject.FindGameObjectWithTag("Player").GetComponent<Renderer>();
        originColorPlayer = playerModel.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        distanceFromPlayer = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);

        if (distanceFromPlayer <= GameManager.instance.playerScript.grabDistance)
        {
            pickUpText.SetActive(true);

            Vector3 playerDirection = GameManager.instance.player.transform.position - transform.position;
            Quaternion rot = Quaternion.LookRotation(new Vector3(playerDirection.x, playerDirection.y, playerDirection.z));
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 100);
        }
        else
        {
            pickUpText.SetActive(false);
        }
    }

    public void Interact()
    {
        if (gameObject.CompareTag("Health")) {
            GameManager.instance.playerScript.Heal(healthAmount);
            StartCoroutine(PlayerHealFlash());
            Debug.Log("Healed " + healthAmount + " health.");
            StartCoroutine(ItemPickupFlash());
        }
        else if (gameObject.CompareTag("Ammo")) {
            GameManager.instance.weaponScript.PickUpAmmo();
            StartCoroutine(ItemPickupFlash());
        }
        
    }

    IEnumerator ItemPickupFlash()
    {
        //Give feedback on interaction
        itemModel.material.color = Color.white;
        yield return new WaitForSeconds(0.2f);
        itemModel.material.color = originColorItem;

        //Destroy obj
        Destroy(gameObject);
    }
    IEnumerator PlayerHealFlash()
    {
        //Give feedback on interaction
        playerModel.material.color = Color.green;
        yield return new WaitForSeconds(0.1f);
        playerModel.material.color = originColorPlayer;
    }


}
