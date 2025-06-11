using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using UnityEngine.EventSystems;
// Note For Future - UnityEngine.EventSystems allows event listener behavior,
// to be used with mouse clicks in game to dynamically grab the clicked object or parent object.
// This is extremely useful for any shop manipulation since almost every game shop has a buy button.
// When Using with Objects you must know the hierarchy of the structure.

public class ButtonFunctions : MonoBehaviour
{
    //Main Menu Functionality
    [SerializeField] GameObject howToPlay;

    //Shop Functionality
    [SerializeField] AudioSource buyAudio;
    public GameObject shopRifle;
    public GameObject shopRifleAmmo;
    [SerializeField] GunStats shopRifleGunStats;
    [SerializeField] GunStats pistolStats;

    public void Resume()
    {
        GameManager.instance.StateUnpause();
    }

    public void Restart()
    {
        GameManager.instance.savedStatsScript.Restart();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.instance.StateUnpause();
    }

    public void Quit()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            GameManager.instance.savedStatsScript.DeleteAllData();
        }
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void Close()
    {
        GameManager.instance.CloseShop();
    }

    public void BuyRifle()
    {
        shopRifle = EventSystem.current.currentSelectedGameObject.transform.parent.gameObject;
        if (GameManager.instance.playerScript.money >= 100) {
            buyAudio.Play();
            shopRifleGunStats.currentAmmo = shopRifleGunStats.magSize;
            shopRifleGunStats.magCount = shopRifleGunStats.startingMagCount;
            GameManager.instance.weaponScript.GetGunStats(shopRifleGunStats);
            Destroy(shopRifle);
            GameManager.instance.hotbarRifle.SetActive(true);
            GameManager.instance.moneyScript.SubtractMoney(100);
            shopRifleAmmo.SetActive(true);
        }
    }

    public void GoToShop()
    {
        GameManager.instance.savedStatsScript.SaveStats();
        SceneManager.LoadSceneAsync("Shop");
    }

    public void BuyHealth()
    {
        if(GameManager.instance.playerScript.money >= 100)
        {
            buyAudio.Play();
            GameManager.instance.playerScript.Heal(GameManager.instance.playerScript.maxHP);
            GameManager.instance.moneyScript.SubtractMoney(100);
        }
    }

    public void BuyPistolAmmo()
    {
        if (GameManager.instance.playerScript.money >= 50)
        {
            buyAudio.Play();
            pistolStats.magCount++;
            GameManager.instance.weaponScript.magCount++;
            GameManager.instance.ammoScript.UpdateAmmoAndMagCount();
            GameManager.instance.moneyScript.SubtractMoney(50);
        }
    }

    public void BuyRifleAmmo()
    {
        if (GameManager.instance.playerScript.money >= 50)
        {
            buyAudio.Play();
            shopRifleGunStats.magCount++;
            GameManager.instance.weaponScript.magCount++;
            GameManager.instance.ammoScript.UpdateAmmoAndMagCount();
            GameManager.instance.moneyScript.SubtractMoney(50);
        }
    }

    public void BuyMolotov()
    {
        if (GameManager.instance.playerScript.money >= 100)
        {
            if (GameManager.instance.throwConsumableScript.molotovCount == 0)
            {
                GameManager.instance.GrenadeUI.SetActive(false);
                GameManager.instance.MolotovUI.SetActive(true);
                ThrowConsumable.GrenadeType currentType = ThrowConsumable.GrenadeType.Molotov;
                GameManager.instance.playerScript.throwConsumable.currentType = currentType;
            }
            buyAudio.Play();
            GameManager.instance.playerScript.throwConsumable.molotovCount++;
            GameManager.instance.molotovCounter.text = GameManager.instance.playerScript.throwConsumable.molotovCount.ToString();
            GameManager.instance.moneyScript.SubtractMoney(100);
        }
    }

    public void BuyGrenade()
    {
        if (GameManager.instance.playerScript.money >= 100)
        {
            if (GameManager.instance.throwConsumableScript.grenadeCount == 0)
            {
                GameManager.instance.MolotovUI.SetActive(false);
                GameManager.instance.GrenadeUI.SetActive(true);
                ThrowConsumable.GrenadeType currentType = ThrowConsumable.GrenadeType.Frag;
                GameManager.instance.playerScript.throwConsumable.currentType = currentType;
            }
            buyAudio.Play();
            GameManager.instance.playerScript.throwConsumable.grenadeCount++;
            GameManager.instance.grenadeCounter.text = GameManager.instance.playerScript.throwConsumable.grenadeCount.ToString();
            GameManager.instance.moneyScript.SubtractMoney(100);
        }
    }

    public void NewGame()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void CloseHowToPlay()
    {
        howToPlay.SetActive(false);
    }

    public void ExitToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
