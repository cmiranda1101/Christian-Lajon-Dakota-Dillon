using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAIMelee : MonoBehaviour, IDamage, IHeardSomething
{
    [SerializeField] AudioSource walkSource;
    [SerializeField] AudioSource weaponSource;
    [SerializeField] AudioClip[] walkClips;
    [SerializeField] AudioClip[] weaponClips;

    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] Transform headPos;
    [SerializeField] GameObject[] meleeHitBox;

    [SerializeField] int HP;
    [SerializeField] float meleeRange;
    [SerializeField] float meleeDamage;
    [SerializeField] float meleeCooldown;
    [SerializeField] int facePlayerSpeed;
    [SerializeField] int FOV;

    [SerializeField] bool allowRoam = true;
    [SerializeField] float walkRate;
    [SerializeField] float patrolRadius;
    [SerializeField] float patrolInterval;
    float patrolTimer;
    private Vector3 patrolOrigin;

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

    bool isPlayerInSightline;
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

        patrolOrigin = transform.position;
        agent.stoppingDistance = meleeRange * 0.9f; // stops a bit before melee range
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

        bool canSeePlayer = playerInRange && CanSeePlayer() && !GameManager.instance.playerScript.isHiding;

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

        // Direction from enemy's head to player
        playerDir = player.position - headPos.position;

        IsPlayerInSightline();

        // Flat angle to player
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);
        // Check FOV angle
        if (angleToPlayer <= FOV)
        {
            // Ignores EnemyPassable wall with raycast to see the player
            int ignoreLayer = LayerMask.NameToLayer("EnemyPassable");
            int raycastMask = ~(1 << ignoreLayer); // Invert to exclude that layer

            RaycastHit hit;
            if (Physics.Raycast(headPos.position, playerDir.normalized, out hit, Mathf.Infinity, raycastMask))
            {
                // Check if ray hits the player
                if (hit.collider.CompareTag("Player"))
                {
                    return true;
                }
             }
        }
        return false;
    }

    void IsPlayerInSightline()
    {
        RaycastHit hit;
        if(Physics.Raycast(headPos.position, playerDir.normalized, out hit, Mathf.Infinity))
        {
            if(hit.collider.CompareTag("Player"))
            {
                isPlayerInSightline = true;
            }
        }
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
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Vector3 targetPosition = player.position - directionToPlayer * agent.stoppingDistance;

        agent.SetDestination(player.position);

        if (agent.remainingDistance <= agent.stoppingDistance + 0.1f)
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
           
            anim.SetTrigger("meleeAtk");
            nextMeleeTime = Time.time + meleeCooldown;
        }
    }

    public void MeleeColOn()
    {
        for (int i = 0; i < meleeHitBox.Length; ++i) {
            if (meleeHitBox[i])
                meleeHitBox[i].SetActive(true);
        }
    }

    public void MeleeColOff()
    {
        for (int i = 0; i < meleeHitBox.Length; ++i) {
            if (meleeHitBox[i])
                meleeHitBox[i].SetActive(false);
        }
    }

    public void takeDamage(int damageAmount)
    {
        HP -= damageAmount;

        if (HP <= 0) {
           
            Destroy(gameObject);
        }
        else {
            GameManager.instance.playerScript.isHiding = false;
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
        if (!allowRoam) return; // doesn't roam if not checked

        if (agent.remainingDistance <= agent.stoppingDistance && patrolTimer >= patrolInterval)
        {
            Vector3 newPos = RandomNavSphere(patrolOrigin, patrolRadius, -1);
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
        AudioManager.PlaySFX(walkSource, walkClips[i]);
    }

    void WeaponSound()
    {
        int i = Random.Range(0, weaponClips.Length);
        AudioManager.PlaySFX(weaponSource, weaponClips[i]);
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
    public void OnHeardSomething(Vector3 soundPosition, float soundRadius)
    {
        if (CanSeePlayer() && playerInRange) { return; }
        agent.SetDestination(soundPosition);
        if(agent.remainingDistance < soundRadius)
        {
            allowRoam = false;
            StartCoroutine(WaitToRoam());
        } 
        else if (isPlayerInSightline == false || agent.remainingDistance > soundRadius)
        {
            agent.SetDestination(patrolOrigin);
        } 
        if (isPlayerInSightline == true && agent.remainingDistance < agent.stoppingDistance)
        {
            FacePlayer();
        }
    }

    IEnumerator WaitToRoam()
    {
        yield return new WaitUntil(() => CanSeePlayer() == false);
        allowRoam = true;
    }
}
