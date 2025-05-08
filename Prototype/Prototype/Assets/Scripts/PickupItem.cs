using UnityEngine;
using System.Collections;


//Put this Script on any object to be picked up by the player//

public class InteractScript : MonoBehaviour, IInteract
{
    [SerializeField] Renderer model;

    [SerializeField] int healthAmount;
    [SerializeField] int ammoAmount;

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

    public void Interact(GameObject item)
    {
        StartCoroutine(FlashColor());

        if (item.CompareTag("Health")) {
            GameManager.instance.playerScript.Heal(healthAmount);
        }
        else if (item.CompareTag("Ammo")) {
            GameManager.instance.weaponScript.PickUpAmmo();
        }
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
