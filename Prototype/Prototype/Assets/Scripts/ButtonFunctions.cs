using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    [SerializeField] AudioSource buyAudio;
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
        if (GameManager.instance.playerScript.money >= 100) {
            buyAudio.Play();
            GameManager.instance.weaponScript.EquipRifle();
            GameManager.instance.playerScript.money -= 100;
            GameManager.instance.moneyScript.UpdateMoneyText();
        }
    }

    public void GoToShop()
    {
        GameManager.instance.savedStats.SaveStats();
        SceneManager.LoadSceneAsync("Shop");
    }
}
