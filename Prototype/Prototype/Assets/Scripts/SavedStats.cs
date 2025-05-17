using UnityEngine;

public class SavedStats : MonoBehaviour
{

    public void SaveStats()
    {
        PlayerPrefs.SetFloat("PlayerHP", GameManager.instance.playerScript.currentHP);
        PlayerPrefs.SetInt("PlayerMoney", GameManager.instance.playerScript.money);
    }

    public void LoadStats()
    {
        GameManager.instance.playerScript.currentHP = PlayerPrefs.GetFloat("PlayerHP");
        GameManager.instance.playerScript.money = PlayerPrefs.GetInt("PlayerMoney");
    }
    
    //Call this if you want to reset stats to default
    public void DeleteAllData()
    {
        PlayerPrefs.DeleteAll();
    }
}
