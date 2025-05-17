using UnityEngine;
using TMPro;
using UnityEditor;

// TODO: once weapons are scriptable objects add ammo and mag count to the UI

public class AmmoUI : MonoBehaviour
{
    public TextMeshProUGUI ammoCount;
    public TextMeshProUGUI magCount;
    void Start()
    {
        //UpdateAmmoUI();
    }
    public void UpdateAmmoCount()
    {
        ammoCount.text = GameManager.instance.playerScript.pistol.ToString();
    }
    public void UpdateMagCount()
    {
        magCount.text = GameManager.instance.weaponScript.magCount.ToString();
    }
    public void UpdateAmmoUI()
    {
        UpdateAmmoCount();
        UpdateMagCount();
    }
}
