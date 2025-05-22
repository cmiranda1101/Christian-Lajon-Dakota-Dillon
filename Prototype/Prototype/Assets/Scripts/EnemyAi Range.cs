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
    [SerializeField] int FOV;
    [SerializeField] Transform headPos;
    [SerializeField] int HP;
    [SerializeField] int facePlayerSpeed;

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

    // strafe variables
    [SerializeField] bool enableStrafe = true;
    [SerializeField] float strafeSpeed;
    [SerializeField] float strafeDis;
    [SerializeField] float strafeDur;
    [SerializeField][Range (0f, 1f)] float chanceToStrafe;
    

    bool isStrafing = false;
    float strafeTimer = 0f;
    Vector3 strafeDir;

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
        // Increment timers
        shootTimer += Time.deltaTime;
        walkTimer += Time.deltaTime;
        patrolTimer += Time.deltaTime;

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
        bool canSeePlayer = playerInRange && CanSeePlayer();

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

    // Check if the enemy can see the player
    bool CanSeePlayer()
    {
        if (GameManager.instance.player == null || headPos == null)
            return false;

        Vector3 directionToPlayer = GameManager.instance.player.transform.position - headPos.position;
        float angleToPlayer = Vector3.Angle(new Vector3(directionToPlayer.x, 0, directionToPlayer.z), transform.forward);

        Debug.DrawRay(headPos.position, directionToPlayer.normalized , Color.red);

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

            if(enableStrafe && Random.value <= chanceToStrafe)
            {
                Strafe();
            }
        }
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
            shoot();
            GunShotSound();
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
        shootTimer = 0;
        Instantiate(bullet, shootingPos.position, transform.rotation);
    }

    // Play walk sound
    void WalkSound()
    {
        int i = Random.Range(0, walkClips.Length);
        walkSource.clip = walkClips[i];
        walkSource.Play();
    }

    // Play gunshot sound
    void GunShotSound()
    {
        int i = Random.Range(0, gunClips.Length);
        gunSource.clip = gunClips[i];
        gunSource.Play();
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
            Vector3 newPos = RandomNavSphere(transform.position, patrolRadius, -1);
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

        shoot();
    }
}
