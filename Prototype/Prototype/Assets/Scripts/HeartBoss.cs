using UnityEngine;
using System.Collections;

public class HeartBoss : MonoBehaviour, IDamage
{
    [SerializeField] Transform spawner1Location;
    [SerializeField] Transform spawner2Location;
    [SerializeField] Transform spawner3Location;
    [SerializeField] Transform spawner4Location;
    [SerializeField] GameObject spawnerPrefab;
    [SerializeField] GameObject bulletSpawner;

    [SerializeField] AudioSource heartBeatSource;
    [SerializeField] AudioClip fastBeatClip;
    [SerializeField] AudioClip slowBeatClip;

    [SerializeField] int shieldDownTime;

    [SerializeField] int bossHPMax;
    int bossHpCurr;

    Animation pumpAnim;
    Color HPColorOrigin;

    float slowPumpSpeed;
    float fastPumpSpeed;
    bool enemiesSpawned;
    public bool isShielded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isShielded = true;
        enemiesSpawned = false;
        bossHpCurr = bossHPMax;
        GameManager.instance.bossHealthUI.SetActive(true);
        HPColorOrigin = GameManager.instance.bossHealthBar.color;

        GameManager.instance.AmbianceForLevels.SetActive(false);
        GameManager.instance.AmbianceForBoss.SetActive(true);

        heartBeatSource.clip = slowBeatClip;

        pumpAnim = GetComponent<Animation>();
        fastPumpSpeed = pumpAnim["Armature|Pumping"].speed;
        slowPumpSpeed = pumpAnim["Armature|Pumping"].speed * 0.5f;  //slow the animation while shield up
        pumpAnim["Armature|Pumping"].speed = 0.5f;

        GameManager.instance.UpdateGameGoal(1);
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
            StartCoroutine(ShieldDown());
        }
    }

    public void takeDamage(int amount)  //Can only hurt boss in shield down mode;
    {
        if (!isShielded) {
            bossHpCurr = Mathf.Clamp(bossHpCurr -= amount, 0, bossHPMax);
            GameManager.instance.bossHealthBar.fillAmount = (float)bossHpCurr / bossHPMax;

            if (bossHpCurr <= 0) {
                GameManager.instance.bossHealthUI.SetActive(false);
                Destroy(gameObject);
                GameManager.instance.Win();
            }
        }
    }

    IEnumerator ShieldDown()    //Shield is down, you can hurt boss;
    {
        Debug.Log("in Shield down");

        enemiesSpawned = false;
        isShielded = false;
        SpawnersOff();
        pumpAnim["Armature|Pumping"].speed = fastPumpSpeed;
        heartBeatSource.clip = fastBeatClip;
        GameManager.instance.bossHealthBar.color = Color.red;

        yield return new WaitForSeconds(shieldDownTime);

        isShielded = true;
        pumpAnim["Armature|Pumping"].speed = slowPumpSpeed;
        heartBeatSource.clip = slowBeatClip;
        GameManager.instance.bossHealthBar.color = HPColorOrigin;
        SpawnersOn();
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
        Instantiate(spawnerPrefab, spawner1Location);
        Instantiate(spawnerPrefab, spawner2Location);
        Instantiate(spawnerPrefab, spawner3Location);
        Instantiate(spawnerPrefab, spawner4Location);
    }

    void SpawnersOff()
    {
        Destroy(spawner1Location.GetChild(0).gameObject); 
        Destroy(spawner2Location.GetChild(0).gameObject); 
        Destroy(spawner3Location.GetChild(0).gameObject); 
        Destroy(spawner4Location.GetChild(0).gameObject); 
    }
}
