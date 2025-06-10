using UnityEngine;
using TMPro;
using UnityEditor;

public class AmmoUI : MonoBehaviour
{
    public TextMeshProUGUI ammoCount;
    public TextMeshProUGUI magCount;
    public TextMeshProUGUI molotovCount;
    public TextMeshProUGUI grenadeCount;
    public void UpdateAmmoAndMagCount()
    {
        ammoCount.text = GameManager.instance.weaponScript.currentAmmo.ToString();
        magCount.text = GameManager.instance.weaponScript.magCount.ToString();
    }

    public void UpdateMolotovCount()
    {
        molotovCount.text = GameManager.instance.throwConsumableScript.molotovCount.ToString();
    }
    public void UpdateGrenadeCount()
    {
        grenadeCount.text = GameManager.instance.throwConsumableScript.grenadeCount.ToString();
    }
}
