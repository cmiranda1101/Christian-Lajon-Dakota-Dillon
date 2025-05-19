using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HeartBoss : MonoBehaviour, IDamage
{
    [SerializeField] GameObject spawner1;
    [SerializeField] GameObject spawner2;
    [SerializeField] GameObject spawner3;
    [SerializeField] GameObject spawner4;
    [SerializeField] GameObject bulletSpawner;

    [SerializeField] AudioSource heartBeatSource;
    [SerializeField] AudioClip fastBeatClip;
    [SerializeField] AudioClip slowBeatClip;

    [SerializeField] int bossHPMax;
    int bossHpCurr;

    Animation pumpAnim;
    MeshRenderer model;
    Color HPcolorOrigin;

    float slowPumpSpeed;
    float fastPumpSpeed;
    bool isShielded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bossHpCurr = bossHPMax;
        isShielded = true;
        GameManager.instance.bossHealthUI.SetActive(true);
        HPcolorOrigin = GameManager.instance.bossHealthBar.color;

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
        if (GameManager.instance.CheckGameGoal() == 1) {
            SpawnersOff();
            StartCoroutine(ShieldDown());
        }
    }

    public void takeDamage(int amount)
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

    IEnumerator ShieldDown()
    {
        isShielded = false;
        pumpAnim["Armature|Pumping"].speed = fastPumpSpeed;
        heartBeatSource.clip = fastBeatClip;
        GameManager.instance.bossHealthBar.color = Color.red;

        yield return new WaitForSeconds(30);

        isShielded = true;
        pumpAnim["Armature|Pumping"].speed = slowPumpSpeed;
        heartBeatSource.clip = slowBeatClip;
        GameManager.instance.bossHealthBar.color = HPcolorOrigin;
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
        if (isShielded) {
            spawner1.SetActive(true);
            spawner2.SetActive(true);
            spawner3.SetActive(true);
            spawner4.SetActive(true);
        }
    }

    void SpawnersOff()
    {
        if (isShielded) {
            spawner1.SetActive(false);
            spawner2.SetActive(false);
            spawner3.SetActive(false);
            spawner4.SetActive(false);
        }
    }
}
