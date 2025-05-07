using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] int HP;
    [SerializeField] float moveSpeed;

    Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        
        MoveToPlayer();
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
}


