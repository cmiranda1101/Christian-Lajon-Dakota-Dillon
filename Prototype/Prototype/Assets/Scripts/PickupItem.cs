using UnityEngine;
using System.Collections;


//Put this Script on any object to be picked up by the player//

public class InteractScript : MonoBehaviour, IInteract
{
    [SerializeField] Renderer model;
    public GameObject item;

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
            GameManager.instance.playerScript.Heal(10);
        }
    }

    IEnumerator FlashColor()
    {
        //Give feedback on interaction
        model.material.color = Color.green;
        yield return new WaitForSeconds(0.2f);
        model.material.color = originColor;

        //Destroy obj
        Destroy(gameObject);
    }
}
