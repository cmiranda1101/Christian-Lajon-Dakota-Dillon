using UnityEngine;

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
    }

    public void LoadStats()
    {
        if (PlayerPrefs.GetFloat("PlayerHP") <= 0)
        {
            GameManager.instance.playerScript.currentHP = GameManager.instance.playerScript.maxHP;
            GameManager.instance.playerScript.money = 1000;
        }
        GameManager.instance.playerScript.currentHP = PlayerPrefs.GetFloat("PlayerHP");
        GameManager.instance.playerScript.money = PlayerPrefs.GetInt("PlayerMoney");
    }
    
    //Call this if you want to reset stats to default
    public void DeleteAllData()
    {
        PlayerPrefs.DeleteAll();
    }
}
