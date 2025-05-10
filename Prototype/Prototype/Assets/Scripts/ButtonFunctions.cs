using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
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
            GameManager.instance.weaponScript.EquipRifle();
        }
    }

    public void GoToShop()
    {
        SceneManager.LoadScene("Shop");
    }
}
