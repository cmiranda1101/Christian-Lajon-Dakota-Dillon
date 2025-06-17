using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] TextMeshPro textToShow;
    [SerializeField] AudioSource radioSquelch;
    AudioClip radioChirp;

    [SerializeField] float typeSpeed;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            dialogueText.text = "";
            GameManager.instance.dialogueBox.SetActive(true);
           
            StartCoroutine(TypeWriter());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameManager.instance.dialogueBox.SetActive(false);
    }

    IEnumerator TypeWriter()
    {
        radioSquelch.Play();
        yield return new WaitWhile(() => radioSquelch.isPlaying);

        foreach (char letter in textToShow.text.ToCharArray()) {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }
    }
}
