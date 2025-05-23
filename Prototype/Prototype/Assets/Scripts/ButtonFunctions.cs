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
    [SerializeField] AudioSource buyAudio;
    GameObject shopRifle;

    public void Resume()
    {
        GameManager.instance.StateUnpause();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.instance.StateUnpause();
    }

    public void Quit()
    {
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
            //GameManager.instance.weaponScript.EquipRifle();
            Destroy(shopRifle);
            GameManager.instance.moneyScript.SubtractMoney(100);
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
        //if (GameManager.instance.playerScript.money >= 50 && GameManager.instance.playerScript.pistol != null)
        //{
        //    buyAudio.Play();
        //    GameManager.instance.moneyScript.SubtractMoney(50);
        //    GunBase pistol = GameManager.instance.playerScript.pistol.GetComponent<GunBase>();
        //    pistol.magCount++;
        //    pistol.UpdateAmmo();
        //}
        
    }

    public void BuyRifleAmmo()
    {
        //if (GameManager.instance.playerScript.money >= 50 && GameManager.instance.playerScript.rifle != null)
        //{
        //    buyAudio.Play();
        //    GameManager.instance.moneyScript.SubtractMoney(50);
        //    GunBase rifle = GameManager.instance.playerScript.rifle.GetComponent<GunBase>();
        //    rifle.magCount++;
        //    rifle.UpdateAmmo();
        //}
    }

    public void BuyMolotov()
    {
        if (GameManager.instance.playerScript.money >= 100)
        {
            if (GameManager.instance.throwConsumableScript.molotovCount == 0)
            {
                GameManager.instance.MolotovUI.SetActive(true);
            }
            buyAudio.Play();
            GameManager.instance.playerScript.throwConsumable.molotovCount++;
            GameManager.instance.molotovCounter.text = GameManager.instance.playerScript.throwConsumable.molotovCount.ToString();
            GameManager.instance.moneyScript.SubtractMoney(100);
        }
    }
}
