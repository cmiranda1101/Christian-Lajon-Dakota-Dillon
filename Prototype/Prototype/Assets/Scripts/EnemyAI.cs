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
    [SerializeField] Animator anim;
    [SerializeField] Transform headPos;
    [SerializeField] GameObject meleeHitBox;

    [SerializeField] int HP;
    [SerializeField] float meleeRange;
    [SerializeField] float meleeDamage;
    [SerializeField] float meleeCooldown;
    [SerializeField] int facePlayerSpeed;
    [SerializeField] int FOV;

    [SerializeField] float walkRate;
    [SerializeField] float patrolRadius;
    [SerializeField] float patrolInterval;
    float patrolTimer;

    [SerializeField] bool enableStrafe = true;
    [SerializeField] float strafeSpeed;
    [SerializeField] float strafeDis;
    [SerializeField] float strafeDur;
    [SerializeField][Range(0f, 1f)] float chanceToStrafe;

    [SerializeField] float animTransSpeed;

    bool isStrafing = false;
    float strafeTimer = 0f;
    Vector3 strafeDir;

    float walkTimer;
    float nextMeleeTime;
    Color originalColor;

    bool playerInRange;
    bool isChasing;

    float angleToPlayer;
    Transform player;
    Vector3 playerDir;

    bool isMoving;

    float animationCurr;
    float agentSpeedCurr;

    void Start()
    {
        originalColor = model.material.color;
        player = GameManager.instance.player.transform;
        GameManager.instance.UpdateGameGoal(1);
    }

    void Update()
    {
        if(agent.velocity.magnitude >= 0.01f) 
            isMoving = true;
        walkTimer += Time.deltaTime;
        patrolTimer += Time.deltaTime;

        SetAnimParameter();

        if (isStrafing && Random.value <= 0.75f)
        {
            agent.Move(strafeDir * strafeSpeed * Time.deltaTime);
            strafeTimer += Time.deltaTime;

            if (strafeTimer >= strafeDur)
            {
                isStrafing = false;
            }

            return;
        }

        if (player == null) return;

        bool canSeePlayer = playerInRange && CanSeePlayer();

        if(canSeePlayer)
        {
            StartChasing();
        }
        else if(isChasing)
        {
            ContinueChasing();
        }
        // handles chasing and wandering
        if(isChasing)
        {
            HandleChase();
        }
        else
        {
            Wander();
        }
        if (walkTimer >= walkRate && isMoving) 
        {
            WalkSound();
            walkTimer = 0f;
        }
    }

    void SetAnimParameter()
    {
        agentSpeedCurr = agent.velocity.normalized.magnitude;
        animationCurr = anim.GetFloat("Speed");

        anim.SetFloat("Speed", Mathf.Lerp(animationCurr, agentSpeedCurr, Time.deltaTime * animTransSpeed));
    }
    bool CanSeePlayer()
    {
        if (player == null || headPos == null)
            return false;

        // calculate the direction to the player from the head position
        playerDir = player.position - headPos.position;
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);

        // show me line to player
        Debug.DrawRay(headPos.position, playerDir.normalized * 50f, Color.red);

        if (angleToPlayer <= FOV) // Check if the player is within FOV angle
        {
            RaycastHit hit;
            if (Physics.Raycast(headPos.position, playerDir.normalized, out hit, Mathf.Infinity))
            {
                if (hit.collider.CompareTag("Player")) // Check if the raycast hits the player
                {
                    return true;
                }
            }
        }

        return false;
    }
    // Start chasing the player
    void StartChasing()
    {
        isChasing = true;
    }

    // Continue chasing after losing sight for a while
    void ContinueChasing()
    {
        if (Vector3.Distance(transform.position, player.position) > patrolRadius)
        {
            isChasing = false;
        }
    }

    // Handle the actual chasing logic
    void HandleChase()
    {
        isMoving = true;
        agent.SetDestination(player.position);

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            isMoving = false;
            FacePlayer();
            MeleeAttack();
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
        if (Time.time >= nextMeleeTime && distance <= meleeRange) {
            //IDamage damageable = player.GetComponent<IDamage>();
            //if (damageable != null) {
            //    damageable.takeDamage((int)meleeDamage);
            //    Debug.Log($"Enemy melee hit {player.name} for {meleeDamage} damage.");
            //    WeaponSound();
            //}
            //else {
            //    Debug.LogWarning($"Target {player.name} does not have an IDamage component.");
            //}
            anim.SetTrigger("meleeAtk");
            nextMeleeTime = Time.time + meleeCooldown;
        }
    }

    public void MeleeColOn()
    {
        if(meleeHitBox)
            meleeHitBox.SetActive(true);
    }

    public void MeleeColOff()
    {
        if(meleeHitBox)
            meleeHitBox.SetActive(false);
    }

    public void takeDamage(int damageAmount)
    {
        HP -= damageAmount;

        if (HP <= 0) {
            GameManager.instance.UpdateGameGoal(-1);
            Destroy(gameObject);
        }
        else {
            StartCoroutine(FlashRed());
            agent.SetDestination(player.position);

            if (enableStrafe && Random.value <= chanceToStrafe)
            {
                Strafe();
            }
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
        if (other.CompareTag("Player")) {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) {
            playerInRange = false;
        }
    }
    // Wander around the patrol area when not chasing
    void Wander()
    {
        if (agent.remainingDistance <= agent.stoppingDistance && patrolTimer >= patrolInterval)
        {
            Vector3 newPos = RandomNavSphere(transform.position, patrolRadius, -1);
            agent.SetDestination(newPos);
            patrolTimer = 0f;
            isMoving = true;
           
        }
    }

    // Get a random position within a sphere around the enemy
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
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
    void Strafe()
    {
        Vector3 right = transform.right;
        Vector3 left = -transform.right;

        strafeDir = (Random.value > .5f ? right : left).normalized;
        strafeDir *= strafeDis;

        isStrafing = true;
        strafeTimer = 0f;

        
    }
}
