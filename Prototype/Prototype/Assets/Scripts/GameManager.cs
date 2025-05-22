using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
    [SerializeField] GameObject menuAmmo;
    [SerializeField] GameObject menuShop;
    [SerializeField] GameObject savedStats;

    public GameObject DamageFlash;
    public GameObject HealFlash;
    public GameObject miniMap;
    public GameObject hotBarPistol;
    public GameObject hotbarRifle;
    public GameObject healthUI;
    public GameObject MolotovUI;
    public UnityEngine.UI.Image healthBar;
    public GameObject bossHealthUI;
    public UnityEngine.UI.Image bossHealthBar;
    public UnityEngine.UI.Image dodgeCooldownRadial;
    public TextMeshProUGUI chemlightCounter;
    public TextMeshProUGUI molotovCounter;

    public GameObject AmbianceForLevels;
    public GameObject AmbianceForBoss;

    public GameObject player;
    public GameObject weapons;
    public PlayerController playerScript;
    public GunBase weaponScript;
    public MoneyUI moneyScript;
    public SavedStats savedStatsScript;
    public AmmoUI ammoScript;
    public LevelExit levelExitScript;
    public HeartBoss heartBossScript;
    public ThrowConsumable throwConsumableScript;


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
            //if you need to test going to the shop or another scene make sure there is a level exit prefab in your scene
            levelExitScript = GameObject.FindWithTag("LevelExit").GetComponent<LevelExit>();
            levelExitScript.levelToLoad++;
        }
        else
        {
            levelExitScript = null;
        }
        weapons = GameObject.FindWithTag("Weapons");
        weaponScript = weapons.GetComponent<GunBase>();
        moneyScript = menuMoney.GetComponentInChildren<MoneyUI>();
        ammoScript = menuAmmo.GetComponentInChildren<AmmoUI>();
        throwConsumableScript = player.GetComponentInChildren<ThrowConsumable>();
        timeScaleOrig = Time.timeScale;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Start()
    {
        LevelStart();
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
        healthBar.fillAmount = playerScript.currentHP / playerScript.maxHP;
    }

    public int CheckGameGoal()
    {
        return gameGoalCount;
    }
}
