using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAIMelee : MonoBehaviour, IDamage
{
    [SerializeField] AudioSource walkSource;
    [SerializeField] AudioSource weaponSource;
    [SerializeField] AudioClip[] walkClips;
    [SerializeField] AudioClip[] weaponClips;

    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [SerializeField] int HP;
    [SerializeField] float meleeRange;
    [SerializeField] float meleeDamage;
    [SerializeField] float meleeCooldown;
    [SerializeField] int facePlayerSpeed;

    [SerializeField] float walkRate;

    float walkTimer;
    float nextMeleeTime;
    Color originalColor;

    bool playerInRange;
    Transform player;
    Vector3 playerDir;

    bool isMoving;

    void Start()
    {
        originalColor = model.material.color;
        player = GameManager.instance.player.transform;
        GameManager.instance.UpdateGameGoal(1);
    }

    void Update()
    {
        isMoving = true;
        walkTimer += Time.deltaTime;

        if (player == null || !playerInRange) return;

        playerDir = player.position - transform.position;

        // Move toward the player
        agent.SetDestination(player.position);

        // If close enough, face and attack
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            isMoving = false;
            FacePlayer();
            MeleeAttack();
        }
        if (walkTimer >= walkRate && isMoving) {
            WalkSound();
            walkTimer = 0f;
        }
    }

    void FacePlayer()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * facePlayerSpeed);
    }

    void MeleeAttack()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (Time.time >= nextMeleeTime && distance <= meleeRange)
        {
            IDamage damageable = player.GetComponent<IDamage>();
            if (damageable != null)
            {
                damageable.takeDamage((int)meleeDamage);
                Debug.Log($"Enemy melee hit {player.name} for {meleeDamage} damage.");
                WeaponSound();
            }
            else
            {
                Debug.LogWarning($"Target {player.name} does not have an IDamage component.");
            }

            nextMeleeTime = Time.time + meleeCooldown;
        }
    }

    public void takeDamage(int damageAmount)
    {
        HP -= damageAmount;

        if (HP <= 0)
        {
            GameManager.instance.UpdateGameGoal(-1);
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(FlashRed());
            agent.SetDestination(player.position);
        }
    }

    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        model.material.color = originalColor;
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

    void WalkSound()
    {
        int i = Random.Range(0, walkClips.Length);
        walkSource.clip = walkClips[i];
        walkSource.Play();
    }

    void WeaponSound()
    {
        int i = Random.Range(0, weaponClips.Length);
        weaponSource.clip = weaponClips[i];
        weaponSource.Play();
    }
}
