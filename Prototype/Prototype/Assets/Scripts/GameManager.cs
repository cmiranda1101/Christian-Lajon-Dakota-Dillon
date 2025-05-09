using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuHotbar;

    public GameObject player;
    public GameObject weapon;
    public PlayerController playerScript;
    public GunBase weaponScript;

    public bool isPaused;

    float timeScaleOrig;

    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        weapon = GameObject.FindWithTag("Weapon");
        playerScript = player.GetComponent<PlayerController>();
        weaponScript = weapon.GetComponent<GunBase>();
        timeScaleOrig = Time.timeScale;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    
    void Update()
    {
       if(Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
               StatePause();
               menuActive = menuPause;
               menuActive.SetActive(isPaused);
            }
            else if (menuActive == menuPause)
            {
                StateUnpause();
                menuActive = null;
            }
        }
    }

    public void StatePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        menuHotbar.SetActive(false);
    }

    public void StateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuHotbar.SetActive(true);
        menuActive.SetActive(false);
        menuActive = null;
    }
}
