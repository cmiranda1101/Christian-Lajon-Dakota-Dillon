using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] TextMeshPro textToShow;
    [SerializeField] int numActivationsAllowed;

    int numActivations;

    private void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            if (numActivations >= numActivationsAllowed) {
                Destroy(gameObject);
                return;
            }

            numActivations++;
            GameManager.instance.dialogueBox.SetActive(true);
            GameManager.instance.dialogueArray.Add(textToShow);

            PlayDialogue(); 
        }
    }

    void PlayDialogue()
    {
        if (!GameManager.instance.isDialoguePlaying) 
            StartCoroutine(GameManager.instance.dialogueSystem.TypeWriter());
    }
}
