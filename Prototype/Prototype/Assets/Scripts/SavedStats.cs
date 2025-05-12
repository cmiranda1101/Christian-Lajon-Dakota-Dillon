using UnityEngine;

public class SavedStats : MonoBehaviour
{
    public float playerHP = 0;
    int playerMoney;
    int magCount;

    public void SaveStats()
    {
        playerHP = GameManager.instance.playerScript.currentHP;
        playerMoney = GameManager.instance.playerScript.money;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadStats()
    {
        GameManager.instance.playerScript.currentHP = playerHP;
        GameManager.instance.playerScript.money = playerMoney;
    }
}
