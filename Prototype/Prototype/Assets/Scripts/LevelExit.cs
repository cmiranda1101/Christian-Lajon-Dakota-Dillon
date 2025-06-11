using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [SerializeField] GameObject exitText;
    [SerializeField] int moneyEarned;

    [SerializeField] public int levelToLoad;
    bool canExit = false;

    private void Update()
    {
        if (canExit)
        {
            exitText.transform.rotation = GameManager.instance.player.transform.rotation;
            if (Input.GetButtonDown("Interact"))
            {
                if (SceneManager.GetActiveScene().name != "Shop" && SceneManager.GetActiveScene().name != "Tutorial")
                {
                    GameManager.instance.playerScript.money += moneyEarned;
                    GameManager.instance.savedStatsScript.SaveStats();
                    SceneManager.LoadScene("Shop");
                }
                else
                {
                    GameManager.instance.savedStatsScript.SaveStats();
                    SceneManager.LoadScene(levelToLoad);
                }
                if (levelToLoad == 0)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            exitText.SetActive(true);
            canExit = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        exitText.SetActive(false);
        canExit = false;
    }
}
