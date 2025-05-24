using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [SerializeField] GameObject exitText;

    [SerializeField] public int levelToLoad;
    bool canExit = false;

    private void Update()
    {
        if (canExit)
        {
            exitText.transform.rotation = GameManager.instance.player.transform.rotation;
            if (Input.GetButtonDown("Interact"))
            {
                GameManager.instance.savedStatsScript.SaveStats();
                SceneManager.LoadScene(levelToLoad);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        exitText.SetActive(true);
        canExit = true;
    }

    private void OnTriggerExit(Collider other)
    {
        exitText.SetActive(false);
        canExit = false;
    }
}
