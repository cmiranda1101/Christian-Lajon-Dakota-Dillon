using TMPro;
using UnityEngine;

public class MoneyUI : MonoBehaviour
{
    public TextMeshProUGUI moneyText;

    void Start()
    {
        UpdateMoneyText();
    }
    public void SetMoney(int amount)
    {
        GameManager.instance.playerScript.money = amount;
        UpdateMoneyText();
    }
    public void SubtractMoney(int amount)
    {
        GameManager.instance.playerScript.money -= amount;
        UpdateMoneyText();
    }
    public void AddMoney(int amount)
    {
        GameManager.instance.playerScript.money += amount;
        UpdateMoneyText();
    }
    public void UpdateMoneyText()
    {
        moneyText.text = "$" + GameManager.instance.playerScript.money.ToString();
    }
}
