using UnityEngine;
using System.Collections;


//Put this Script on any object to be picked up by the player//

public class InteractScript : MonoBehaviour, Interact
{
    [SerializeField] Renderer model;

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

    public void Pickup()
    {
        StartCoroutine(FlashColor());
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
