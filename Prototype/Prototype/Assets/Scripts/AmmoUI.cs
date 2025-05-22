using UnityEngine;
using TMPro;
using UnityEditor;

public class AmmoUI : MonoBehaviour
{
    public TextMeshProUGUI ammoCount;
    public TextMeshProUGUI magCount;
    public void UpdatePistolAmmoAndMagCount()
    {
        //ammoCount.text = GameManager.instance.playerScript.pistol.GetComponent<GunBase>().currentBullets.ToString();
        //magCount.text = GameManager.instance.playerScript.pistol.GetComponent<GunBase>().magCount.ToString();
    }
    public void UpdateRifleAmmoAndMagCount()
    {
        
        //if(GameManager.instance.playerScript.rifle != null)
        //{
        //    ammoCount.text = GameManager.instance.playerScript.rifle.GetComponent<GunBase>().currentBullets.ToString();
        //    magCount.text = GameManager.instance.playerScript.rifle.GetComponent<GunBase>().magCount.ToString();
        //}
    }
   
}
