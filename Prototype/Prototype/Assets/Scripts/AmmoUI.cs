using UnityEngine;
using TMPro;
using UnityEditor;

public class AmmoUI : MonoBehaviour
{
    public TextMeshProUGUI ammoCount;
    public TextMeshProUGUI magCount;

    public void Start()
    {
        UpdatePistolAmmoAndMagCount();
    }
    public void UpdatePistolAmmoAndMagCount()
    {
        GunBase pistol = GameObject.FindWithTag("Pistol").GetComponent<GunBase>();
        ammoCount.text = pistol.currentBullets.ToString();
        magCount.text = pistol.magCount.ToString();
    }
    public void UpdateRifleAmmoAndMagCount()
    {
        GunBase rifle = GameObject.FindWithTag("Rifle").GetComponent<GunBase>();
        if (rifle != null)
        {
            ammoCount.text = rifle.currentBullets.ToString();
            magCount.text = rifle.magCount.ToString();
        }
    }
   
}
