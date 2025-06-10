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
        PlayerPrefs.SetInt("Molotov", GameManager.instance.throwConsumableScript.molotovCount);
        PlayerPrefs.SetInt("Grenade", GameManager.instance.throwConsumableScript.grenadeCount);
        if (SceneManager.GetActiveScene().name == "IntroLevel")
        {
            PlayerPrefs.SetString("EquippedWeapons", "");
        } else {
            SaveEquippedWeapons();
        }
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
            GameManager.instance.playerScript.money = 0;
        }
        else
        {
            GameManager.instance.playerScript.currentHP = PlayerPrefs.GetFloat("PlayerHP");
            GameManager.instance.playerScript.money = PlayerPrefs.GetInt("PlayerMoney");
            LoadConsumables();
            LoadEquippedWeapons();
            if (SceneManager.GetActiveScene().name == "Shop")
            {
                GameManager.instance.levelExitScript.levelToLoad = PlayerPrefs.GetInt("LevelIndex") + 1;
            }
        }
    }
    
    //Call this if you want to reset stats to default
    public void DeleteAllData()
    {
        GameManager.instance.playerScript.currentHP = GameManager.instance.playerScript.maxHP;
        GameManager.instance.playerScript.money = 0;
        GameManager.instance.levelExitScript.levelToLoad = 1;
        SaveStats();
    }

    public void SaveEquippedWeapons()
    {
        string weapons = "";
        foreach (GunStats gun in GameManager.instance.weaponScript.gunList)
        {
            weapons += gun + ",";
        }
        PlayerPrefs.SetString("EquippedWeapons", weapons);
    }

    public void LoadEquippedWeapons()
    {
        string weapons = PlayerPrefs.GetString("EquippedWeapons", "");
        if (!string.IsNullOrEmpty(weapons))
        {
            string[] weaponArray = weapons.Split(',');
            foreach (string weapon in weaponArray)
            {
                if (!string.IsNullOrEmpty(weapon))
                {
                    switch (weapon)
                    {
                        case "Rifle (GunStats)":
                            GunStats gunStats = Resources.Load<GunStats>("Prefabs/Guns/Rifle");
                            GameManager.instance.weaponScript.GetGunStats(gunStats);
                            GameManager.instance.hotbarRifle.SetActive(true);
                            GameManager.instance.buttonScript.shopRifle.SetActive(false);
                            GameManager.instance.buttonScript.shopRifleAmmo.SetActive(true);
                            break;
                    }
                }
            }
        }
    }

    public void LoadConsumables()
    {
        GameManager.instance.throwConsumableScript.molotovCount = PlayerPrefs.GetInt("Molotov");
        GameManager.instance.throwConsumableScript.grenadeCount = PlayerPrefs.GetInt("Grenade");
        GameManager.instance.ammoScript.UpdateMolotovCount();
        GameManager.instance.ammoScript.UpdateGrenadeCount();
        if(GameManager.instance.throwConsumableScript.molotovCount > 0)
        {
            ThrowConsumable.GrenadeType currentType = ThrowConsumable.GrenadeType.Molotov;
            GameManager.instance.throwConsumableScript.currentType = currentType;
            GameManager.instance.MolotovUI.SetActive(true);
        } 
        else if (GameManager.instance.throwConsumableScript.grenadeCount > 0)
        {
            ThrowConsumable.GrenadeType currentType = ThrowConsumable.GrenadeType.Frag;
            GameManager.instance.throwConsumableScript.currentType = currentType;
            GameManager.instance.GrenadeUI.SetActive(true);
        }
    }

    public void Restart()
    {
        foreach(GunStats gun in GameManager.instance.weaponScript.gunList)
        {
            gun.currentAmmo = gun.levelStartCurrentAmmo;
            gun.magCount = gun.levelStartmagCount;
        }
    }
}
