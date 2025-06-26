using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using UnityEngine.EventSystems;
using System.Collections;
// Note For Future - UnityEngine.EventSystems allows event listener behavior,
// to be used with mouse clicks in game to dynamically grab the clicked object or parent object.
// This is extremely useful for any shop manipulation since almost every game shop has a buy button.
// When Using with Objects you must know the hierarchy of the structure.

public class ButtonFunctions : MonoBehaviour
{
    //Main Menu Functionality
    [SerializeField] GameObject howToPlay;
    [SerializeField] GameObject credits;

    //Shop Functionality
    [SerializeField] AudioSource buyAudio;
    public GameObject shopRifle;
    public GameObject shopRifleAmmo;
    [SerializeField] GunStats shopRifleGunStats;
    [SerializeField] GunStats pistolStats;

    [SerializeField] AudioSource UISoundSource;

    private void Start()
    {
        UISoundSource.ignoreListenerPause = true;
    }

    public void OnResumeButton()
    {
        StartCoroutine(Resume());
    }
    IEnumerator Resume()
    {
        yield return StartCoroutine(UISound());
        GameManager.instance.StateUnpause();
    }

    public void OnRestartButton()
    {
        StartCoroutine(Restart());
    }
    IEnumerator Restart()
    {
        yield return StartCoroutine(UISound());
        GameManager.instance.savedStatsScript.Restart();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.instance.StateUnpause();
    }

    public void OnQuitButton()
    {
        StartCoroutine(Quit());
    }
    IEnumerator Quit()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            GameManager.instance.savedStatsScript.DeleteAllData();
        }
#if UNITY_EDITOR
        yield return StartCoroutine(UISound());
        UnityEditor.EditorApplication.isPlaying = false;
#else
            yield return StartCoroutine(UISound());
            Application.Quit();
#endif
    }

    public void OnCloseButton()
    {
        StartCoroutine(Close());
    }
    IEnumerator Close()
    {
        yield return StartCoroutine(UISound());
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
        if(GameManager.instance.playerScript.money >= 100 && GameManager.instance.playerScript.currentHP < GameManager.instance.playerScript.maxHP)
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
            if (GameManager.instance.weaponScript.gunList[GameManager.instance.weaponScript.gunListIndex] == pistolStats)
            {
                GameManager.instance.weaponScript.magCount++;
            }
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
            if (GameManager.instance.weaponScript.gunList[GameManager.instance.weaponScript.gunListIndex] == shopRifleGunStats)
            {
                GameManager.instance.weaponScript.magCount++;
            }
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

    public void OnCreditsButton()
    {
        StartCoroutine(Credits());
    }

    IEnumerator Credits()
    {
        yield return StartCoroutine(UISound());
        credits.SetActive(true);
    }

    public void OnCloseCreditsButton()
    {
        StartCoroutine(CloseCredits());
    }

    IEnumerator CloseCredits()
    {
        yield return StartCoroutine(UISound());
        credits.SetActive(false);
    }

    public void OnNewGameButton()
    {
        StartCoroutine(NewGame());
    }
    IEnumerator NewGame()
    {
        yield return StartCoroutine(UISound());
        SceneManager.LoadScene("Tutorial");
    }

    public void OnCloseHowToPlayButton()
    {
        StartCoroutine(CloseHowToPlay());
    }
    IEnumerator CloseHowToPlay()
    {
        yield return StartCoroutine(UISound());
        howToPlay.SetActive(false);
    }
    public void OnExitToMenuButton()
    {
        StartCoroutine(ExitToMenu());
    }
    IEnumerator ExitToMenu()
    {
        yield return StartCoroutine(UISound());
        SceneManager.LoadScene("MainMenu");
    }

    public IEnumerator UISound()
    {
        AudioManager.PlaySFX(UISoundSource, UISoundSource.clip);
        yield return new WaitForSecondsRealtime(.2f);
    }
    //public void OpenOptionsFromStart()
    //{
    //    GameManager.instance.previousMenu = GameManager.PreviousMenu.Start;
    //    GameManager.instance.menuButtons.SetActive(false); // Hide Start Menu
    //    GameManager.instance.menuOptions.SetActive(true);  // Show Options Menu
    //    GameManager.instance.menuActive = GameManager.instance.menuOptions;
    //}

    public void OnOpenOptionsFromPauseButton()
    {
        StartCoroutine(OpenOptionsFromPause());
    }
    IEnumerator OpenOptionsFromPause()
    {
        yield return StartCoroutine(UISound());
        GameManager.instance.previousMenu = GameManager.PreviousMenu.Pause;
        GameManager.instance.menuPause.SetActive(false);   // Hide Pause Menu
        GameManager.instance.menuOptions.SetActive(true);  // Show Options Menu
        GameManager.instance.menuActive = GameManager.instance.menuOptions;
    }

    public void OnBackFromOptionsButton()
    {
        StartCoroutine(BackFromOptions());
    }
    IEnumerator BackFromOptions()
    {
        yield return StartCoroutine(UISound());

        GameManager.instance.menuOptions.SetActive(false); // Hide Options

        switch (GameManager.instance.previousMenu)
        {
            case GameManager.PreviousMenu.Start:
                GameManager.instance.menuButtons.SetActive(true);
                GameManager.instance.menuActive = GameManager.instance.menuButtons;
                break;

            case GameManager.PreviousMenu.Pause:
                GameManager.instance.menuPause.SetActive(true);
                GameManager.instance.menuActive = GameManager.instance.menuPause;
                break;
        }

        GameManager.instance.previousMenu = GameManager.PreviousMenu.None;
    }
}
