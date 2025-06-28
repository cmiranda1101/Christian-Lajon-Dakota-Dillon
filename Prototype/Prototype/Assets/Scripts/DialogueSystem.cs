using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] AudioSource radioSquelch;

    [SerializeField] float displayTime;
    [SerializeField] float typeSpeed;

    TextMeshProUGUI dialogueText;

    private void Start()
    {
        dialogueText = GameManager.instance.dialogueText;
    }

    public IEnumerator TypeWriter()
    {
        GameManager.instance.isDialoguePlaying = true;

        AudioManager.PlaySFX(radioSquelch, radioSquelch.clip);
        yield return new WaitWhile(() => radioSquelch.isPlaying);

        while (GameManager.instance.dialogueArray.Count > 0) {
            TextMeshPro temp = GameManager.instance.dialogueArray[0];

            foreach (char letter in temp.text.ToCharArray()) {
                dialogueText.text += letter;
                yield return new WaitForSeconds(typeSpeed);
            }
            yield return new WaitForSeconds(displayTime);
            dialogueText.text = "";
            GameManager.instance.dialogueArray.RemoveAt(0);
        }
        dialogueText.text = "";
        GameManager.instance.dialogueBox.SetActive(false);
        GameManager.instance.isDialoguePlaying = false;
    }


}
