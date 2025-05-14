using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class EnemyAiRange : MonoBehaviour, IDamage
{
    [SerializeField] AudioSource walkSource;
    [SerializeField] AudioSource gunSource;
    [SerializeField] AudioClip[] walkClips;
    [SerializeField] AudioClip[] gunClips;

    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [SerializeField] int HP;
    [SerializeField] int facePlayerSpeed;

    [SerializeField] Transform shootingPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float fireRate;

    Color colorOrig;

    Vector3 playerDir;

    [SerializeField] float walkRate;
    float walkTimer;
    float shootTimer;
    

    bool playerInRange;
    bool isMoving;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;
        GameManager.instance.UpdateGameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime;
        walkTimer += Time.deltaTime;

        if (playerInRange)
        {
            isMoving = true;
            playerDir = (GameManager.instance.player.transform.position - transform.position);
            agent.SetDestination(GameManager.instance.player.transform.position);

            if (shootTimer >= fireRate)
            {
                shoot();
                GunShotSound();
            }
            if (agent.remainingDistance<= agent.stoppingDistance)
            {
                isMoving = false;
                facePlayer();
            }
            if(walkTimer>= walkRate && isMoving) {
                WalkSound();
                walkTimer = 0f;
            }
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    public void takeDamage(int amount)
    {
        HP -= amount;

        if (HP <= 0)
        {
            GameManager.instance.UpdateGameGoal(-1);
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(flashRed());
            agent.SetDestination(GameManager.instance.player.transform.position);
        }
    }
    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        model.material.color = colorOrig;
    }
    void facePlayer()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * facePlayerSpeed);
    }
    void shoot()
    {
        shootTimer = 0;
        Instantiate(bullet, shootingPos.position, transform.rotation);
    }

    void WalkSound()
    {
        int i = Random.Range(0, walkClips.Length);
        walkSource.clip = walkClips[i];
        walkSource.Play();
    }

    void GunShotSound()
    {
        int i = Random.Range(0, gunClips.Length);
        gunSource.clip = gunClips[i];
        gunSource.Play();
    }
}