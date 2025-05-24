using UnityEngine;
using UnityEngine.SceneManagement;

public class SavedStats : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            Debug.Log("Data deleted");
            DeleteAllData();
        }
    }
    public void SaveStats()
    {
        PlayerPrefs.SetFloat("PlayerHP", GameManager.instance.playerScript.currentHP);
        PlayerPrefs.SetInt("PlayerMoney", GameManager.instance.playerScript.money);
        if (GameManager.instance.levelExitScript != null)
        {
            PlayerPrefs.SetInt("LevelIndex", GameManager.instance.levelExitScript.levelToLoad);
        }
    }

    public void LoadStats()
    {
        if (PlayerPrefs.GetFloat("PlayerHP") <= 0)
        {
            GameManager.instance.playerScript.currentHP = GameManager.instance.playerScript.maxHP;
            GameManager.instance.playerScript.money = 1000;
        }
        else
        {
            GameManager.instance.playerScript.currentHP = PlayerPrefs.GetFloat("PlayerHP");
            GameManager.instance.playerScript.money = PlayerPrefs.GetInt("PlayerMoney");
            if (SceneManager.GetActiveScene().name == "Shop")
            {
                GameManager.instance.levelExitScript.levelToLoad = PlayerPrefs.GetInt("LevelIndex");
            }
        }
    }
    
    //Call this if you want to reset stats to default
    public void DeleteAllData()
    {
        GameManager.instance.playerScript.currentHP = GameManager.instance.playerScript.maxHP;
        GameManager.instance.playerScript.money = 1000;
        GameManager.instance.levelExitScript.levelToLoad = 1;
        SaveStats();
    }
}
