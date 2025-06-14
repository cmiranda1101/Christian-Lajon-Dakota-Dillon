using UnityEngine;
using System.Collections;

public class HeartBoss : MonoBehaviour, IDamage
{
    [SerializeField] Transform spawner1Location;
    [SerializeField] Transform spawner2Location;
    [SerializeField] Transform spawner3Location;
    [SerializeField] Transform spawner4Location;
    [SerializeField] GameObject enemySpawnerPrefab;
    [SerializeField] GameObject itemSpawners;
    GeneralSpawner generalSpawnerPrefab;

    [SerializeField] AudioSource heartBeatSource;
    [SerializeField] AudioClip fastBeatClip;
    [SerializeField] AudioClip slowBeatClip;

    [SerializeField] int bossHPMax;
    int bossHpCurr;

    Animation pumpAnim;
    Color HPColorOrigin;

    int phaseNum;
    float slowPumpSpeed;
    float fastPumpSpeed;
    bool enemiesSpawned;
    public bool isShielded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        phaseNum = 0;
        isShielded = true;
        enemiesSpawned = false;
        GameManager.instance.heartBossScript.generalSpawnerPrefab.startSpawn = true;

        bossHpCurr = bossHPMax;
        GameManager.instance.bossHealthUI.SetActive(true);
        HPColorOrigin = GameManager.instance.bossHealthBar.color;

        GameManager.instance.AmbianceForLevels.SetActive(false);
        GameManager.instance.AmbianceForBoss.SetActive(true);

        heartBeatSource.clip = slowBeatClip;

        pumpAnim = GetComponent<Animation>();
        fastPumpSpeed = pumpAnim["Armature|Pumping"].speed;
        slowPumpSpeed = pumpAnim["Armature|Pumping"].speed * 0.5f;  //slow the animation while shield up
        pumpAnim["Armature|Pumping"].speed = slowPumpSpeed;

       
        StartCoroutine(PlayBeat());
        SpawnersOn();
    }

    // Update is called once per frame
    void Update()
    {
        int enemies = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (enemies > 1 && !enemiesSpawned) {
            enemiesSpawned = true;  //enemies have spawned, stopping Coroutine spam;
        }
        if (enemies == 1 && enemiesSpawned) {
            enemiesSpawned = false;
            ShieldDown();
        }
    }

    public void takeDamage(int amount)  //Can only hurt boss in shield down mode;
    {
        if (!isShielded) {
            bossHpCurr = Mathf.Clamp(bossHpCurr -= amount, 0, bossHPMax);
            GameManager.instance.bossHealthBar.fillAmount = Mathf.Lerp(bossHpCurr,((float)bossHpCurr / bossHPMax), 9);
            ShieldUp();

            if (bossHpCurr <= 0) {
                GameManager.instance.bossHealthUI.SetActive(false);
                Destroy(gameObject);
                GameManager.instance.Win();
            }
        }
    }

    void ShieldDown()    //Shield is down, you can hurt boss;
    {
        //Debug.Log("in Shield down");

        isShielded = false;
        SpawnersOff();
        GameManager.instance.heartBossScript.generalSpawnerPrefab.startSpawn = true;
        pumpAnim["Armature|Pumping"].speed = fastPumpSpeed;
        heartBeatSource.clip = fastBeatClip;
        GameManager.instance.bossHealthBar.color = Color.red;
    }

    void ShieldUp()
    {
        if (bossHpCurr <= bossHPMax * .75f && phaseNum == 0 || bossHpCurr <= bossHPMax * .5f && phaseNum == 1 || bossHpCurr <= bossHPMax * .25f && phaseNum == 2) {
            isShielded = true;
            pumpAnim["Armature|Pumping"].speed = slowPumpSpeed;
            heartBeatSource.clip = slowBeatClip;
            GameManager.instance.bossHealthBar.color = HPColorOrigin;
            SpawnersOn();
            generalSpawnerPrefab = itemSpawners.GetComponent<GeneralSpawner>();
            generalSpawnerPrefab.startSpawn = true;
            phaseNum++;
        }
    }

    IEnumerator PlayBeat()
    {
        while (true) {
            heartBeatSource.Play();
            yield return new WaitWhile(() => heartBeatSource.isPlaying);
        }
    }

    void SpawnersOn()
    {
        Instantiate(enemySpawnerPrefab, spawner1Location);
        Instantiate(enemySpawnerPrefab, spawner2Location);
        Instantiate(enemySpawnerPrefab, spawner3Location);
        Instantiate(enemySpawnerPrefab, spawner4Location);
    }

    void SpawnersOff()
    {
        Destroy(spawner1Location.GetChild(0).gameObject); 
        Destroy(spawner2Location.GetChild(0).gameObject); 
        Destroy(spawner3Location.GetChild(0).gameObject); 
        Destroy(spawner4Location.GetChild(0).gameObject); 
    }
}
