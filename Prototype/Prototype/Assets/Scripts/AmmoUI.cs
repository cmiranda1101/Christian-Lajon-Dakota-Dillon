using UnityEngine;
using TMPro;
using UnityEditor;

public class AmmoUI : MonoBehaviour
{
    public TextMeshProUGUI ammoCount;
    public TextMeshProUGUI magCount;
    public void UpdateAmmoAndMagCount()
    {
        ammoCount.text = GameManager.instance.weaponScript.currentAmmo.ToString();
        magCount.text = GameManager.instance.weaponScript.magCount.ToString();
    }  
}
