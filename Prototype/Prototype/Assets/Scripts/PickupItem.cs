using UnityEngine;
using System.Collections;


//Put this Script on any object to be picked up by the player//

public class PickUpItem : MonoBehaviour, IInteract
{
    [SerializeField] Renderer model;

    [SerializeField] int healthAmount;

    Color originColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originColor = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact()
    {
        if (gameObject.CompareTag("Health")) {
            GameManager.instance.playerScript.Heal(healthAmount);
            Debug.Log("Healed " + healthAmount + " health.");
        }
        else if (gameObject.CompareTag("Ammo")) {
            GameManager.instance.weaponScript.PickUpAmmo();
        }

        StartCoroutine(FlashColor());
    }

    IEnumerator FlashColor()
    {
        //Give feedback on interaction
        model.material.color = Color.white;
        yield return new WaitForSeconds(0.2f);
        model.material.color = originColor;

        //Destroy obj
        Destroy(gameObject);
    }


}
