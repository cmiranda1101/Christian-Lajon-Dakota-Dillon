using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class EnemyAiRange : MonoBehaviour, IDamage, IHeardSomething
{
    [SerializeField] AudioSource walkSource;
    [SerializeField] AudioSource gunSource;
    [SerializeField] AudioClip[] walkClips;
    [SerializeField] AudioClip[] gunClips;

    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] int FOV;
    [SerializeField] Transform headPos;
    [SerializeField] int HP;
    [SerializeField] int facePlayerSpeed;
    [SerializeField] int deadTime;

    [SerializeField] Transform shootingPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float fireRate;

    // chase varibles 
    [SerializeField] float chaseDur;
    [SerializeField] float forgetDelay;
    float lastSeen = Mathf.Infinity;
    bool isChasing = false;

    // patrol variables 
    [SerializeField] float patrolRadius;
    [SerializeField] float patrolInterval;
    float patrolTimer;
    private Vector3 patrolOrigin; // stores spawn position to roam around

    // strafe variables
    [SerializeField] bool enableStrafe = true;
    [SerializeField] float strafeSpeed;
    [SerializeField] float strafeDis;
    [SerializeField] float strafeDur;
    [SerializeField][Range (0f, 1f)] float chanceToStrafe;

    [SerializeField] float animTransSpeed;

    bool isStrafing = false;
    float strafeTimer = 0f;
    Vector3 strafeDir;

    Color colorOrig;
    Vector3 playerDir;

    [SerializeField] float walkRate;
    float walkTimer;
    float shootTimer;

    float animationCurr;
    float agentSpeedCurr;

    bool isPlayerInSightline;
    bool playerInRange;
    bool isMoving;
    bool isDead;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;
        patrolOrigin = transform.position; // stores the original spawn point
       
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;
        // Increment timers
        shootTimer += Time.deltaTime;
        walkTimer += Time.deltaTime;
        patrolTimer += Time.deltaTime;

        SetAnimParameter();

        if(isStrafing && Random.value <= chanceToStrafe)
        {
            agent.Move(strafeDir * strafeSpeed * Time.deltaTime);
            strafeTimer += Time.deltaTime;

            if (strafeTimer >= strafeDur)
            {
                isStrafing = false;
            }

            return;
        }

        // Check if player is in range and can be seen
        bool canSeePlayer = playerInRange && CanSeePlayer() && !GameManager.instance.playerScript.isHiding;

        // Handle chasing and movement
        if (canSeePlayer)
        {
            StartChasing();
        }
        else if (isChasing)
        {
            ContinueChasing();
        }

        // Perform actions based on state (chasing or patrolling)
        if (isChasing)
        {
            HandleChase();
        }
        else
        {
            Wander();
        }
    }

    void SetAnimParameter()
    {
        agentSpeedCurr = agent.velocity.normalized.magnitude;
        animationCurr = anim.GetFloat("Speed");

        anim.SetFloat("Speed", Mathf.Lerp(animationCurr, agentSpeedCurr, Time.deltaTime * animTransSpeed));
    }

    // Check if the enemy can see the player
    bool CanSeePlayer()
    {
        if (GameManager.instance.player == null || headPos == null)
            return false;

        Vector3 directionToPlayer = GameManager.instance.player.transform.position - headPos.position;
        float angleToPlayer = Vector3.Angle(new Vector3(directionToPlayer.x, 0, directionToPlayer.z), transform.forward);

        //Debug.DrawRay(headPos.position, directionToPlayer.normalized , Color.red);

        IsPlayerInSightline(directionToPlayer);

        if (angleToPlayer <= FOV)
        {
            RaycastHit hit;
            if (Physics.Raycast(headPos.position, directionToPlayer.normalized, out hit, Mathf.Infinity))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    return true;
                }
            }
        }

        return false;
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
            playerInRange = false;
        }
    }

    public void takeDamage(int amount)
    {
        if (isDead) return;
        HP -= amount;

        if (HP <= 0)
        {
            StartCoroutine(Death());
        }
        else
        {
            StartCoroutine(flashRed());
            GameManager.instance.playerScript.isHiding = false;
            agent.SetDestination(GameManager.instance.player.transform.position);

            if(enableStrafe && Random.value <= chanceToStrafe)
            {
                Strafe();
            }
        }
    }

    IEnumerator Death()
    {
        isDead = true;
        anim.SetBool("isDead", true);

        agent.ResetPath();
        agent.isStopped = true;
        agent.enabled = false;

        foreach (Collider col in GetComponentsInChildren<Collider>()) {
            col.enabled = false;
        }
        StartCoroutine(AddLayerFadeOut(1, 1));

        yield return new WaitForSeconds(deadTime);
        Destroy(gameObject);
    }

    IEnumerator AddLayerFadeOut(int index, float fadeTime)
    {
        float timer = 0f;
        float weight = anim.GetLayerWeight(index);

        while (timer < fadeTime) {
            float newWeight = Mathf.Lerp(weight, 0, timer / fadeTime);
            anim.SetLayerWeight(index, newWeight);
            timer += Time.deltaTime;
            yield return null;
        }
        anim.SetLayerWeight(index, 0);
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        model.material.color = colorOrig;
    }

    // Start chasing the player
    void StartChasing()
    {
        isChasing = true;
        lastSeen = 0f;
    }

    // Continue chasing the player after losing sight
    void ContinueChasing()
    {
        lastSeen += Time.deltaTime;

        if (lastSeen >= chaseDur)
        {
            isChasing = false;
        }
    }

    // Handle the actual chasing logic
    void HandleChase()
    {
        isMoving = true;
        playerDir = GameManager.instance.player.transform.position - transform.position;
        agent.SetDestination(GameManager.instance.player.transform.position);

        // Shoot at the player if the fire rate allows
        if (shootTimer >= fireRate)
        {
            anim.SetTrigger("Shoot");
        }

        // Stop moving and face the player when close enough
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            isMoving = false;
            facePlayer();
        }

        // Play walking sound if moving
        if (walkTimer >= walkRate && isMoving)
        {
            WalkSound();
            walkTimer = 0f;
        }
    }

    // Make the enemy face the player
    void facePlayer()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * facePlayerSpeed);
    }

    // Shoot at the player
    void shoot()
    {
        if (isDead) return;
        
        Vector3 playerPos = GameManager.instance.player.transform.position + Vector3.up * 1.0f; 
        Vector3 dirToPlayer = (playerPos - shootingPos.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation(dirToPlayer);

        Instantiate(bullet, shootingPos.position, lookRot);
    }

    // Play walk sound
    void WalkSound()
    {
        int i = Random.Range(0, walkClips.Length);
        AudioManager.PlaySFX(walkSource, walkClips[i]);
    }

    // Play gunshot sound
    void GunShotSound()
    {
        if (isDead) return;

        int i = Random.Range(0, gunClips.Length);
        AudioManager.PlaySFX(gunSource, gunClips[i]);
    }

    // Randomly choose a position within a given radius
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    // Wander when not chasing the player
    void Wander()
    {
        if (agent.remainingDistance <= agent.stoppingDistance && patrolTimer >= patrolInterval)
        {
            Vector3 newPos = RandomNavSphere(patrolOrigin, patrolRadius, -1);
            agent.SetDestination(newPos);
            patrolTimer = 0f;
            isMoving = true;
        }

        // Play walk sound if wandering
        if (walkTimer >= walkRate && isMoving)
        {
            WalkSound();
            walkTimer = 0f;
        }

        // Increment lastSeen timer if not chasing
        if (!isChasing)
        {
            lastSeen += Time.deltaTime;
        }
    }
    void Strafe()
    {
        Vector3 right = transform.right;
        Vector3 left = -transform.right;

        strafeDir = (Random.value > .5f ? right : left).normalized;
        strafeDir *= strafeDis;

        isStrafing = true;
        strafeTimer = 0f;

        anim.SetTrigger("Shoot");
    }

    void IsPlayerInSightline(Vector3 directionToPlayer)
    {
        RaycastHit hit;
        if (Physics.Raycast(headPos.position, directionToPlayer, out hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag("Player"))
            {
                isPlayerInSightline = true;
            }
            else
            {
                isPlayerInSightline = false;
            }
        }
    }

    public void OnHeardSomething(Vector3 soundPosition, float soundRadius)
    {
        if (CanSeePlayer() && playerInRange) { return; }
        StartCoroutine(HandleHeardSomething(soundPosition, soundRadius));
    }

    IEnumerator HandleHeardSomething(Vector3 soundPosition, float soundRadius)
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
            yield break;

        agent.SetDestination(soundPosition);
        patrolTimer = 0;
        isMoving = true;

        yield return new WaitUntil(() => agent.pathPending == false);

        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
            yield break;

        if (isPlayerInSightline == true && agent.remainingDistance <= agent.stoppingDistance && agent.remainingDistance < soundRadius)
        {
            StartChasing();
        }
        else if (isPlayerInSightline == false || agent.remainingDistance > soundRadius)
        {
            if (agent == null || !agent.enabled || !agent.isOnNavMesh)
                yield break;
            agent.SetDestination(patrolOrigin);
        }
    }
}
