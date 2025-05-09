using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAIMelee : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [SerializeField] int HP;
    [SerializeField] float meleeRange;
    [SerializeField] float meleeDamage;
    [SerializeField] float meleeCooldown;

    float nextMeleeTime;
    Color originalColor;

    bool playerInRange;
    Transform player;

    void Start()
    {
        originalColor = model.material.color;
        player = GameManager.instance.player.transform;
        GameManager.instance.UpdateGameGoal(1);
    }

    void Update()
    {
        if (player == null || !playerInRange) return;

        // Move toward player
        agent.SetDestination(player.position);

        // Face the player if close enough
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            FacePlayer();
            TryMeleeAttack();
        }
    }

    void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void TryMeleeAttack()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (Time.time >= nextMeleeTime && distance <= meleeRange)
        {
            IDamage damageable = player.GetComponent<IDamage>();
            if (damageable != null)
            {
                damageable.takeDamage((int)meleeDamage);
                Debug.Log($"Enemy melee hit {player.name} for {meleeDamage} damage.");
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
        agent.SetDestination(player.position);

        if (HP <= 0)
        {
            GameManager.instance.UpdateGameGoal(-1);
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(FlashRed());
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
}