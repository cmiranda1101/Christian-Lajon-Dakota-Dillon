using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] int HP;
    [SerializeField] float moveSpeed;
    [SerializeField] float meleeRange;
    [SerializeField] float meleeDamage;
    [SerializeField] float meleeCooldown;
    [SerializeField] float nextMeleeTime;


    bool isAttacking = false;

    Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        
        MoveToPlayer();
        melee();
    }

    // Function to move the enemy toward the player
    void MoveToPlayer()
    {
        // Calculate target position, keeping only X and Z for movement on the plane
        Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);

        // Move towards the player
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Rotate the enemy to face the player
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    
    public void takeDamage(int damageAmount)
    {
        HP -= damageAmount;

        // destroy if HP drops below or equal to 0
        if (HP <= 0)
        {
            Destroy(gameObject);
        }

        
        StartCoroutine(FlashRed());
    }

    
    IEnumerator FlashRed()
    {
        if (model != null)
        {
            Color originalColor = model.material.color;
            model.material.color = Color.red;
            yield return new WaitForSeconds(0.05f);
            model.material.color = originalColor;
        }
    }
    IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(meleeCooldown);
        isAttacking = false;
    }
    void melee()
    {
        // attack range = enemy position - player position

        // if attack range is >= vec3.distance(enemy position, player position

        if (isAttacking) return; // prevent attack if already attacking

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= meleeRange&& Time.time >= nextMeleeTime)
        {
            IDamage damageable = player.GetComponent<IDamage>();
            Debug.Log($"Enemy hit {player.name} for {meleeDamage} damage.");
        }
        else
        {
            Debug.LogWarning($"Target {player.name} does not have an IDamage component.");
        }

        isAttacking = true;
        nextMeleeTime = Time.time + meleeCooldown;
        StartCoroutine(ResetAttack());

    }
}