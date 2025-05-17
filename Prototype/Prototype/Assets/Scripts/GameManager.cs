using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuGameOver;
    [SerializeField] GameObject menuHotbar;
    [SerializeField] GameObject menuMoney;
    [SerializeField] GameObject menuShop;
    [SerializeField] GameObject savedStats;

    public GameObject DamageFlash;
    public GameObject miniMap;
    public GameObject hotBarPistol;
    public GameObject hotbarRifle;
    public GameObject healthBar;

    public GameObject player;
    public GameObject weapons;
    public PlayerController playerScript;
    public GunBase weaponScript;
    public MoneyUI moneyScript;
    public SavedStats savedStatsScript;


    public bool isPaused;

    float timeScaleOrig;
    int gameGoalCount;

    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
        savedStatsScript = savedStats.GetComponent<SavedStats>();
        miniMap = GameObject.FindWithTag("MiniMap");
        if(SceneManager.GetActiveScene().name == "Shop")
        {
            miniMap.SetActive(false);
        }
        weapons = GameObject.FindWithTag("Weapons");
        weaponScript = weapons.GetComponent<GunBase>();
        moneyScript = menuMoney.GetComponentInChildren<MoneyUI>();
        timeScaleOrig = Time.timeScale;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        LevelStart();
    }

    private void Start()
    {
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
    public void UpdateGameGoal(int amount)
    {
        gameGoalCount += amount;
        if (gameGoalCount <= 0)
        {
            Win();
        }
        
    }

    public void OpenShop()
    {
        StatePause();
        menuShop.SetActive(true);
        menuActive = menuShop;
        menuActive.SetActive(isPaused);
    }

    public void CloseShop()
    {
        StateUnpause();
        menuShop.SetActive(false);
    }

    public void Win()
    {
        StatePause();
        menuActive = menuWin;
        menuWin.SetActive(true);
    }

    public void YouLose()
    {
        StatePause();
        menuActive = menuGameOver;
        menuGameOver.SetActive(true);
    }

    public void LevelStart()
    {
        isPaused = false;
        timeScaleOrig = 1;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuHotbar.SetActive(true);
        if (SceneManager.GetActiveScene().name != "IntroLevel")
        {
            savedStatsScript.LoadStats();
        }
        healthBar.transform.localScale = new Vector3(playerScript.currentHP / playerScript.maxHP, .75f, 1);
    }
}
